using System.Net.WebSockets;
using WebSocketLib.Hubs;
using WebSocketLib.Services;

namespace WebSocketLib.Example
{
    public class ExampleHub : WebSocketHub
    {
        public ExampleHub(
            IConnectionService connectionService,
            IUserService userService,
            IGroupService groupService)
            : base(
                connectionService,
                userService,
                groupService)
        {
        }

        public async Task SendMessageToAllAsync(string message)
        {
            await Clients.All.SendAsync("ReceiveMessageToAll", message);
        }

        public async Task SendMessageToCallerAsync(string message)
        {
            await Clients.Caller.SendAsync("ReceiveMessageToCaller", message);
        }

        public async Task SendMessageToOthersAsync(string message)
        {
            await Clients.Others.SendAsync("ReceiveMessageToOthers", message);
        }

        public async Task SendMessageToUserAsync(string message, string userId)
        {
            await Clients.User(userId).SendAsync("ReceiveMessageToUser", message);
        }

        public async Task SendMessageToUsersAsync(string message, params string[] userIds)
        {
            await Clients.Users(userIds).SendAsync("ReceiveMessageToUsers", message);
        }

        public async Task SendMessageToGroupAsync(string message, string groupId)
        {
            await Clients.Group(groupId).SendAsync("ReceiveMessageToGroup", message);
        }

        public async Task SendMessageToGroupsAsync(string message, params string[] groupIds)
        {
            await Clients.Groups(groupIds).SendAsync("ReceiveMessageToGroups", message);
        }

        public async Task SendMessageToOthersInGroupAsync(string message, string groupId)
        {
            await Clients.OthersInGroup(groupId).SendAsync("ReceiveMessageToOthersInGroup", message);
        }

        public async Task SendMessageToOthersInGroupsAsync(string message, params string[] groupIds)
        {
            await Clients.OthersInGroups(groupIds).SendAsync("ReceiveMessageToOthersInGroups", message);
        }

        public async Task SendMultiParamMessageAsync(string message, float number, bool flag)
        {
            await Clients.All.SendAsync("ReceiveMultiParamMessage", message, number, flag);
        }

        public void JoinGroup(string groupId)
        {
            JoinGroup(groupId, Context.UserId);
        }

        public void JoinGroup(string groupId, string userId)
        {
            Console.WriteLine("User: " + userId + " joined the group: " + groupId);
            Groups.Join(groupId, userId);
        }

        public void LeaveGroup(string groupId)
        {
            LeaveGroup(groupId, Context.UserId);
        }

        public void LeaveGroup(string groupId, string userId)
        {
            Console.WriteLine("User: " + userId + " left the group: " + groupId);
            Groups.Leave(groupId, userId);
        }

        public override async Task<string?> OnConnectedAsync(WebSocket socket, string userId)
        {
            var connectionId = await base.OnConnectedAsync(socket, userId);
            Console.WriteLine("Connected: " + connectionId);
            return connectionId;
        }

        public override async Task<string?> OnDisconnectedAsync(WebSocket socket, string errorMessage, WebSocketCloseStatus status)
        {
            var connectionId = await base.OnDisconnectedAsync(socket, errorMessage, status);
            Console.WriteLine("Disconnected: " + connectionId);
            return connectionId;
        }
    }
}
