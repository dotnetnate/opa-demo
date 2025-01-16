using AutoMapper;
using CitizensFinancialGroup.Elements.ApplicationModel.Commands;
using CitizensFinancialGroup.Elements.ApplicationModel.Queries;
using CitizensFinancialGroup.Threvw.Policies.Domain;
using CitizensFinancialGroup.Threvw.Policy.Service.Http.Features.Policies.Controllers;
using CitizensFinancialGroup.Threvw.Policy.Service.Http.Features.Policies.Models;
using CitizensFinancialGroup.Threvw.Policy.Service.Http.Features.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Testing.Platform.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace zzz.Service.Http.Tests.Unit.Features.Policies.Controllers {
    [TestClass]
    public class PolicyControllerTestFixture {

        [TestMethod]
        public async Task FindPolicies_When_Called_With_Valid_Request_Should_Return_Ok_Result() {
            // Arrange
            var mockPolicyService = new Mock<IPolicyService>();
            var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<PolicyController>>();
            var mockMapper = new Mock<IMapper>();

            var request = new FindPoliciesRequest();
            var query = new FindPoliciesQuery();
            var policies = new List<Policy>();
            var queryResult = QueryResult<IEnumerable<Policy>>.SuccessResult(policies);
            var policyModels = new List<PolicyModel>();

            mockMapper.Setup(m => m.Map<FindPoliciesQuery>(request)).Returns(query);
            mockPolicyService.Setup(s => s.FindPolicies(query)).ReturnsAsync(queryResult);
            mockMapper.Setup(m => m.Map<IEnumerable<PolicyModel>>(policies)).Returns(policyModels);

            var controller = new PolicyController(mockPolicyService.Object, mockLogger.Object, mockMapper.Object);

            // Act
            var result = await controller.FindPolicies(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual(policyModels, okResult?.Value);
        }

        [TestMethod]
        public async Task GetPolicyByResource_When_Called_With_Valid_Request_Should_Return_Ok_Result() {
            // Arrange
            var mockPolicyService = new Mock<IPolicyService>();
            var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<PolicyController>>();
            var mockMapper = new Mock<IMapper>();

            var request = new FindPolicyByResourceRequest { Authority = "authority", ResourceId = "resourceId" };
            var query = new FindPolicyByResourceQuery { Resource = new Resource { Authority = "authority", Identifier = "resourceId" } };
            var policy = new Policy { Resource = new Resource { Authority = "authority", Identifier = "resourceId" }, Rules = new List<Rule>() };
            var queryResult = QueryResult<Policy>.SuccessResult(policy);
            var policyModel = new PolicyModel { Resource = new ResourceModel { Authority = "authority", Identifier = "resourceId" } };

            mockMapper.Setup(m => m.Map<FindPolicyByResourceQuery>(request)).Returns(query);
            mockPolicyService.Setup(s => s.GetPolicyByResource(query)).ReturnsAsync(queryResult);
            mockMapper.Setup(m => m.Map<PolicyModel>(policy)).Returns(policyModel);

            var controller = new PolicyController(mockPolicyService.Object, mockLogger.Object, mockMapper.Object);

            // Act
            var result = await controller.GetPolicyForResource(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual(policyModel, okResult?.Value);
        }

        [TestMethod]
        public async Task CreatePolicy_When_Called_With_Valid_Request_Should_Return_CreatedAtAction_Result() {
            // Arrange
            var mockPolicyService = new Mock<IPolicyService>();
            var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<PolicyController>>();
            var mockMapper = new Mock<IMapper>();

            var model = new CreatePolicyRequest { Resource = new ResourceModel { Authority = "authority", Identifier = "resourceId" }, Rules = new List<RuleModel>() };
            var command = new CreatePolicyCommand { Resource = new Resource { Authority = "authority", Identifier = "resourceId" } };
            var policy = new Policy { Resource = new Resource { Authority = "authority", Identifier = "resourceId" }, Rules = new List<Rule>() };
            var commandResult = CommandResult<Policy>.SuccessResult(policy);
            var policyModel = new PolicyModel { Resource = new ResourceModel { Authority = "authority", Identifier = "resourceId" } };

            mockMapper.Setup(m => m.Map<CreatePolicyCommand>(model)).Returns(command);
            mockPolicyService.Setup(s => s.CreatePolicy(command)).ReturnsAsync(commandResult);
            mockMapper.Setup(m => m.Map<PolicyModel>(policy)).Returns(policyModel);

            var controller = new PolicyController(mockPolicyService.Object, mockLogger.Object, mockMapper.Object);

            // Act
            var result = await controller.CreatePolicy(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            var createdResult = result as CreatedAtActionResult;
            Assert.AreEqual(policyModel, createdResult?.Value);
        }

        [TestMethod]
        public async Task UpdatePolicy_When_Called_With_Valid_Request_Should_Return_Ok_Result() {
            // Arrange
            var mockPolicyService = new Mock<IPolicyService>();
            var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<PolicyController>>();
            var mockMapper = new Mock<IMapper>();

            var resourceId = "resourceId";
            var authority = "authority";
            var model = new UpdatePolicyRequest { Rules = new List<RuleModel>() };
            var command = new UpdatePolicyCommand { Resource = new Resource { Identifier = resourceId, Authority = authority } };
            var policy = new Policy { Resource = new Resource { Authority = "authority", Identifier = "resourceId" }, Rules = new List<Rule>() };
            var commandResult = CommandResult<Policy>.SuccessResult(policy);
            var policyModel = new PolicyModel { Resource = new ResourceModel { Authority = "authority", Identifier = "resourceId" } };

            mockMapper.Setup(m => m.Map<UpdatePolicyCommand>(model)).Returns(command);
            mockPolicyService.Setup(s => s.UpdatePolicy(command)).ReturnsAsync(commandResult);
            mockMapper.Setup(m => m.Map<PolicyModel>(policy)).Returns(policyModel);

            var controller = new PolicyController(mockPolicyService.Object, mockLogger.Object, mockMapper.Object);

            // Act
            var result = await controller.UpdatePolicy(resourceId, authority, model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual(policyModel, okResult?.Value);
        }

        [TestMethod]
        public async Task DeletePolicy_When_Called_With_Valid_Request_Should_Return_NoContent_Result() {
            // Arrange
            var mockPolicyService = new Mock<IPolicyService>();
            var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<PolicyController>>();
            var mockMapper = new Mock<IMapper>();

            var authority = "authority";
            var resourceId = "resourceId";
            var command = new DeletePolicyCommand { Resource = new Resource { Identifier = resourceId, Authority = authority } };
            var commandResult = CommandResult.SuccessResult();

            mockPolicyService.Setup(s => s.DeletePolicy(It.IsAny<DeletePolicyCommand>())).ReturnsAsync(commandResult);

            var controller = new PolicyController(mockPolicyService.Object, mockLogger.Object, mockMapper.Object);

            // Act
            var result = await controller.DeletePolicyForResource(authority, resourceId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task AddOrUpdateRule_When_Called_With_Valid_Request_Should_Return_NoContent_Result() {
            // Arrange
            var mockPolicyService = new Mock<IPolicyService>();
            var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<PolicyController>>();
            var mockMapper = new Mock<IMapper>();

            var authority = "authority";
            var resourceId = "resourceId";
            var model = new AddOrUpdateRuleRequest { Rule = new RuleModel { Subject = new SubjectModel { Authority = "abc", Identifier = "def" } } };
            var command = new AddOrUpdateRuleCommand { Resource = new Resource { Identifier = resourceId, Authority = authority }, Rule = new Rule { Subject = new Subject { Authority = "abc", Identifier = "def" } } };
            var commandResult = CommandResult.SuccessResult();

            mockMapper.Setup(m => m.Map<AddOrUpdateRuleCommand>(model)).Returns(command);
            mockPolicyService.Setup(s => s.AddOrUpdateRule(command)).ReturnsAsync(commandResult);

            var controller = new PolicyController(mockPolicyService.Object, mockLogger.Object, mockMapper.Object);

            // Act
            var result = await controller.AddOrUpdateRule(authority, resourceId, model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task DeleteRule_When_Called_With_Valid_Request_Should_Return_NoContent_Result() {
            // Arrange
            var mockPolicyService = new Mock<IPolicyService>();
            var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<PolicyController>>();
            var mockMapper = new Mock<IMapper>();

            var authority = "authority";
            var resourceId = "resourceId";
            var subjectId = "subjectId";
            var command = new DeleteRuleCommand { Resource = new Resource { Identifier = resourceId, Authority = authority }, SubjectId = subjectId };
            var commandResult = CommandResult.SuccessResult();

            mockPolicyService.Setup(s => s.DeleteRule(It.IsAny<DeleteRuleCommand>())).ReturnsAsync(commandResult);

            var controller = new PolicyController(mockPolicyService.Object, mockLogger.Object, mockMapper.Object);

            // Act
            var result = await controller.DeleteRule(authority, resourceId, subjectId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
        [TestMethod]
        public async Task FindPolicies_When_Called_With_Invalid_Request_Should_Return_BadRequest_Result() {
            // Arrange
            var mockPolicyService = new Mock<IPolicyService>();
            var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<PolicyController>>();
            var mockMapper = new Mock<IMapper>();

            var request = new FindPoliciesRequest();
            var query = new FindPoliciesQuery();
            var queryResult = QueryResult<IEnumerable<Policy>>.FailureResult(executionException: new Exception("this is an error."), validationResult: new CitizensFinancialGroup.Elements.Validation.InternalValidationResult {
                Errors = new List<CitizensFinancialGroup.Elements.Validation.InternalValidationError> { new CitizensFinancialGroup.Elements.Validation.InternalValidationError { PropertyName = "resource", ErrorMessage = "Resource not found" } }
            });

            mockMapper.Setup(m => m.Map<FindPoliciesQuery>(request)).Returns(query);
            mockPolicyService.Setup(s => s.FindPolicies(query)).ReturnsAsync(queryResult);

            var controller = new PolicyController(mockPolicyService.Object, mockLogger.Object, mockMapper.Object);

            // Act
            var result = await controller.FindPolicies(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetPolicyByResource_When_Called_With_Invalid_Request_Should_Return_NotFound_Result() {
            // Arrange
            var mockPolicyService = new Mock<IPolicyService>();
            var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<PolicyController>>();
            var mockMapper = new Mock<IMapper>();

            var request = new FindPolicyByResourceRequest { Authority = "authority", ResourceId = "resourceId" };
            var query = new FindPolicyByResourceQuery { Resource = new Resource { Authority = "authority", Identifier = "resourceId" } };
            var queryResult = QueryResult<Policy>.FailureResult(failureCategory: QueryFailureCategory.ResourceNotFound);

            mockMapper.Setup(m => m.Map<FindPolicyByResourceQuery>(request)).Returns(query);
            mockPolicyService.Setup(s => s.GetPolicyByResource(query)).ReturnsAsync(queryResult);

            var controller = new PolicyController(mockPolicyService.Object, mockLogger.Object, mockMapper.Object);

            // Act
            var result = await controller.GetPolicyForResource(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task CreatePolicy_When_Called_With_Invalid_Request_Should_Return_BadRequest_Result() {
            // Arrange
            // Arrange
            var mockPolicyService = new Mock<IPolicyService>();
            var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<PolicyController>>();
            var mockMapper = new Mock<IMapper>();

            var model = new CreatePolicyRequest { Resource = new ResourceModel { Authority = "authority", Identifier = "resourceId" }, Rules = new List<RuleModel>() };
            var command = new CreatePolicyCommand { Resource = new Resource { Authority = "authority", Identifier = "resourceId" } };
            var policy = new Policy { Resource = new Resource { Authority = "authority", Identifier = "resourceId" }, Rules = new List<Rule>() };
            var commandResult = CommandResult.FailureResult<Policy>((Policy)null,  failureCategory: CommandFailureCategory.ParameterValidation, validationResult: new CitizensFinancialGroup.Elements.Validation.InternalValidationResult {
                Errors = new List<CitizensFinancialGroup.Elements.Validation.InternalValidationError> { new CitizensFinancialGroup.Elements.Validation.InternalValidationError { PropertyName = "resource", ErrorMessage = "Resource not found" } }
            });
            var policyModel = new PolicyModel { Resource = new ResourceModel { Authority = "authority", Identifier = "resourceId" } };

            mockMapper.Setup(m => m.Map<CreatePolicyCommand>(model)).Returns(command);
            mockPolicyService.Setup(s => s.CreatePolicy(command)).ReturnsAsync(commandResult);
            mockMapper.Setup(m => m.Map<PolicyModel>(policy)).Returns(policyModel);

            var controller = new PolicyController(mockPolicyService.Object, mockLogger.Object, mockMapper.Object);

            // Act
            var result = await controller.CreatePolicy(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));            
        }

        [TestMethod]
        public async Task UpdatePolicy_When_Called_With_Invalid_Request_Should_Return_BadRequest_Result() {
            // Arrange
            var mockPolicyService = new Mock<IPolicyService>();
            var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<PolicyController>>();
            var mockMapper = new Mock<IMapper>();

            var resourceId = "resourceId";
            var authority = "authority";
            var model = new UpdatePolicyRequest { Rules = new List<RuleModel>() };
            var command = new UpdatePolicyCommand { Resource = new Resource { Identifier = resourceId, Authority = authority } };
            var commandResult = CommandResult<Policy>.FailureResult((Policy?)null, validationResult: new CitizensFinancialGroup.Elements.Validation.InternalValidationResult { Errors = new List<CitizensFinancialGroup.Elements.Validation.InternalValidationError> { new CitizensFinancialGroup.Elements.Validation.InternalValidationError { PropertyName = "resource", ErrorMessage = "Resource not found" } } });

            mockMapper.Setup(m => m.Map<UpdatePolicyCommand>(model)).Returns(command);
            mockPolicyService.Setup(s => s.UpdatePolicy(command)).ReturnsAsync(commandResult);

            var controller = new PolicyController(mockPolicyService.Object, mockLogger.Object, mockMapper.Object);

            // Act
            var result = await controller.UpdatePolicy(resourceId, authority, model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task DeletePolicy_When_Called_With_Invalid_Request_Should_Return_BadRequest_Result() {
            // Arrange
            var mockPolicyService = new Mock<IPolicyService>();
            var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<PolicyController>>();
            var mockMapper = new Mock<IMapper>();

            var authority = "authority";
            var resourceId = "resourceId";
            var command = new DeletePolicyCommand { Resource = new Resource { Identifier = resourceId, Authority = authority } };
            var commandResult = CommandResult.FailureResult(failureCategory: CommandFailureCategory.ParameterValidation, validationResult: new CitizensFinancialGroup.Elements.Validation.InternalValidationResult {
                Errors = new List<CitizensFinancialGroup.Elements.Validation.InternalValidationError> { new CitizensFinancialGroup.Elements.Validation.InternalValidationError { PropertyName = "resource", ErrorMessage = "Resource not found" } }
            });

            mockPolicyService.Setup(s => s.DeletePolicy(It.IsAny<DeletePolicyCommand>())).ReturnsAsync(commandResult);

            var controller = new PolicyController(mockPolicyService.Object, mockLogger.Object, mockMapper.Object);

            // Act
            var result = await controller.DeletePolicyForResource(null, null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task AddOrUpdateRule_When_Called_With_Invalid_Request_Should_Return_BadRequest_Result() {
            // Arrange
            var mockPolicyService = new Mock<IPolicyService>();
            var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<PolicyController>>();
            var mockMapper = new Mock<IMapper>();

            var authority = "authority";
            var resourceId = "resourceId";
            var model = new AddOrUpdateRuleRequest { Rule = new RuleModel { Subject = new SubjectModel { Authority = "abc", Identifier = "def" } } };
            var command = new AddOrUpdateRuleCommand { Resource = new Resource { Identifier = resourceId, Authority = authority }, Rule = new Rule { Subject = new Subject { Authority = "abc", Identifier = "def" } } };
            var commandResult = CommandResult.FailureResult("Error", validationResult: new CitizensFinancialGroup.Elements.Validation.InternalValidationResult { Errors = new List<CitizensFinancialGroup.Elements.Validation.InternalValidationError> { new CitizensFinancialGroup.Elements.Validation.InternalValidationError { PropertyName = "resource", ErrorMessage = "Resource not found" } } });

            mockMapper.Setup(m => m.Map<AddOrUpdateRuleCommand>(model)).Returns(command);
            mockPolicyService.Setup(s => s.AddOrUpdateRule(command)).ReturnsAsync(commandResult);

            var controller = new PolicyController(mockPolicyService.Object, mockLogger.Object, mockMapper.Object);

            // Act
            var result = await controller.AddOrUpdateRule(authority, resourceId, model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task DeleteRule_When_Called_With_Invalid_Request_Should_Return_BadRequest_Result() {
            // Arrange
            var mockPolicyService = new Mock<IPolicyService>();
            var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<PolicyController>>();
            var mockMapper = new Mock<IMapper>();

            var authority = "authority";
            var resourceId = "resourceId";
            var subjectId = "subjectId";
            var command = new DeleteRuleCommand { Resource = new Resource { Identifier = resourceId, Authority = authority }, SubjectId = subjectId };
            var commandResult = CommandResult.FailureResult("Error", validationResult: new CitizensFinancialGroup.Elements.Validation.InternalValidationResult {
                Errors = new List<CitizensFinancialGroup.Elements.Validation.InternalValidationError> { new CitizensFinancialGroup.Elements.Validation.InternalValidationError { PropertyName = "resource", ErrorMessage = "Resource not found" } }
            });

            
            mockPolicyService.Setup(s => s.DeleteRule(It.IsAny<DeleteRuleCommand>())).ReturnsAsync(commandResult);

            var controller = new PolicyController(mockPolicyService.Object, mockLogger.Object, mockMapper.Object);

            // Act
            var result = await controller.DeleteRule(authority, resourceId, subjectId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

    }
}
