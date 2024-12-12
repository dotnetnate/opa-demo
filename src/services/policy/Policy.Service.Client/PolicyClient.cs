using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;

#region Models

public class Policy {
    public Resource Resource { get; set; }
    public List<Rule> Rules { get; set; } = new();
}

public class Resource {
    public string ResourceId { get; set; }
    public string ResourceType { get; set; }
}

public class Rule {
    public Subject Subject { get; set; }
    public List<Privilege> Privileges { get; set; } = new();
}

public class Subject {
    public string Identifier { get; set; }
    public string Type { get; set; }
    public string Authority { get; set; }
}

public class Privilege {
    public string Name { get; set; }
    public List<Condition> Conditions { get; set; } = new();
}

public class Condition {
    public string Attribute { get; set; }
    public ConditionOperator Operator { get; set; }
    public object Value { get; set; }
}

public enum ConditionOperator {
    Eq,
    Neq,
    Gt,
    Gte,
    Lt,
    Lte
}

public class PolicyDefinition {
    public bool EnforceConfiguredPermissions { get; set; }
    public List<PermissionDefinition> PermissionDefinitions { get; set; } = new();
    public string Type { get; set; }
}

public class PermissionDefinition {
    public string Name { get; set; }
    public Visibility Visibility { get; set; }
    public List<AppliesTo> AppliesTo { get; set; } = new();
    public List<AllowedCondition> AllowedConditions { get; set; } = new();
}

public class Visibility {
    public bool Enabled { get; set; }
    public DateTime? ActivationDate { get; set; }
    public DateTime? DeactivationDate { get; set; }
}

public class AppliesTo {
    public List<string> ResourceType { get; set; } = new();
}

public class AllowedCondition {
    public string Name { get; set; }
    public string DataType { get; set; }
    public bool Required { get; set; }
}

#endregion

#region PolicyBuilder

public class PolicyBuilder {
    private readonly Policy _policy;
    private static PolicyDefinition _policyDefinition;
    private readonly IValidator<Policy> _policyValidator;

    static PolicyBuilder() {
        LoadPolicyDefinitions();
    }

    public PolicyBuilder(Resource resource, IValidator<Policy> policyValidator) {
        if (string.IsNullOrEmpty(resource.ResourceId) || string.IsNullOrEmpty(resource.ResourceType))
            throw new ArgumentException("ResourceId and ResourceType must be provided.");

        _policy = new Policy {
            Resource = resource
        };

        _policyValidator = policyValidator ?? throw new ArgumentNullException(nameof(policyValidator));
    }

    public PolicyBuilder AddRule(Rule rule) {
        _policy.Rules.Add(rule);
        return this;
    }

    public PolicyBuilder AddPrivilegeToRule(Subject subject, Privilege privilege) {
        var rule = _policy.Rules.FirstOrDefault(r => r.Subject.Identifier == subject.Identifier);
        if (rule == null) {
            rule = new Rule {
                Subject = subject
            };
            _policy.Rules.Add(rule);
        }

        rule.Privileges.Add(privilege);
        return this;
    }

    public PolicyBuilder RemoveRule(string subjectId) {
        _policy.Rules.RemoveAll(r => r.Subject.Identifier == subjectId);
        return this;
    }

    public Policy Build() {
        var validationResult = _policyValidator.Validate(_policy);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        return _policy;
    }

    public static void LoadPolicyDefinitions() {
        var json = File.ReadAllText("policydef.json");
        _policyDefinition = JsonSerializer.Deserialize<PolicyDefinition>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        if (_policyDefinition == null)
            throw new InvalidOperationException("Failed to load policy definitions.");
    }

    public static PolicyDefinition GetPolicyDefinition() => _policyDefinition;
}

#endregion

#region Validators

public class PolicyValidator : AbstractValidator<Policy> {
    public PolicyValidator() {
        var policyDefinition = PolicyBuilder.GetPolicyDefinition();

        RuleFor(policy => policy.Resource)
            .NotNull().WithMessage("Policy must have a resource.");

        RuleForEach(policy => policy.Rules)
            .SetValidator(new RuleValidator(policyDefinition));
    }
}

public class RuleValidator : AbstractValidator<Rule> {
    private readonly PolicyDefinition _policyDefinition;

    public RuleValidator(PolicyDefinition policyDefinition) {
        _policyDefinition = policyDefinition;

        RuleFor(rule => rule.Subject)
            .NotNull().WithMessage("Rule must have a subject.");

        RuleForEach(rule => rule.Privileges)
            .SetValidator(new PrivilegeValidator(policyDefinition));
    }
}

public class PrivilegeValidator : AbstractValidator<Privilege> {
    private readonly PolicyDefinition _policyDefinition;

    public PrivilegeValidator(PolicyDefinition policyDefinition) {
        _policyDefinition = policyDefinition;

        RuleFor(privilege => privilege.Name)
            .NotEmpty().WithMessage("Privilege must have a name.")
            .Must(name => _policyDefinition.PermissionDefinitions.Any(p => p.Name == name))
            .WithMessage(name => $"Privilege '{name}' is not defined in the policy definition.");

        RuleForEach(privilege => privilege.Conditions)
            .SetValidator(new ConditionValidator(policyDefinition));
    }
}

public class ConditionValidator : AbstractValidator<Condition> {
    private readonly PolicyDefinition _policyDefinition;

    public ConditionValidator(PolicyDefinition policyDefinition) {
        _policyDefinition = policyDefinition;

        RuleFor(condition => condition.Attribute)
            .NotEmpty().WithMessage("Condition must have an attribute.")
            .Must(attr => _policyDefinition.PermissionDefinitions
                .SelectMany(p => p.AllowedConditions)
                .Any(ac => ac.Name == attr))
            .WithMessage(attr => $"Condition attribute '{attr}' is not allowed.");

        RuleFor(condition => condition.Value)
            .Must((condition, value) => IsValidDataType(condition.Attribute, value))
            .WithMessage(condition => $"Condition '{condition.Attribute}' has an invalid value.");
    }

    private bool IsValidDataType(string attribute, object value) {
        var conditionDefinition = _policyDefinition.PermissionDefinitions
            .SelectMany(p => p.AllowedConditions)
            .FirstOrDefault(ac => ac.Name == attribute);

        if (conditionDefinition == null) return false;

        return conditionDefinition.DataType switch {
            "number" => value is int || value is double,
            "string" => value is string,
            "date" => value is DateTime,
            _ => false
        };
    }
}

#endregion

#region PolicyClient

public class PolicyClient {
    private readonly HttpClient _httpClient;

    public PolicyClient(HttpClient httpClient) {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<IEnumerable<Policy>> FindPolicies(FindPoliciesQuery query) {
        var response = await _httpClient.GetAsync($"/api/policies?{ToQueryString(query)}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<Policy>>();
    }

    public async Task<Policy> GetPolicyByResource(Resource resource) {
        var response = await _httpClient.GetAsync($"/api/policies/{resource.ResourceId}/{resource.ResourceType}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Policy>();
    }

    public async Task CreatePolicy(Policy policy) {
        var response = await _httpClient.PostAsJsonAsync("/api/policies", policy);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdatePolicy(Resource resource, Policy policy) {
        var response = await _httpClient.PutAsJsonAsync($"/api/policies/{resource.ResourceId}/{resource.ResourceType}", policy);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeletePolicy(Resource resource) {
        var response = await _httpClient.DeleteAsync($"/api/policies/{resource.ResourceId}/{resource.ResourceType}");
        response.EnsureSuccessStatusCode();
    }

    private static string ToQueryString(object obj) {
        var properties = from p in obj.GetType().GetProperties()
                         where p.GetValue(obj) != null
                         select $"{Uri.EscapeDataString(p.Name)}={Uri.EscapeDataString(p.GetValue(obj).ToString())}";
        return string.Join("&", properties);
    }
}

public class FindPoliciesQuery {
    public string ResourceId { get; set; } // Optional filter for resource ID
    public string ResourceType { get; set; } // Optional filter for resource type
    public string SubjectId { get; set; } // Optional filter for subject ID
    public int PageNumber { get; set; } = 1; // Pagination: page number
    public int PageSize { get; set; } = 10; // Pagination: page size
}

#endregion
