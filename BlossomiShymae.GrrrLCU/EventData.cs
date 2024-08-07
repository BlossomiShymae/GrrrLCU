namespace BlossomiShymae.GrrrLCU
{
    /// <summary>
    /// Represents the data of an League Client websocket message.
    /// </summary>
    public class EventData(Dictionary<string, object>? data, string eventType, string uri)
    {
        /// <summary>
        /// The payload of the data as an weak-typed dictionary.
        /// </summary>
        public Dictionary<string, object>? Data { get; } = data;
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