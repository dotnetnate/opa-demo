using AutoMapper;
using NOCO.Threvw.Policies.Domain;
using NOCO.Threvw.Policy.Service.Http.Features.Policies.Controllers;
using NOCO.Threvw.Policy.Service.Http.Features.Policies.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using DomainPolicyDecision = NOCO.Threvw.Policies.Domain.PolicyDecision;

namespace Policy.Service.Http.Tests.Unit.Features.Policies.Controllers {
    [TestClass]
    public class PolicyDecisionControllerTestFixture {
        private Mock<IPolicyEvaluationService> _mockEvaluationService = null!;
        private Mock<ILogger<PolicyDecisionController>> _mockLogger = null!;
        private Mock<IMapper> _mockMapper = null!;
        private PolicyDecisionController _controller = null!;

        [TestInitialize]
        public void Setup() {
            _mockEvaluationService = new Mock<IPolicyEvaluationService>();
            _mockLogger = new Mock<ILogger<PolicyDecisionController>>();
            _mockMapper = new Mock<IMapper>();
            _controller = new PolicyDecisionController(
                _mockEvaluationService.Object,
                _mockLogger.Object,
                _mockMapper.Object);
        }

        [TestMethod]
        public async Task EvaluatePolicy_NullRequest_ReturnsBadRequest() {
            // Act
            var result = await _controller.EvaluatePolicy(null!);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequest = result as BadRequestObjectResult;
            Assert.IsTrue(badRequest!.Value!.ToString()!.Contains("Request body is required"));
        }

        [TestMethod]
        public async Task EvaluatePolicy_NullResource_ReturnsBadRequest() {
            // Arrange
            var request = new EvaluatePolicyRequest {
                Resource = null!,
                Subject = new SubjectModel { Identifier = "user-1", Authority = "auth0" },
                Action = new ActionContextModel { Id = "READ" }
            };

            // Act
            var result = await _controller.EvaluatePolicy(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task EvaluatePolicy_NullSubject_ReturnsBadRequest() {
            // Arrange
            var request = new EvaluatePolicyRequest {
                Resource = new ResourceModel { Identifier = "resource-1", Authority = "test" },
                Subject = null!,
                Action = new ActionContextModel { Id = "READ" }
            };

            // Act
            var result = await _controller.EvaluatePolicy(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task EvaluatePolicy_NullAction_ReturnsBadRequest() {
            // Arrange
            var request = new EvaluatePolicyRequest {
                Resource = new ResourceModel { Identifier = "resource-1", Authority = "test" },
                Subject = new SubjectModel { Identifier = "user-1", Authority = "auth0" },
                Action = null!
            };

            // Act
            var result = await _controller.EvaluatePolicy(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task EvaluatePolicy_EmptyActionId_ReturnsBadRequest() {
            // Arrange
            var request = new EvaluatePolicyRequest {
                Resource = new ResourceModel { Identifier = "resource-1", Authority = "test" },
                Subject = new SubjectModel { Identifier = "user-1", Authority = "auth0" },
                Action = new ActionContextModel { Id = "" }
            };

            // Act
            var result = await _controller.EvaluatePolicy(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task EvaluatePolicy_ValidRequest_ReturnsOkWithDecision() {
            // Arrange
            var request = new EvaluatePolicyRequest {
                Resource = new ResourceModel { Identifier = "resource-1", Authority = "test" },
                Subject = new SubjectModel { Identifier = "user-1", Authority = "auth0" },
                Action = new ActionContextModel {
                    Id = "READ",
                    Context = new Dictionary<string, object> { { "amount", 1000 } }
                }
            };

            var domainResource = new Resource { Identifier = "resource-1", Authority = "test" };
            var domainSubject = new Subject { Identifier = "user-1", Authority = "auth0" };
            var evaluationResult = new PolicyEvaluationResult {
                Decision = DomainPolicyDecision.Permit,
                Obligations = new List<Obligation> {
                    new Obligation { Type = "logging" }
                },
                Advice = new List<Advice> {
                    new Advice { Type = "monitor" }
                },
                EvaluationDetails = "First applicable rule matched"
            };

            _mockMapper.Setup(m => m.Map<Resource>(request.Resource)).Returns(domainResource);
            _mockMapper.Setup(m => m.Map<Subject>(request.Subject)).Returns(domainSubject);
            _mockMapper.Setup(m => m.Map<List<ObligationModel>>(evaluationResult.Obligations))
                .Returns(new List<ObligationModel> {
                    new ObligationModel { Type = "logging" }
                });
            _mockMapper.Setup(m => m.Map<List<AdviceModel>>(evaluationResult.Advice))
                .Returns(new List<AdviceModel> {
                    new AdviceModel { Type = "monitor" }
                });

            _mockEvaluationService
                .Setup(s => s.EvaluatePolicy(
                    domainResource,
                    domainSubject,
                    "READ",
                    request.Action.Context))
                .ReturnsAsync(evaluationResult);

            // Act
            var result = await _controller.EvaluatePolicy(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as EvaluatePolicyResponse;
            Assert.IsNotNull(response);
            Assert.AreEqual(NOCO.Threvw.Policy.Service.Http.Features.Policies.Models.PolicyDecision.Permit, response.Decision);
            Assert.AreEqual(1, response.Obligations.Count);
            Assert.AreEqual("logging", response.Obligations[0].Type);
            Assert.AreEqual(1, response.Advice.Count);
            Assert.AreEqual("monitor", response.Advice[0].Type);
            Assert.AreEqual("First applicable rule matched", response.EvaluationDetails);
        }

        [TestMethod]
        public async Task EvaluatePolicy_DenyDecision_ReturnsOkWithDeny() {
            // Arrange
            var request = new EvaluatePolicyRequest {
                Resource = new ResourceModel { Identifier = "resource-1", Authority = "test" },
                Subject = new SubjectModel { Identifier = "user-1", Authority = "auth0" },
                Action = new ActionContextModel { Id = "WRITE" }
            };

            var domainResource = new Resource { Identifier = "resource-1", Authority = "test" };
            var domainSubject = new Subject { Identifier = "user-1", Authority = "auth0" };
            var evaluationResult = new PolicyEvaluationResult {
                Decision = DomainPolicyDecision.Deny,
                Obligations = new List<Obligation> {
                    new Obligation { Type = "notification" }
                }
            };

            _mockMapper.Setup(m => m.Map<Resource>(request.Resource)).Returns(domainResource);
            _mockMapper.Setup(m => m.Map<Subject>(request.Subject)).Returns(domainSubject);
            _mockMapper.Setup(m => m.Map<List<ObligationModel>>(evaluationResult.Obligations))
                .Returns(new List<ObligationModel> {
                    new ObligationModel { Type = "notification" }
                });
            _mockMapper.Setup(m => m.Map<List<AdviceModel>>(evaluationResult.Advice))
                .Returns(new List<AdviceModel>());

            _mockEvaluationService
                .Setup(s => s.EvaluatePolicy(
                    domainResource,
                    domainSubject,
                    "WRITE",
                    It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(evaluationResult);

            // Act
            var result = await _controller.EvaluatePolicy(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as EvaluatePolicyResponse;
            Assert.IsNotNull(response);
            Assert.AreEqual(NOCO.Threvw.Policy.Service.Http.Features.Policies.Models.PolicyDecision.Deny, response.Decision);
        }

        [TestMethod]
        public async Task EvaluatePolicy_ServiceThrowsException_Returns500() {
            // Arrange
            var request = new EvaluatePolicyRequest {
                Resource = new ResourceModel { Identifier = "resource-1", Authority = "test" },
                Subject = new SubjectModel { Identifier = "user-1", Authority = "auth0" },
                Action = new ActionContextModel { Id = "READ" }
            };

            var domainResource = new Resource { Identifier = "resource-1", Authority = "test" };
            var domainSubject = new Subject { Identifier = "user-1", Authority = "auth0" };

            _mockMapper.Setup(m => m.Map<Resource>(request.Resource)).Returns(domainResource);
            _mockMapper.Setup(m => m.Map<Subject>(request.Subject)).Returns(domainSubject);

            _mockEvaluationService
                .Setup(s => s.EvaluatePolicy(
                    It.IsAny<Resource>(),
                    It.IsAny<Subject>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>()))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var result = await _controller.EvaluatePolicy(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult!.StatusCode);
        }

        [TestMethod]
        public async Task EvaluatePolicy_WithContextAttributes_PassesContextToService() {
            // Arrange
            var context = new Dictionary<string, object> {
                { "amount", 50000 },
                { "status", "active" },
                { "tier", "premium" }
            };

            var request = new EvaluatePolicyRequest {
                Resource = new ResourceModel { Identifier = "resource-1", Authority = "test" },
                Subject = new SubjectModel { Identifier = "user-1", Authority = "auth0" },
                Action = new ActionContextModel {
                    Id = "WIRE_TRANSFER",
                    Context = context
                }
            };

            var domainResource = new Resource { Identifier = "resource-1", Authority = "test" };
            var domainSubject = new Subject { Identifier = "user-1", Authority = "auth0" };
            var evaluationResult = new PolicyEvaluationResult {
                Decision = DomainPolicyDecision.Permit
            };

            _mockMapper.Setup(m => m.Map<Resource>(request.Resource)).Returns(domainResource);
            _mockMapper.Setup(m => m.Map<Subject>(request.Subject)).Returns(domainSubject);
            _mockMapper.Setup(m => m.Map<List<ObligationModel>>(It.IsAny<List<Obligation>>()))
                .Returns(new List<ObligationModel>());
            _mockMapper.Setup(m => m.Map<List<AdviceModel>>(It.IsAny<List<Advice>>()))
                .Returns(new List<AdviceModel>());

            _mockEvaluationService
                .Setup(s => s.EvaluatePolicy(
                    domainResource,
                    domainSubject,
                    "WIRE_TRANSFER",
                    context))
                .ReturnsAsync(evaluationResult);

            // Act
            var result = await _controller.EvaluatePolicy(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _mockEvaluationService.Verify(
                s => s.EvaluatePolicy(
                    domainResource,
                    domainSubject,
                    "WIRE_TRANSFER",
                    context),
                Times.Once);
        }

        [TestMethod]
        public void HealthCheck_ReturnsOk() {
            // Act
            var result = _controller.HealthCheck();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult!.Value);
        }
    }
}
