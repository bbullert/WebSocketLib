using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebSocketLib.Models
{
    public class WebSocketPayload
    {
        [JsonPropertyName("actionName")]
        public string ActionName { get; set; }

        [JsonPropertyName("data")]
        public IEnumerable<string> Data { get; set; }

        public static string Seriazlie(dynamic[] data)
        {
            var model = new WebSocketPayload();
            if (data.Length > 0) model.ActionName = data[0];
            if (data.Length > 1) model.Data = data.Skip(1).Cast<string>().ToList();

            return JsonSerializer.Serialize(model);
        }

        public static WebSocketPayload? Deseriazlie(string data)
        {
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };
            return JsonSerializer.Deserialize<WebSocketPayload>(data, options);
        }
    }
}