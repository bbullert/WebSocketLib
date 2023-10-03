using System.Net.WebSockets;
using System.Text;
using WebSocketLib.Builders;
using WebSocketLib.Models;
using WebSocketLib.Services;

namespace WebSocketLib.Hubs
{
    public abstract class WebSocketHub : IWebSocketHub
    {
        protected readonly IConnectionService _connectionService;
        protected readonly IUserService _userService;
        protected readonly IGroupService _groupService;
        protected readonly ClientAccessBuilder _clientAccessBuilder;
        protected readonly GroupAccessBuilder _groupAccessBuilder;

        public WebSocketHub(
            IConnectionService connectionService, 
            IUserService userService, 
            IGroupService groupService)
        {
            _connectionService = connectionService;
            _userService = userService;
            _groupService = groupService;
            _clientAccessBuilder = new ClientAccessBuilder(this, _userService, _groupService);
            _groupAccessBuilder = new GroupAccessBuilder(this, _groupService);
        }

        public WebSocketHubContext Context { get; protected set; }
        protected ClientAccessBuilder Clients => _clientAccessBuilder;
        protected GroupAccessBuilder Groups => _groupAccessBuilder;

        public async Task SendAsync(params dynamic[] data)
        {
            foreach (var userId in Clients.Selected)
            {
                var connectionId = _userService.GetConnectionId(userId);
                if (string.IsNullOrEmpty(connectionId)) continue;
                var socket = _connectionService.GetSocket(connectionId);
                if (socket == null) continue;
                await SendAsync(socket, data);
            }
        }

        protected async Task SendAsync(WebSocket socket, params dynamic[] data)
        {
            if (socket.State != WebSocketState.Open)
                return;

            var str = WebSocketPayload.Seriazlie(data);
            var buffer = Encoding.ASCII.GetBytes(str);

            await socket.SendAsync(
                new ArraySegment<byte>(buffer, 0, str.Length),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );
        }

        public virtual async Task<string?> OnConnectedAsync(WebSocket socket, string userId)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));

            var connectionId = _connectionService.TryAddSocket(socket);
            if (connectionId == null) throw new ApplicationException("Unable to add a socket");

            var result = _userService.TryAddUser(userId, connectionId);
            if (!result) throw new ApplicationException("Unable to add a user");

            return connectionId;
        }

        public virtual async Task<string?> OnDisconnectedAsync(
            WebSocket socket,
            string message,
            WebSocketCloseStatus status)
        {
            var connectionId = _connectionService.GetConnectionId(socket);
            if (connectionId == null) throw new ApplicationException("Unable to determine a connection");

            var userId = _userService.GetUserId(connectionId);
            if (userId == null) throw new ApplicationException("Unable to determine a user");

            bool result;
            var uesrGroups = _groupService.GetUserGroups(userId);
            foreach (var group in uesrGroups)
            {
                result = _groupService.TryRemoveUserFromGroup(userId, group);
                if (!result) throw new ApplicationException("Unable to remove a user from a group");
            }

            result = _userService.TryRemoveUser(userId);
            if (!result) throw new ApplicationException("Unable to remove a user");

            result = _connectionService.TryRemoveSocket(socket);
            if (!result) throw new ApplicationException("Unable to remove a socket");

            await DisconnectAsync(socket, message, status);

            return connectionId;
        }

        public virtual async Task DisconnectAsync(
            WebSocket socket, 
            string message, 
            WebSocketCloseStatus status)
        {
            await socket.CloseAsync(status, message, CancellationToken.None);
        }

        public void SetContext(WebSocket socket)
        {
            var connectionId = _connectionService.GetConnectionId(socket);
            if (connectionId == null) throw new ApplicationException("Unable to determine a connection");

            var userId = _userService.GetUserId(connectionId);
            if (userId == null) throw new ApplicationException("Unable to determine a user");

            Context = new WebSocketHubContext()
            {
                ConnectionId = connectionId,
                UserId = userId
            };
        }
    }
}
