using NOCO.Threvw.Policies.Domain;
using Microsoft.Extensions.Logging;
using Moq;
using DomainPolicy = NOCO.Threvw.Policies.Domain.Policy;

namespace Policy.Domain.Tests.Unit {
    [TestClass]
    public class PolicyEvaluationServiceTestFixture {
        private Mock<IPolicyRepository> _mockRepository = null!;
        private Mock<IPrivilegeEvaluator> _mockPrivilegeEvaluator = null!;
        private Mock<ILogger<PolicyEvaluationService>> _mockLogger = null!;
        private PolicyEvaluationService _service = null!;

        [TestInitialize]
        public void Setup() {
            _mockRepository = new Mock<IPolicyRepository>();
            _mockPrivilegeEvaluator = new Mock<IPrivilegeEvaluator>();
            _mockLogger = new Mock<ILogger<PolicyEvaluationService>>();
            _service = new PolicyEvaluationService(
                _mockRepository.Object,
                _mockPrivilegeEvaluator.Object,
                _mockLogger.Object);
        }

        [TestMethod]
        public async Task EvaluatePolicy_PolicyNotFound_ReturnsNotApplicable() {
            // Arrange
            var resource = new Resource { Identifier = "test-resource", Authority = "test" };
            var subject = new Subject { Identifier = "user-1", Authority = "auth0" };
            var context = new Dictionary<string, object>();

            _mockRepository
                .Setup(r => r.FindPolicyByResource(It.IsAny<FindPolicyByResourceQuery>()))
                .ReturnsAsync((DomainPolicy?)null);

            // Act
            var result = await _service.EvaluatePolicy(resource, subject, "READ", context);

            // Assert
            Assert.AreEqual(PolicyDecision.NotApplicable, result.Decision);
            Assert.IsTrue(result.EvaluationDetails!.Contains("No policy found"));
        }

        [TestMethod]
        public async Task EvaluatePolicy_NoMatchingRule_ReturnsDeny() {
            // Arrange
            var resource = new Resource { Identifier = "test-resource", Authority = "test" };
            var subject = new Subject { Identifier = "user-1", Authority = "auth0" };
            var context = new Dictionary<string, object>();
            var policy = new DomainPolicy {
                Resource = resource,
                Rules = new List<Rule> {
                    new Rule {
                        Subject = new Subject { Identifier = "different-user", Authority = "auth0" },
                        Privileges = new List<Privilege>()
                    }
                }
            };

            _mockRepository
                .Setup(r => r.FindPolicyByResource(It.IsAny<FindPolicyByResourceQuery>()))
                .ReturnsAsync(policy);

            // Act
            var result = await _service.EvaluatePolicy(resource, subject, "READ", context);

            // Assert
            Assert.AreEqual(PolicyDecision.Deny, result.Decision);
            Assert.IsTrue(result.EvaluationDetails!.Contains("No rule found for subject"));
        }

        [TestMethod]
        public async Task EvaluatePolicy_NoMatchingPrivilege_ReturnsDeny() {
            // Arrange
            var resource = new Resource { Identifier = "test-resource", Authority = "test" };
            var subject = new Subject { Identifier = "user-1", Authority = "auth0" };
            var context = new Dictionary<string, object>();
            var policy = new DomainPolicy {
                Resource = resource,
                Rules = new List<Rule> {
                    new Rule {
                        Subject = subject,
                        Privileges = new List<Privilege> {
                            new Privilege { PermissionName = "WRITE" }
                        }
                    }
                }
            };

            _mockRepository
                .Setup(r => r.FindPolicyByResource(It.IsAny<FindPolicyByResourceQuery>()))
                .ReturnsAsync(policy);

            // Act
            var result = await _service.EvaluatePolicy(resource, subject, "READ", context);

            // Assert
            Assert.AreEqual(PolicyDecision.Deny, result.Decision);
            Assert.IsTrue(result.EvaluationDetails!.Contains("No privilege defined for action"));
        }

        [TestMethod]
        public async Task EvaluatePolicy_ValidMatch_DelegatesToPrivilegeEvaluator() {
            // Arrange
            var resource = new Resource { Identifier = "test-resource", Authority = "test" };
            var subject = new Subject { Identifier = "user-1", Authority = "auth0" };
            var context = new Dictionary<string, object> { { "amount", 1000 } };
            var privilege = new Privilege {
                PermissionName = "READ",
                EffectRules = new List<EffectRule>(),
                DefaultEffect = PermissionActions.Permit
            };
            var policy = new DomainPolicy {
                Resource = resource,
                Rules = new List<Rule> {
                    new Rule {
                        Subject = subject,
                        Privileges = new List<Privilege> { privilege }
                    }
                }
            };
            var expectedResult = new PolicyEvaluationResult {
                Decision = PolicyDecision.Permit,
                Obligations = new List<Obligation> { new Obligation { Type = "logging" } }
            };

            _mockRepository
                .Setup(r => r.FindPolicyByResource(It.IsAny<FindPolicyByResourceQuery>()))
                .ReturnsAsync(policy);

            _mockPrivilegeEvaluator
                .Setup(e => e.EvaluatePrivilege(privilege, context))
                .Returns(expectedResult);

            // Act
            var result = await _service.EvaluatePolicy(resource, subject, "READ", context);

            // Assert
            Assert.AreEqual(PolicyDecision.Permit, result.Decision);
            Assert.AreEqual(1, result.Obligations.Count);
            Assert.AreEqual("logging", result.Obligations[0].Type);
            _mockPrivilegeEvaluator.Verify(
                e => e.EvaluatePrivilege(privilege, context),
                Times.Once);
        }

        [TestMethod]
        public async Task EvaluatePolicy_CaseInsensitiveActionMatch_ReturnsResult() {
            // Arrange
            var resource = new Resource { Identifier = "test-resource", Authority = "test" };
            var subject = new Subject { Identifier = "user-1", Authority = "auth0" };
            var context = new Dictionary<string, object>();
            var privilege = new Privilege {
                PermissionName = "WIRE_TRANSFER",
                EffectRules = new List<EffectRule>(),
                DefaultEffect = PermissionActions.Permit
            };
            var policy = new DomainPolicy {
                Resource = resource,
                Rules = new List<Rule> {
                    new Rule {
                        Subject = subject,
                        Privileges = new List<Privilege> { privilege }
                    }
                }
            };
            var expectedResult = new PolicyEvaluationResult {
                Decision = PolicyDecision.Permit
            };

            _mockRepository
                .Setup(r => r.FindPolicyByResource(It.IsAny<FindPolicyByResourceQuery>()))
                .ReturnsAsync(policy);

            _mockPrivilegeEvaluator
                .Setup(e => e.EvaluatePrivilege(privilege, context))
                .Returns(expectedResult);

            // Act - lowercase action
            var result = await _service.EvaluatePolicy(resource, subject, "wire_transfer", context);

            // Assert
            Assert.AreEqual(PolicyDecision.Permit, result.Decision);
            _mockPrivilegeEvaluator.Verify(
                e => e.EvaluatePrivilege(privilege, context),
                Times.Once);
        }

        [TestMethod]
        public async Task EvaluatePolicy_ExceptionThrown_ReturnsIndeterminate() {
            // Arrange
            var resource = new Resource { Identifier = "test-resource", Authority = "test" };
            var subject = new Subject { Identifier = "user-1", Authority = "auth0" };
            var context = new Dictionary<string, object>();

            _mockRepository
                .Setup(r => r.FindPolicyByResource(It.IsAny<FindPolicyByResourceQuery>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.EvaluatePolicy(resource, subject, "READ", context);

            // Assert
            Assert.AreEqual(PolicyDecision.Indeterminate, result.Decision);
            Assert.IsTrue(result.EvaluationDetails!.Contains("Error during evaluation"));
        }

        [TestMethod]
        public async Task EvaluatePolicy_MultiplePrivileges_ReturnsCorrectOne() {
            // Arrange
            var resource = new Resource { Identifier = "test-resource", Authority = "test" };
            var subject = new Subject { Identifier = "user-1", Authority = "auth0" };
            var context = new Dictionary<string, object>();
            var readPrivilege = new Privilege { PermissionName = "READ" };
            var writePrivilege = new Privilege { PermissionName = "WRITE" };
            var policy = new DomainPolicy {
                Resource = resource,
                Rules = new List<Rule> {
                    new Rule {
                        Subject = subject,
                        Privileges = new List<Privilege> { readPrivilege, writePrivilege }
                    }
                }
            };
            var expectedResult = new PolicyEvaluationResult {
                Decision = PolicyDecision.Permit
            };

            _mockRepository
                .Setup(r => r.FindPolicyByResource(It.IsAny<FindPolicyByResourceQuery>()))
                .ReturnsAsync(policy);

            _mockPrivilegeEvaluator
                .Setup(e => e.EvaluatePrivilege(writePrivilege, context))
                .Returns(expectedResult);

            // Act
            var result = await _service.EvaluatePolicy(resource, subject, "WRITE", context);

            // Assert
            Assert.AreEqual(PolicyDecision.Permit, result.Decision);
            _mockPrivilegeEvaluator.Verify(
                e => e.EvaluatePrivilege(writePrivilege, context),
                Times.Once);
            _mockPrivilegeEvaluator.Verify(
                e => e.EvaluatePrivilege(readPrivilege, context),
                Times.Never);
        }
    }
}
