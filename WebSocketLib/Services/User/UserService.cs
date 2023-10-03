using System.Collections.Concurrent;

namespace WebSocketLib.Services
{
    public class UserService : IUserService
    {
        private static ConcurrentDictionary<string, string> _users = new ConcurrentDictionary<string, string>();

        public ConcurrentDictionary<string, string> GetAllUsers()
        {
            return _users;
        }

        public string? GetConnectionId(string userId)
        {
            return _users.FirstOrDefault(p => p.Key == userId).Value;
        }

        public string? GetUserId(string connectionId)
        {
            return _users.FirstOrDefault(p => p.Value == connectionId).Key;
        }

        public bool TryAddUser(string userId, string connectionId)
        {
            var result = _users.TryAdd(userId, connectionId);
            return result;
        }

        public bool TryRemoveUser(string userId)
        {
            var result = _users.TryRemove(userId, out _);
            return result;
        }
    }
}