using System.Text.Json.Serialization;

namespace BlossomiShymae.GrrrLCU
{
    /// <summary>
    /// Messages that are sent via the League Client websocket.
    /// </summary>
    [JsonConverter(typeof(EventMessageConverter))]
    public class EventMessage(RequestType requestType, string kind, EventData? data = null)
    {
        /// <summary>
        /// Documented kinds available for messages.
        /// </summary>
        public static class Kinds
        {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public const string OnJsonApiEvent = "OnJsonApiEvent";
            public const string OnLcdsEvent = "OnLcdsEvent";
            public const string OnLog = "OnLog";
            public const string OnRegionLocaleChanged = "OnRegionLocaleChanged";
            public const string OnServiceProxyAsyncEvent = "OnServiceProxyAsyncEvent";
            public const string OnServiceProxyMethodEvent = "OnServiceProxyMethodEvent";
            public const string OnServiceProxyUuidEvent = "OnServiceProxyUuidEvent";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        }

        /// <summary>
        /// The operation type of the request message.
        /// </summary>
        public RequestType RequestType { get; } = requestType;
        /// <summary>
        /// The kind of the event message.
        /// </summary>
        public string Kind { get; } = kind;
        /// <summary>
        /// The data contained in the event message.
        /// </summary>
        public EventData? Data { get; } = data;
    }
}