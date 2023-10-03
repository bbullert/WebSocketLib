using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Text;
using WebSocketLib.Hubs;
using WebSocketLib.Models;
using WebSocketLib.Services;

namespace WebSocketLib.Middleware
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private IWebSocketHub _webSocketHub;

        public WebSocketMiddleware(
            RequestDelegate next,
            IWebSocketHub webSocketHub)
        {
            _next = next;
            _webSocketHub = webSocketHub;
        }

        public async Task InvokeAsync(HttpContext context, IPayloadService _messageService)
        {
            if (!context.WebSockets.IsWebSocketRequest)
                return;

            using var socket = await context.WebSockets.AcceptWebSocketAsync();
            var userId = context.Request.Query["userId"];
            try
            {
                await _webSocketHub.OnConnectedAsync(socket, userId);

                await Receive(socket, async (result, buffer) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var data = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var payload = WebSocketPayload.Deseriazlie(data);
                        if (payload == null) throw new ApplicationException("Unable to deseriazlie payload");
                        
                        var methodInfo = _messageService.GetMethod(
                            payload.ActionName,
                            payload.Data.ToArray(),
                            _webSocketHub.GetType());
                        if (methodInfo == null) throw new ApplicationException($"Unable to determine {nameof(IWebSocketHub)} method");

                        _webSocketHub.SetContext(socket);
                        methodInfo.MethodInfo.Invoke(_webSocketHub, methodInfo.Parameters);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _webSocketHub.OnDisconnectedAsync(
                            socket, 
                            "Connection closed", 
                            WebSocketCloseStatus.NormalClosure);
                    }
                });
            }
            catch (Exception ex)
            {
                await _webSocketHub.DisconnectAsync(
                    socket, 
                    "An unexpected error has occurred", 
                    WebSocketCloseStatus.InternalServerError);
            }
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                handleMessage(result, buffer);
            }
        }
    }
}