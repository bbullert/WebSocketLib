using System.Collections.Concurrent;

namespace WebSocketLib.Services
{
    public interface IUserService
    {
        string? GetConnectionId(string userId);
        string? GetUserId(string connectionId);
        ConcurrentDictionary<string, string> GetAllUsers();
        bool TryAddUser(string userId, string connectionId);
        bool TryRemoveUser(string userId);
    }
}