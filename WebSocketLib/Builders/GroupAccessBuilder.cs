using WebSocketLib.Hubs;
using WebSocketLib.Services;

namespace WebSocketLib.Builders
{
    public class GroupAccessBuilder
    {
        private readonly IWebSocketHub _webSocketHub;
        private readonly IGroupService _groupService;

        public GroupAccessBuilder(
            IWebSocketHub webSocketHub, 
            IGroupService groupService)
        {
            _webSocketHub = webSocketHub;
            _groupService = groupService;
        }

        public void Join(string groupId)
        {
            Join(_webSocketHub.Context.UserId, groupId);
        }

        public void Join(string groupId, string userId)
        {
            _groupService.TryAddUserToGroup(userId, groupId);
        }

        public void Leave(string groupId)
        {
            Leave(_webSocketHub.Context.UserId, groupId);
        }

        public void Leave(string groupId, string userId)
        {
            _groupService.TryRemoveUserFromGroup(userId, groupId);
        }
    }
}
