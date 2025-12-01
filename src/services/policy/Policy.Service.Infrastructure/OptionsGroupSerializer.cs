using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using NOCO.Threvw.Tenants.Domain;
using NOCO.Threvw.Tenants.Domain.Settings;

namespace NOCO.Threvw.Tenants.Infrastructure {
    public class OptionsGroupEntityJsonConverter : JsonConverter<OptionsGroup> {
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
