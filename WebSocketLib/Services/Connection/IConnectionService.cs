using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace WebSocketLib.Services
{
    public interface IConnectionService
    {
        string? TryAddSocket(WebSocket socket);
        ConcurrentDictionary<string, WebSocket> GetAllConnections();
        string? GetConnectionId(WebSocket socket);
        WebSocket? GetSocket(string connectionId);
        bool TryRemoveSocket(WebSocket socket);
    }
}