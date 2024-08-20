using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlossomiShymae.GrrrLCU
{
    internal class EventMessageConverter : JsonConverter<EventMessage>
    {
        public override EventMessage? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

            RequestType? requestType = null;
            string? kind = null;
            EventData? eventData = null;

            while (reader.Read())
            {
                if ((reader.TokenType == JsonTokenType.EndArray) && (requestType != null) && (kind != null))
                {
                    return new EventMessage((RequestType)requestType, kind, eventData);
                }

                if (reader.TokenType == JsonTokenType.Number)
                {
                    requestType = JsonSerializer.Deserialize<RequestType>(ref reader, Connector.JsonSerializerOptions);
                }

                if (reader.TokenType == JsonTokenType.String)
                {
                    kind = reader.GetString();
                }

                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    eventData = JsonSerializer.Deserialize<EventData>(ref reader, Connector.JsonSerializerOptions);
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, EventMessage value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            JsonSerializer.Serialize(writer, value.RequestType);
            writer.WriteStringValue(value.Kind);
            JsonSerializer.Serialize(writer, value.Data);
            
            writer.WriteEndArray();
        }
    }
}