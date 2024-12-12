using System.Text.Json.Serialization;
using System.Text.Json;
using CitizensFinancialGroup.Threvw.Tenants.Service.Http.Features.Settings.Models.Permissions;
using CitizensFinancialGroup.Threvw.Tenants.Service.Http.Features.Settings.Models;

namespace CitizensFinancialGroup.Threvw.Tenants.Service.Http.TBD {
    public class OptionsGroupModelJsonConverter : JsonConverter<Features.Settings.Models.OptionsGroup> {
        public override OptionsGroup Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            // Parse the JSON object
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader)) {
                JsonElement root = doc.RootElement;

                // Determine the type based on the discriminator
                if (root.TryGetProperty("$type", out JsonElement typeProperty)) {
                    string typeDiscriminator = typeProperty.GetString();
                    return typeDiscriminator switch {
                        "permissions" => JsonSerializer.Deserialize<PermissionOptionsGroup>(root.GetRawText(), options),
                        _ => throw new JsonException($"Unknown type discriminator: {typeDiscriminator}")
                    };
                }

                throw new JsonException("Missing type discriminator");
            }
        }

        public override void Write(Utf8JsonWriter writer, OptionsGroup value, JsonSerializerOptions options) {
            // Add the type discriminator during serialization
            writer.WriteStartObject();

            if (value is PermissionOptionsGroup a) {
                writer.WriteString("$type", "permissions");
                JsonSerializer.Serialize(writer, a, typeof(PermissionOptionsGroup), options);
            }
            else {
                throw new JsonException($"Unknown type: {value.GetType()}");
            }

            writer.WriteEndObject();
        }
    }
}
