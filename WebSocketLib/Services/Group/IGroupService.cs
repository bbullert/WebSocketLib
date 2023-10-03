namespace WebSocketLib.Services
{
    public interface IGroupService
    {
        IEnumerable<string> GetAllGroups();
        IEnumerable<string> GetGroupMembers(string groupId);
        IEnumerable<string> GetUserGroups(string userId);
        bool TryAddUserToGroup(string userId, string groupId);
        bool TryRemoveUserFromGroup(string userId, string groupId);
    }
}