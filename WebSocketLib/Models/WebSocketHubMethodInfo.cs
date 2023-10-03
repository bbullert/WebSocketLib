using System.Reflection;

namespace WebSocketLib.Models
{
    public class WebSocketHubMethodInfo
    {
        public MethodInfo MethodInfo { get; set; }
        public dynamic[] Parameters { get; set; }
    }
}
