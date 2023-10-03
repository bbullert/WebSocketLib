using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace WebSocketLib.Services
{
    public class ConnectionService : IConnectionService
    {
        private static ConcurrentDictionary<string, WebSocket> _connections = new ConcurrentDictionary<string, WebSocket>();

        public ConcurrentDictionary<string, WebSocket> GetAllConnections()
        {
            return _connections;
        }

        public string? GetConnectionId(WebSocket socket)
        {
            return _connections.FirstOrDefault(p => p.Value == socket).Key;
        }

        public WebSocket? GetSocket(string connectionId)
        {
            return _connections.FirstOrDefault(p => p.Key == connectionId).Value;
        }

        public string? TryAddSocket(WebSocket socket)
        {
            string connectionId = CreateConnectionId();
            var result = _connections.TryAdd(connectionId, socket);
            return result ? connectionId : null;
        }

        public bool TryRemoveSocket(WebSocket socket)
        {
            var connectionId = GetConnectionId(socket);
            if (string.IsNullOrEmpty(connectionId))
                return false;

            var result = _connections.TryRemove(connectionId, out _);
            return result;
        }

        private string CreateConnectionId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
