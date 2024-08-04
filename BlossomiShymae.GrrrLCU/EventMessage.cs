using System.Text.Json;
using System.Text.Json.Nodes;

namespace BlossomiShymae.GrrrLCU
{
    /// <summary>
    /// Messages that are sent via the League Client websocket.
    /// </summary>
    public class EventMessage(RequestType requestType, string kind, EventData? data = null)
    {
        private static JsonSerializerOptions s_options =  new()
        { 
            PropertyNameCaseInsensitive = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

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

        /// <summary>
        /// Get the raw JSON string of the event message.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Data == null) return $"[{(int)RequestType}, \"{Kind}\"]";
            return $"[{(int)RequestType}, \"{Kind}\", {JsonSerializer.Serialize(Data, s_options)}]";
        }

        internal EventMessage(string json) : this(new(), string.Empty)
        {
            var array = JsonNode.Parse(json)!
                                .AsArray()!;
                                
            RequestType = JsonSerializer.Deserialize<RequestType>(array[0], s_options)!;
            Kind = array[1]!.GetValue<string>()!;
            
            var obj = array[2]!.AsObject();
            var data = obj["data"]?.ToJsonString(s_options) ?? null;
            var eventType = obj["eventType"]!.GetValue<string>();
            var uri = obj["uri"]!.GetValue<string>();
            Data = new EventData(data, eventType, uri);
        }
    }
}