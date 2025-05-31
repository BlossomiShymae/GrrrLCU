using System.Text.Json.Serialization;

namespace BlossomiShymae.Briar.WebSocket.Events
{
    /// <summary>
    /// Messages that are sent via the League Client websocket.
    /// </summary>
    /// <param name="requestType"></param>
    /// <param name="kind"></param>
    /// <param name="data"></param>
    [JsonConverter(typeof(EventMessageJsonConverter))]
    public class EventMessage(EventRequestType requestType, EventKind kind, EventData? data = null)
    {

        /// <summary>
        /// The operation type of the request message.
        /// </summary>
        public EventRequestType RequestType { get; } = requestType;
        /// <summary>
        /// The kind of the event message.
        /// </summary>
        public EventKind Kind { get; } = kind;
        /// <summary>
        /// The data contained in the event message.
        /// </summary>
        public EventData? Data { get; } = data;
    }
}