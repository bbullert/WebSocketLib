using WebSocketLib.Models;

namespace WebSocketLib.Services
{
    public interface IPayloadService
    {
        WebSocketHubMethodInfo? GetMethod(string methodName, string[] parameters, Type type);
    }
}