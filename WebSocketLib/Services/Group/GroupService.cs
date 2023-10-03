using System.Collections.Concurrent;

namespace WebSocketLib.Services
{
    public class GroupService : IGroupService
    {
        private static ConcurrentDictionary<KeyValuePair<string, string>, bool> _groupMemberPairs = new ConcurrentDictionary<KeyValuePair<string, string>, bool>();

        public IEnumerable<string> GetGroupMembers(string groupId)
        {
            var groupMembers = _groupMemberPairs.Keys
                .Where(x => x.Key == groupId).Select(x => x.Value).ToList();
            return groupMembers;
        }

        public IEnumerable<string> GetUserGroups(string userId)
        {
            var groups = _groupMemberPairs.Keys
                .Where(x => x.Value == userId).Select(x => x.Key).ToList();
            return groups;
        }

        public IEnumerable<string> GetAllGroups()
        {
            var groups = _groupMemberPairs.Keys
                .Select(x => x.Key).Distinct().ToList();
            return groups;
        }

        public bool TryAddUserToGroup(string userId, string groupId)
        {
            var result = _groupMemberPairs.TryAdd(new KeyValuePair<string, string>(groupId, userId), false);
            return result;
        }

        public bool TryRemoveUserFromGroup(string userId, string groupId)
        {
            var result = _groupMemberPairs.TryRemove(new KeyValuePair<string, string>(groupId, userId), out _);
            return result;
        }
    }
}
