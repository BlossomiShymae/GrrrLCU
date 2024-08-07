using System.Text.Json.Nodes;

namespace BlossomiShymae.GrrrLCU
{
    /// <summary>
    /// Represents the data of an League Client websocket message.
    /// </summary>
    public class EventData(JsonNode? data, string eventType, string uri)
    {
        /// <summary>
        /// The payload of the data as a JsonNode.
        /// </summary>
        public JsonNode? Data { get; } = data;
        /// <summary>
        /// The type of the event. E.g. "Update".
        /// </summary>
        public string EventType { get; } = eventType;
        /// <summary>
        /// The path from where the event was transmitted.
        /// </summary>
        public string Uri { get; } = uri;
    }
}