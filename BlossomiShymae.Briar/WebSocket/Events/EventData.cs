using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace BlossomiShymae.Briar.WebSocket.Events
{
    /// <summary>
    /// Represents the data of an League Client websocket message.
    /// </summary>
    public class EventData(JsonNode? data, string eventType, string uri)
    {
        /// <summary>
        /// The payload of the data as a JsonNode.
        /// </summary>
        [JsonPropertyName("data")]
        public JsonNode? Data { get; } = data;
        /// <summary>
        /// The type of the event. E.g. "Update".
        /// </summary>
        [JsonPropertyName("eventType")]
        public string EventType { get; } = eventType;
        /// <summary>
        /// The path from where the event was transmitted.
        /// </summary>
        [JsonPropertyName("uri")]
        public string Uri { get; } = uri;
    }
}