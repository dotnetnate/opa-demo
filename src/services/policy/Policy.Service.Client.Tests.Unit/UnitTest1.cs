using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.TestHelper;

#region PolicyBuilderTests

[TestClass]
public class PolicyBuilderTests {
    private readonly PolicyValidator _policyValidator;

    public PolicyBuilderTests() {
        var policyDefinition = new PolicyDefinition {
            PermissionDefinitions = new List<PermissionDefinition>
            {
                new PermissionDefinition
                {
                    Name = "PERMISSION_1",
                    AllowedConditions = new List<AllowedCondition>
                    {
                        new AllowedCondition { Name = "amount", DataType = "number" },
                        new AllowedCondition { Name = "status", DataType = "string" }
                    }
                }
            }
        };

        PolicyBuilder.LoadPolicyDefinitions();
        _policyValidator = new PolicyValidator();
    }

    [TestMethod]
    public void Given_ValidPolicy_When_BuildCalled_Then_ShouldPassValidation() {
        var resource = new Resource { ResourceId = "1", ResourceType = "application" };
        var subject = new Subject { Identifier = "user1", Type = "user", Authority = "https://auth0.com" };
        var privilege = new Privilege {
            Name = "PERMISSION_1",
            Conditions = new List<Condition>
            {
                new Condition { Attribute = "amount", Operator = ConditionOperator.Gt, Value = 100 }
            }
        };

        var builder = new PolicyBuilder(resource, _policyValidator)
            .AddPrivilegeToRule(subject, privilege);

        var policy = builder.Build();

        Assert.IsNotNull(policy);
        Assert.AreEqual(1, policy.Rules.Count);
        Assert.AreEqual(1, policy.Rules.First().Privileges.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ValidationException))]
    public void Given_InvalidPolicy_When_BuildCalled_Then_ShouldThrowValidationException() {
        var resource = new Resource { ResourceId = "1", ResourceType = "application" };
        var builder = new PolicyBuilder(resource, _policyValidator);
        builder.Build();
    }

    [TestMethod]
    public void Given_RuleWithInvalidPrivilegeName_When_BuildCalled_Then_ShouldThrowValidationException() {
        var resource = new Resource { ResourceId = "1", ResourceType = "application" };
        var subject = new Subject { Identifier = "user1", Type = "user", Authority = "https://auth0.com" };
        var privilege = new Privilege {
            Name = "INVALID_PERMISSION",
            Conditions = new List<Condition>
            {
                new Condition { Attribute = "amount", Operator = ConditionOperator.Gt, Value = 100 }
            }
        };

        var builder = new PolicyBuilder(resource, _policyValidator)
            .AddPrivilegeToRule(subject, privilege);

        Assert.ThrowsException<ValidationException>(() => builder.Build());
    }

    [TestMethod]
    public void Given_ConditionWithInvalidAttribute_When_BuildCalled_Then_ShouldThrowValidationException() {
        var resource = new Resource { ResourceId = "1", ResourceType = "application" };
        var subject = new Subject { Identifier = "user1", Type = "user", Authority = "https://auth0.com" };
        var privilege = new Privilege {
            Name = "PERMISSION_1",
            Conditions = new List<Condition>
            {
                new Condition { Attribute = "invalid", Operator = ConditionOperator.Gt, Value = 100 }
            }
        };

        var builder = new PolicyBuilder(resource, _policyValidator)
            .AddPrivilegeToRule(subject, privilege);

        Assert.ThrowsException<ValidationException>(() => builder.Build());
    }

    [TestMethod]
    public void Given_ConditionWithInvalidDataType_When_BuildCalled_Then_ShouldThrowValidationException() {
        var resource = new Resource { ResourceId = "1", ResourceType = "application" };
        var subject = new Subject { Identifier = "user1", Type = "user", Authority = "https://auth0.com" };
        var privilege = new Privilege {
            Name = "PERMISSION_1",
            Conditions = new List<Condition>
            {
                new Condition { Attribute = "amount", Operator = ConditionOperator.Gt, Value = "invalid_value" }
            }
        };

        var builder = new PolicyBuilder(resource, _policyValidator)
            .AddPrivilegeToRule(subject, privilege);

        Assert.ThrowsException<ValidationException>(() => builder.Build());
    }
}

#endregion

#region PolicyClientTests

[TestClass]
public class PolicyClientTests {
    private HttpClient GetMockHttpClient(HttpStatusCode statusCode, object responseContent) {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = statusCode,
                Content = new StringContent(JsonSerializer.Serialize(responseContent)),
            });

        return new HttpClient(handlerMock.Object);
    }

    [TestMethod]
    public async Task Given_ValidResponse_When_FindPoliciesCalled_Then_ShouldReturnPolicies() {
        var mockPolicies = new List<Policy>
        {
            new Policy
            {
                Resource = new Resource { ResourceId = "1", ResourceType = "application" },
                Rules = new List<Rule>
                {
                    new Rule
                    {
                        Subject = new Subject { Identifier = "user1", Type = "user", Authority = "https://auth0.com" },
                        Privileges = new List<Privilege>
                        {
                            new Privilege { Name = "PERMISSION_1" }
                        }
                    }
                }
            }
        };

        var client = new PolicyClient(GetMockHttpClient(HttpStatusCode.OK, mockPolicies));
        var result = await client.FindPolicies(new FindPoliciesQuery());

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
    }

    [TestMethod]
    [ExpectedException(typeof(HttpRequestException))]
    public async Task Given_ErrorResponse_When_FindPoliciesCalled_Then_ShouldThrowException() {
        var client = new PolicyClient(GetMockHttpClient(HttpStatusCode.BadRequest, null));
        await client.FindPolicies(new FindPoliciesQuery());
    }

    [TestMethod]
    public async Task Given_ValidPolicy_When_CreatePolicyCalled_Then_ShouldSucceed() {
        var resource = new Resource { ResourceId = "1", ResourceType = "application" };
        var policy = new Policy {
            Resource = resource,
            Rules = new List<Rule>
            {
                new Rule
                {
                    Subject = new Subject { Identifier = "user1", Type = "user", Authority = "https://auth0.com" },
                    Privileges = new List<Privilege>
                    {
                        new Privilege { Name = "PERMISSION_1" }
                    }
                }
            }
        };

        var client = new PolicyClient(GetMockHttpClient(HttpStatusCode.Created, null));
        await client.CreatePolicy(policy);
    }

    [TestMethod]
    [ExpectedException(typeof(HttpRequestException))]
    public async Task Given_ErrorResponse_When_CreatePolicyCalled_Then_ShouldThrowException() {
        var client = new PolicyClient(GetMockHttpClient(HttpStatusCode.BadRequest, null));
        await client.CreatePolicy(new Policy());
    }
}

#endregion
