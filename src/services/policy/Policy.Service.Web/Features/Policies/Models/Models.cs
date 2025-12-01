using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text.Json;
using NOCO.Threvw.Policy.Service.Http.Features.Shared.Models;
using NOCO.Elements;


namespace NOCO.Threvw.Policy.Service.Http.Features.Policies.Models {
    #region Requests

    public class FindPoliciesRequest {
        public string? ResourceId { get; set; } // Optional filter by resource ID
        public string? ResourceAuthority { get; set; } // Optional filter by resource type
        public string? SubjectId { get; set; } // Optional filter by subject ID
        public int PageNumber { get; set; } = 1; // Pagination page number
        public int PageSize { get; set; } = 10; // Pagination page size
    }

    public class FindPolicyByResourceRequest {
        public required string Authority { get; set; }
        public required string ResourceId { get; set; } // Resource ID for the policy        
    }

    public class CreatePolicyRequest {
        public required ResourceModel Resource { get; set; } // Resource details for the policy
        public required List<RuleModel> Rules { get; set; } = new(); // List of rules for the policy
    }

    public class UpdatePolicyRequest {
        public List<RuleModel> Rules { get; set; } = new(); // Updated list of rules for the policy
        public string? ETag { get; set; } // ETag for optimistic concurrency control
    }

    public class AddOrUpdateRuleRequest {
        public required RuleModel Rule { get; set; } // Rule details to add or update
        public string? ETag { get; set; } // ETag for optimistic concurrency control
    }

    #endregion

    #region Models

    public class DeleteRuleRequest {
        public required string Id { get; set; } // The resource ID identifying the policy
        public required string Authority { get; set; } // The resource type identifying the policy
        public required string SubjectAuthority { get; set; } // The subject authority
        public required string SubjectId { get; set; } // The subject identifier
        public string? ETag { get; set; } // ETag for optimistic concurrency control
    }

    public class PolicyModel {
        public required ResourceModel Resource { get; set; } // Resource associated with the policy
        public List<RuleModel> Rules { get; set; } = new(); // Rules within the policy
        public string? ETag { get; set; } // ETag for optimistic concurrency control
    }

    public class ResourceModel : ScopedIdentity {        
    }

    public class ScopedIdentity {
        /// <summary>
        /// The unique identifier for the identity.
        /// </summary>
        public required string Identifier { get; set; }
        /// <summary>
        /// The owning scope of the identity or the authority under which the identity is unique.
        /// </summary>
        public required string Authority { get; set; }

        public override bool Equals(object? obj) {
            return obj is ScopedIdentity identity &&
                   Identifier == identity.Identifier &&
                   Authority == identity.Authority;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Identifier, Authority);
        }

        public static bool operator ==(ScopedIdentity left, ScopedIdentity right) {
            return left.Equals(right);
        }
        public static bool operator !=(ScopedIdentity left, ScopedIdentity right) {
            return !left.Equals(right);
        }
    }

    public class RuleModel {
        public required SubjectModel Subject { get; set; } // Subject details for the rule
        public List<PrivilegeModel> Privileges { get; set; } = new(); // Privileges granted by the rule
    }

    public class SubjectModel : ScopedIdentity {        
    }

    public enum PermissionActions {
        [Description("deny")]
        Deny = 0,
        [Description("permit")]
        Permit = 1
    }

    public class PrivilegeModel {
        public required string PermissionName { get; set; } // Name of the privilege (e.g., "READ", "WRITE")
        public CombiningAlgorithm CombiningAlgorithm { get; set; } = CombiningAlgorithm.FirstApplicable; // How to combine multiple effect rules
        public List<EffectRuleModel> EffectRules { get; set; } = new(); // Effect rules with conditions, obligations, and advice
        public PermissionActions DefaultEffect { get; set; } = PermissionActions.Deny; // Default effect if no rules match
    }

    public enum CombiningAlgorithm {
        [Description("firstApplicable")]
        FirstApplicable = 0,
        [Description("denyOverrides")]
        DenyOverrides = 1,
        [Description("permitOverrides")]
        PermitOverrides = 2,
        [Description("onlyOneApplicable")]
        OnlyOneApplicable = 3
    }

    public class EffectRuleModel {
        public PermissionActions Effect { get; set; } // Permit or Deny
        public List<ConditionModel> Conditions { get; set; } = new(); // Conditions for this rule
        public Range<DateTimeOffset>? ValidityPeriod { get; set; } // Temporal validity
        public List<ObligationModel> Obligations { get; set; } = new(); // Obligations that must be fulfilled
        public List<AdviceModel> Advice { get; set; } = new(); // Advisory information
    }

    public class ObligationModel {
        public required string Type { get; set; } // Obligation type (e.g., "approval", "mfa", "logging")
        public Dictionary<string, object> Parameters { get; set; } = new(); // Type-specific parameters
    }

    public class AdviceModel {
        public required string Type { get; set; } // Advice type (e.g., "audit", "monitor")
        public Dictionary<string, object> Parameters { get; set; } = new(); // Type-specific parameters
    }

    /// <summary>
    /// Represents the operations that can be performed during evaluation of a condition on a rule.    
    /// </summary>
    public enum Operators {
        [Description("gt")]
        gt,
        [Description("lt")]
        lt,
        [Description("eq")]
        eq,
        [Description("neq")]
        neq,
        [Description("gte")]
        gte,
        [Description("lte")]
        lte
    }
    public class ConditionModel {
        public required string ContextAttributePath { get; set; } // Attribute name (e.g., "amount")
        public required Operators Operator { get; set; } // Operator (e.g., "eq", "gt")
        public required object Value { get; set; } // Value to compare against
    }



    #endregion

    #region Error Handling

    public class ErrorResult {
        public InternalValidationResult? ValidationResult { get; set; } // Validation result details
        public IEnumerable<string>? Errors { get; set; } = new List<string>(); // List of error messages
    }


    #endregion




public class ConditionModelJsonConverter : JsonConverter<ConditionModel> {
        public override ConditionModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {            

            string? attributeValue = string.Empty;
            Operators operatorValue = Operators.eq;
            object? valueValue = null;            

            while (reader.Read()) {
                if (reader.TokenType == JsonTokenType.EndObject) {

                    if(attributeValue == null || valueValue == null) {
                        throw new JsonException("Invalid JSON for ConditionModel");
                    }

                    return new ConditionModel {
                        ContextAttributePath = attributeValue,                        
                        Operator = operatorValue, 
                        Value = valueValue,
                                               
                    };                    
                }

                if (reader.TokenType == JsonTokenType.PropertyName) {
                    string? propertyName = reader.GetString()?.ToLower();
                    reader.Read();

                    switch (propertyName) {
                        case "contextattributepath":
                            attributeValue = reader.GetString();
                            break;
                        case "operator":
                            operatorValue = JsonSerializer.Deserialize<Operators>(ref reader, options);
                            break;
                        case "value":
                            valueValue = ReadDynamicValue(ref reader);
                            break;
                        
                    }
                }
            }
            throw new JsonException("Invalid JSON for ConditionModel");
        }

        private object? ReadDynamicValue(ref Utf8JsonReader reader) {
            switch (reader.TokenType) {
                case JsonTokenType.String:
                    if (reader.TryGetDateTime(out DateTime dateTime)) {
                        return dateTime;
                    }
                    if (reader.TryGetGuid(out Guid guid)) {
                        return guid;
                    }
                    return reader.GetString();
                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out long longValue)) {
                        return longValue;
                    }
                    return reader.GetDouble();
                case JsonTokenType.True:
                case JsonTokenType.False:
                    return reader.GetBoolean();
                default:
                    throw new JsonException("Unsupported JSON token type for dynamic value");
            }
        }

        public override void Write(Utf8JsonWriter writer, ConditionModel value, JsonSerializerOptions options) {
            writer.WriteStartObject();
            writer.WriteString(JsonNamingPolicy.CamelCase.ConvertName(nameof(ConditionModel.ContextAttributePath)), value.ContextAttributePath );
            writer.WriteString(JsonNamingPolicy.CamelCase.ConvertName(nameof(ConditionModel.Operator)), value.Operator.ToString());            
            writer.WritePropertyName(JsonNamingPolicy.CamelCase.ConvertName(nameof(ConditionModel.Value)));
            JsonSerializer.Serialize(writer, value.Value, options);
            writer.WriteEndObject();
        }
    }

}


