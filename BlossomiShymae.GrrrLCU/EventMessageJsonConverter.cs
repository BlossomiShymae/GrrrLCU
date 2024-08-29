using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlossomiShymae.GrrrLCU
{
    internal class EventMessageJsonConverter : JsonConverter<EventMessage>
    {
        internal static JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

        public override EventMessage? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

            EventRequestType? requestType = null;
            EventKind? kind = null;
            EventData? eventData = null;

            while (reader.Read())
            {
                if ((reader.TokenType == JsonTokenType.EndArray) && (requestType != null) && (kind != null))
                {
                    return new EventMessage((EventRequestType)requestType, kind, eventData);
                }

                if (reader.TokenType == JsonTokenType.Number)
                {
                    requestType = JsonSerializer.Deserialize<EventRequestType>(ref reader, JsonSerializerOptions);
                }

                if (reader.TokenType == JsonTokenType.String)
                {
                    kind = JsonSerializer.Deserialize<EventKind>(ref reader, JsonSerializerOptions);
                }

                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    eventData = JsonSerializer.Deserialize<EventData>(ref reader, JsonSerializerOptions);
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, EventMessage value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            JsonSerializer.Serialize(writer, value.RequestType);
            JsonSerializer.Serialize(writer, value.Kind);
            JsonSerializer.Serialize(writer, value.Data);
            
            writer.WriteEndArray();
        }
    }
}