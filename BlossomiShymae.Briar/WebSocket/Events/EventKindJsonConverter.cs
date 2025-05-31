using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlossomiShymae.Briar.WebSocket.Events
{
    internal class EventKindJsonConverter : JsonConverter<EventKind>
    {
        public override EventKind? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String) throw new JsonException();

            var values = reader.GetString()!.Split("_");

            return new EventKind()
            {
                Prefix = values[0],
                Path = values.Length > 1 ? string.Join("_", values.Skip(1)).Replace("_", "/") : null
            };
        }

        public override void Write(Utf8JsonWriter writer, EventKind value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }
    }
}