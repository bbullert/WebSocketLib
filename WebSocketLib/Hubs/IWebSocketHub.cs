using System.Net.WebSockets;
using WebSocketLib.Models;

namespace WebSocketLib.Hubs
{
    public interface IWebSocketHub
    {
        WebSocketHubContext Context { get; }
        Task<string?> OnConnectedAsync(WebSocket socket, string userId);
        Task<string?> OnDisconnectedAsync(WebSocket socket, string message, WebSocketCloseStatus status);
        Task DisconnectAsync(WebSocket socket, string message, WebSocketCloseStatus status);
        Task SendAsync(params dynamic[] data);
        void SetContext(WebSocket socket);
    }
}