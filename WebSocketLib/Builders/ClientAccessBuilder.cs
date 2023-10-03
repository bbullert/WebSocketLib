using WebSocketLib.Hubs;
using WebSocketLib.Services;

namespace WebSocketLib.Builders
{
    public class ClientAccessBuilder
    {
        private IEnumerable<string> _clients;
        private readonly IWebSocketHub _webSocketHub;
        private readonly IUserService _userService;
        private readonly IGroupService _groupService;

        public ClientAccessBuilder(
            IWebSocketHub webSocketHub,
            IUserService userService,
            IGroupService groupService)
        {
            _clients = Enumerable.Empty<string>();
            _webSocketHub = webSocketHub;
            _userService = userService;
            _groupService = groupService;
        }

        public IEnumerable<string> Selected => _clients;

        public IWebSocketHub All
        {
            get
            {
                _clients = _userService.GetAllUsers().Keys.ToList();
                return _webSocketHub;
            }
        }

        public IWebSocketHub Caller
        {
            get
            {
                _clients = _userService.GetAllUsers().Keys
                    .Where(x => x == _webSocketHub.Context.UserId)
                    .ToList();
                return _webSocketHub;
            }
        }

        public IWebSocketHub Others
        {
            get
            {
                _clients = _userService.GetAllUsers().Keys
                    .Where(x => x != _webSocketHub.Context.UserId)
                    .ToList();
                return _webSocketHub;
            }
        }

        public IWebSocketHub User(string userId)
        {
            _clients = _userService.GetAllUsers().Keys
                .Where(x => x == userId)
                .ToList();
            return _webSocketHub;
        }

        public IWebSocketHub Users(params string[] userIds)
        {
            return Users(userIds);
        }

        public IWebSocketHub Users(IEnumerable<string> userIds)
        {
            _clients = _userService.GetAllUsers().Keys
                .Where(x => userIds.Contains(x))
                .ToList();
            return _webSocketHub;
        }

        public IWebSocketHub Group(string groupId)
        {
            _clients = _groupService.GetGroupMembers(groupId);
            return _webSocketHub;
        }

        public IWebSocketHub Groups(params string[] groupIds)
        {
            return Groups(groupIds);
        }

        public IWebSocketHub Groups(IEnumerable<string> groupIds)
        {
            var clients = new List<string>();
            foreach (var groupId in groupIds)
            {
                var groupMembers = _groupService.GetGroupMembers(groupId);
                clients.Concat(groupMembers);
            }
            _clients = clients.Distinct().ToList();
            return _webSocketHub;
        }

        public IWebSocketHub OthersInGroup(string groupId)
        {
            _clients = _groupService.GetGroupMembers(groupId)
                .Where(x => x != _webSocketHub.Context.UserId)
                .ToList();
            return _webSocketHub;
        }

        public IWebSocketHub OthersInGroups(params string[] groupIds)
        {
            return OthersInGroups(groupIds);
        }

        public IWebSocketHub OthersInGroups(IEnumerable<string> groupIds)
        {
            var clients = new List<string>();
            foreach (var groupId in groupIds)
            {
                var groupMembers = _groupService.GetGroupMembers(groupId);
                clients.Concat(groupMembers);
            }
            _clients = clients
                .Distinct()
                .Where(x => x != _webSocketHub.Context.UserId)
                .ToList();
            return _webSocketHub;
        }
    }
}
