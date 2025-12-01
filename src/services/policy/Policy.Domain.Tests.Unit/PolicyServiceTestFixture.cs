using NOCO.Elements.ApplicationModel.Commands;
using NOCO.Elements.ApplicationModel.Queries;
using NOCO.Elements.Security.Identity;
using NOCO.Elements.Validation;
using NOCO.Threvw.Policies.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NOCO.Threvw.Policies.Domain.Tests {

    [TestClass]
    public class PolicyServiceTestFixture {


        

        [TestMethod]
        public async Task Given_Valid_Query_When_Finding_Policies_Should_Return_Policies() {
            // Arrange
            var policyRepositoryMock = new Mock<IPolicyRepository>();
            var validationServiceMock = new Mock<IValidationService>();
            var identityServiceMock = new Mock<IIdentityService<ClaimsIdentity, object>>();
            var loggerMock = new Mock<ILogger<PolicyService<object>>>();
            var policyService = new PolicyService<object>(
                policyRepositoryMock.Object,
                validationServiceMock.Object,
                identityServiceMock.Object,
                loggerMock.Object
            );

            var query = new FindPoliciesQuery {  ResourceFilter = new Resource { Authority = "abc123", Identifier = "def123"}, PageNumber = 1, PageSize = 10 };
            var policies = new List<Policy> { new Policy { Resource = new Resource { Identifier = "1", Authority = "Authority",  ExtendedProperties = new Dictionary<string, string>() }, Rules = new List<Rule>() } };
            policyRepositoryMock.Setup(repo => repo.FindPolicies(query))
                .ReturnsAsync(policies);
            validationServiceMock.Setup(service => service.Validate(It.IsAny<object>()))
                .ReturnsAsync(new InternalValidationResult());

            // Act
            var result = await policyService.FindPolicies(query);

            // Assert
            Assert.IsTrue(result.IsSuccess());
            Assert.AreEqual(policies, result.Result);
        }

        [TestMethod]
        public async Task Given_Existing_Policy_When_Getting_Policy_By_Resource_Should_Return_Policy() {
            // Arrange
            var policyRepositoryMock = new Mock<IPolicyRepository>();
            var validationServiceMock = new Mock<IValidationService>();
            var identityServiceMock = new Mock<IIdentityService<ClaimsIdentity, object>>();
            var loggerMock = new Mock<ILogger<PolicyService<object>>>();
            var policyService = new PolicyService<object>(
                policyRepositoryMock.Object,
                validationServiceMock.Object,
                identityServiceMock.Object,
                loggerMock.Object
            );

            var query = new FindPolicyByResourceQuery { Resource = new Resource { Identifier = "1", Authority = "Authority",  ExtendedProperties = new Dictionary<string, string>() } };
            var policy = new Policy { Resource = query.Resource, Rules = new List<Rule>() };
            policyRepositoryMock.Setup(repo => repo.FindPolicyByResource(query))
                .ReturnsAsync(policy);
            validationServiceMock.Setup(service => service.Validate(It.IsAny<object>()))
                .ReturnsAsync(new InternalValidationResult());

            // Act
            var result = await policyService.GetPolicyByResource(query);

            // Assert
            Assert.IsTrue(result.IsSuccess());
            Assert.AreEqual(policy, result.Result);
        }

        [TestMethod]
        public async Task Given_Non_Existing_Policy_When_Getting_Policy_By_Resource_Should_Return_Not_Found() {
            // Arrange
            var policyRepositoryMock = new Mock<IPolicyRepository>();
            var validationServiceMock = new Mock<IValidationService>();
            var identityServiceMock = new Mock<IIdentityService<ClaimsIdentity, object>>();
            var loggerMock = new Mock<ILogger<PolicyService<object>>>();
            var policyService = new PolicyService<object>(
                policyRepositoryMock.Object,
                validationServiceMock.Object,
                identityServiceMock.Object,
                loggerMock.Object
            );

            var query = new FindPolicyByResourceQuery { Resource = new Resource { Identifier = "1", Authority = "Authority", ExtendedProperties = new Dictionary<string, string>() } };
            policyRepositoryMock.Setup(repo => repo.FindPolicyByResource(query))
                .ReturnsAsync((Policy?)null);
            validationServiceMock.Setup(service => service.Validate(It.IsAny<object>()))
                .ReturnsAsync(new InternalValidationResult());

            // Act
            var result = await policyService.GetPolicyByResource(query);

            // Assert
            Assert.IsFalse(result.IsSuccess());
            Assert.IsNull(result.Result);
        }

        [TestMethod]
        public async Task Given_Valid_Command_When_Creating_Policy_Should_Return_Success() {
            // Arrange
            var policyRepositoryMock = new Mock<IPolicyRepository>();
            var validationServiceMock = new Mock<IValidationService>();
            var identityServiceMock = new Mock<IIdentityService<ClaimsIdentity, object>>();
            var loggerMock = new Mock<ILogger<PolicyService<object>>>();
            var policyService = new PolicyService<object>(
                policyRepositoryMock.Object,
                validationServiceMock.Object,
                identityServiceMock.Object,
                loggerMock.Object
            );

            var command = new CreatePolicyCommand { Resource = new Resource { Identifier = "1", Authority = "Authority",  ExtendedProperties = new Dictionary<string, string>() }, Rules = new List<Rule>() };
            var policy = new Policy { Resource = command.Resource, Rules = command.Rules };
            policyRepositoryMock.Setup(repo => repo.CreatePolicy(policy))
                .Returns(Task.CompletedTask);
            validationServiceMock.Setup(service => service.Validate(It.IsAny<object>()))
                .ReturnsAsync(new InternalValidationResult());

            // Act
            var result = await policyService.CreatePolicy(command);

            // Assert
            Assert.IsTrue(result.IsSuccess());
            Assert.AreEqual(result.Status, CommandStatus.Succeeded);
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(policy.Resource.Identifier,  result.Result.Resource.Identifier);
        }

        [TestMethod]
        public async Task Given_Existing_Policy_When_Creating_Policy_Should_Return_Conflict() {
            // Arrange
            var policyRepositoryMock = new Mock<IPolicyRepository>();
            var validationServiceMock = new Mock<IValidationService>();
            var identityServiceMock = new Mock<IIdentityService<ClaimsIdentity, object>>();
            var loggerMock = new Mock<ILogger<PolicyService<object>>>();
            var policyService = new PolicyService<object>(
                policyRepositoryMock.Object,
                validationServiceMock.Object,
                identityServiceMock.Object,
                loggerMock.Object
            );

            var command = new CreatePolicyCommand { Resource = new Resource { Identifier = "1", Authority = "Authority",  ExtendedProperties = new Dictionary<string, string>() }, Rules = new List<Rule>() };
            var policy = new Policy { Resource = command.Resource, Rules = command.Rules };
            policyRepositoryMock.Setup(repo => repo.CreatePolicy(It.IsAny<Policy>()))
                .ThrowsAsync(new InvalidOperationException());
            validationServiceMock.Setup(service => service.Validate(It.IsAny<object>()))
                .ReturnsAsync(new InternalValidationResult());

            // Act
            var result = await policyService.CreatePolicy(command);

            // Assert
            Assert.IsFalse(result.IsSuccess());
            Assert.AreEqual(CommandFailureCategory.ResourceConflict, result.FailureCategory);
        }

        [TestMethod]
        public async Task Given_Valid_Command_When_Updating_Policy_Should_Return_Success() {
            // Arrange
            var policyRepositoryMock = new Mock<IPolicyRepository>();
            var validationServiceMock = new Mock<IValidationService>();
            var identityServiceMock = new Mock<IIdentityService<ClaimsIdentity, object>>();
            var loggerMock = new Mock<ILogger<PolicyService<object>>>();
            var policyService = new PolicyService<object>(
                policyRepositoryMock.Object,
                validationServiceMock.Object,
                identityServiceMock.Object,
                loggerMock.Object
            );

            var command = new UpdatePolicyCommand { Resource = new Resource { Identifier = "1", Authority = "Authority",  ExtendedProperties = new Dictionary<string, string>() }, Rules = new List<Rule>(), ETag = null };
            var policy = new Policy { Resource = command.Resource, Rules = command.Rules };
            policyRepositoryMock.Setup(repo => repo.UpdatePolicy(It.IsAny<Resource>(), It.IsAny<Policy>(), It.IsAny<string?>()))
                .ReturnsAsync(true);
            validationServiceMock.Setup(service => service.Validate(It.IsAny<object>()))
                .ReturnsAsync(new InternalValidationResult());

            // Act
            var result = await policyService.UpdatePolicy(command);

            // Assert
            // Assert
            Assert.IsTrue(result.IsSuccess());
            Assert.AreEqual(result.Status, CommandStatus.Succeeded);
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(policy.Resource.Identifier, result.Result.Resource.Identifier);
        }

        [TestMethod]
        public async Task Given_Update_Failure_When_Updating_Policy_Should_Return_Conflict() {
            // Arrange
            var policyRepositoryMock = new Mock<IPolicyRepository>();
            var validationServiceMock = new Mock<IValidationService>();
            var identityServiceMock = new Mock<IIdentityService<ClaimsIdentity, object>>();
            var loggerMock = new Mock<ILogger<PolicyService<object>>>();
            var policyService = new PolicyService<object>(
                policyRepositoryMock.Object,
                validationServiceMock.Object,
                identityServiceMock.Object,
                loggerMock.Object
            );

            var command = new UpdatePolicyCommand { Resource = new Resource { Identifier = "1", Authority = "Authority",  ExtendedProperties = new Dictionary<string, string>() }, Rules = new List<Rule>() };
            var policy = new Policy { Resource = command.Resource, Rules = command.Rules };
            policyRepositoryMock.Setup(repo => repo.UpdatePolicy(It.IsAny<Resource>(), It.IsAny<Policy>(), null))
                .ThrowsAsync(new InvalidOperationException());
            validationServiceMock.Setup(service => service.Validate(It.IsAny<object>()))
                .ReturnsAsync(new InternalValidationResult());

            // Act
            var result = await policyService.UpdatePolicy(command);

            // Assert
            Assert.IsFalse(result.IsSuccess());
            Assert.AreEqual(CommandFailureCategory.ResourceConflict, result.FailureCategory);
        }

        [TestMethod]
        public async Task Given_Valid_Command_When_Deleting_Policy_Should_Return_Success() {
            // Arrange
            var policyRepositoryMock = new Mock<IPolicyRepository>();
            var validationServiceMock = new Mock<IValidationService>();
            var identityServiceMock = new Mock<IIdentityService<ClaimsIdentity, object>>();
            var loggerMock = new Mock<ILogger<PolicyService<object>>>();
            var policyService = new PolicyService<object>(
                policyRepositoryMock.Object,
                validationServiceMock.Object,
                identityServiceMock.Object,
                loggerMock.Object
            );

            var command = new DeletePolicyCommand { Resource = new Resource { Identifier = "1", Authority = "Authority",  ExtendedProperties = new Dictionary<string, string>() } };
            policyRepositoryMock.Setup(repo => repo.DeletePolicy(command.Resource, null))
                .ReturnsAsync(true);
            validationServiceMock.Setup(service => service.Validate(It.IsAny<object>()))
                .ReturnsAsync(new InternalValidationResult());

            // Act
            var result = await policyService.DeletePolicy(command);

            // Assert
            Assert.IsTrue(result.IsSuccess());
        }

        [TestMethod]
        public async Task Given_Valid_Command_When_Adding_Or_Updating_Rule_Should_Return_Success() {
            // Arrange
            var policyRepositoryMock = new Mock<IPolicyRepository>();
            var validationServiceMock = new Mock<IValidationService>();
            var identityServiceMock = new Mock<IIdentityService<ClaimsIdentity, object>>();
            var loggerMock = new Mock<ILogger<PolicyService<object>>>();
            var policyService = new PolicyService<object>(
                policyRepositoryMock.Object,
                validationServiceMock.Object,
                identityServiceMock.Object,
                loggerMock.Object
            );

            var command = new AddOrUpdateRuleCommand { Resource = new Resource { Identifier = "1", Authority = "Authority",  ExtendedProperties = new Dictionary<string, string>() }, Rule = new Rule { Subject = new Subject { Identifier = "SubjectId", Authority = "Authority",  ExtendedProperties = new Dictionary<string, string>() } } };
            policyRepositoryMock.Setup(repo => repo.AddOrUpdateRule(command.Resource, command.Rule, null))
                .ReturnsAsync(true);
            validationServiceMock.Setup(service => service.Validate(It.IsAny<object>()))
                .ReturnsAsync(new InternalValidationResult());

            // Act
            var result = await policyService.AddOrUpdateRule(command);

            // Assert
            Assert.IsTrue(result.IsSuccess());
        }

        [TestMethod]
        public async Task Given_Valid_Command_When_Deleting_Rule_Should_Return_Success() {
            // Arrange
            var policyRepositoryMock = new Mock<IPolicyRepository>();
            var validationServiceMock = new Mock<IValidationService>();
            var identityServiceMock = new Mock<IIdentityService<ClaimsIdentity, object>>();
            var loggerMock = new Mock<ILogger<PolicyService<object>>>();
            var policyService = new PolicyService<object>(
                policyRepositoryMock.Object,
                validationServiceMock.Object,
                identityServiceMock.Object,
                loggerMock.Object
            );

            var subject = new Subject { Identifier = "subjectId", Authority = "Authority", ExtendedProperties = new Dictionary<string, string>() };
            var command = new DeleteRuleCommand { Resource = new Resource { Identifier = "1", Authority = "Authority",  ExtendedProperties = new Dictionary<string, string>() }, Subject = subject };
            policyRepositoryMock.Setup(repo => repo.DeleteRule(command.Resource, command.Subject, null))
                .ReturnsAsync(true);
            validationServiceMock.Setup(service => service.Validate(It.IsAny<object>()))
                .ReturnsAsync(new InternalValidationResult());

            // Act
            var result = await policyService.DeleteRule(command);

            // Assert
            Assert.IsTrue(result.IsSuccess());
        }
        [TestMethod]
        public async Task Given_Invalid_Command_When_Creating_Policy_Should_Return_Validation_Failure() {
            // Arrange
            var policyRepositoryMock = new Mock<IPolicyRepository>();
            var validationServiceMock = new Mock<IValidationService>();
            var identityServiceMock = new Mock<IIdentityService<ClaimsIdentity, object>>();
            var loggerMock = new Mock<ILogger<PolicyService<object>>>();
            var policyService = new PolicyService<object>(
                policyRepositoryMock.Object,
                validationServiceMock.Object,
                identityServiceMock.Object,
                loggerMock.Object
            );

            var command = new CreatePolicyCommand { Resource = new Resource { Identifier = "1", Authority = "Authority",  ExtendedProperties = new Dictionary<string, string>() }, Rules = new List<Rule>() };            
            validationServiceMock.Setup(service => service.Validate(It.IsAny<object>()))
                .ReturnsAsync(new InternalValidationResult {  Errors = new List<InternalValidationError> { new InternalValidationError {  ErrorCode = "ERR", ErrorMessage = "SOMEMESSAGE", PropertyName = "PROP.123"} } });

            // Act
            var result = await policyService.CreatePolicy(command);

            // Assert
            Assert.IsFalse(result.IsSuccess());
            Assert.AreEqual(CommandFailureCategory.ParameterValidation, result.FailureCategory);
            Assert.AreEqual(CommandStatus.Failed, result.Status);
        }
        [TestMethod]
        public async Task Given_Error_During_Processing_When_Creating_Policy_Should_Return_Failed_Status_With_Exception_Info() {
            // Arrange
            var policyRepositoryMock = new Mock<IPolicyRepository>();
            var validationServiceMock = new Mock<IValidationService>();
            var identityServiceMock = new Mock<IIdentityService<ClaimsIdentity, object>>();
            var loggerMock = new Mock<ILogger<PolicyService<object>>>();
            var policyService = new PolicyService<object>(
                policyRepositoryMock.Object,
                validationServiceMock.Object,
                identityServiceMock.Object,
                loggerMock.Object
            );

            var command = new CreatePolicyCommand { Resource = new Resource { Identifier = "1", Authority = "Authority",  ExtendedProperties = new Dictionary<string, string>() }, Rules = new List<Rule>() };
            var exception = new Exception("An error occurred during processing");
            policyRepositoryMock.Setup(repo => repo.CreatePolicy(It.IsAny<Policy>()))
                .Throws(exception);
            validationServiceMock.Setup(service => service.Validate(It.IsAny<object>()))
                .ReturnsAsync(new InternalValidationResult());

            // Act
            var result = await policyService.CreatePolicy(command);

            // Assert
            Assert.IsFalse(result.IsSuccess());
            Assert.AreEqual(CommandStatus.Failed, result.Status);
            Assert.AreEqual(exception.Message, result.ExecutionException?.Message);
        }
        [TestMethod]
        public async Task Given_Invalid_Query_When_Finding_Policies_Should_Return_Validation_Failure() {
            // Arrange
            var policyRepositoryMock = new Mock<IPolicyRepository>();
            var validationServiceMock = new Mock<IValidationService>();
            var identityServiceMock = new Mock<IIdentityService<ClaimsIdentity, object>>();
            var loggerMock = new Mock<ILogger<PolicyService<object>>>();
            var policyService = new PolicyService<object>(
                policyRepositoryMock.Object,
                validationServiceMock.Object,
                identityServiceMock.Object,
                loggerMock.Object
            );

            var query = new FindPoliciesQuery { ResourceFilter = new Resource { Authority = "abc", Identifier = "def"}, PageNumber = 1, PageSize = 10 };
            validationServiceMock.Setup(service => service.Validate(It.IsAny<object>()))
                .ReturnsAsync(new InternalValidationResult { Errors = new List<InternalValidationError> { new InternalValidationError { ErrorCode = "ERR", ErrorMessage = "SOMEMESSAGE", PropertyName = "PROP.123" } } });

            // Act
            var result = await policyService.FindPolicies(query);

            // Assert
            Assert.IsFalse(result.IsSuccess());
            Assert.AreEqual(QueryFailureCategory.ParameterValidation, result.FailureCategory);
            Assert.AreEqual(QueryStatus.Failed, result.Status);
        }
        [TestMethod]
        public async Task Given_Error_During_Execution_When_Finding_Policies_Should_Return_Failed_Status_With_Exception_Info() {
            // Arrange
            var policyRepositoryMock = new Mock<IPolicyRepository>();
            var validationServiceMock = new Mock<IValidationService>();
            var identityServiceMock = new Mock<IIdentityService<ClaimsIdentity, object>>();
            var loggerMock = new Mock<ILogger<PolicyService<object>>>();
            var policyService = new PolicyService<object>(
                policyRepositoryMock.Object,
                validationServiceMock.Object,
                identityServiceMock.Object,
                loggerMock.Object
            );

            var query = new FindPoliciesQuery { ResourceFilter = new Resource { Authority = "abc", Identifier = "def" }, PageNumber = 1, PageSize = 10 };
            var exception = new Exception("An error occurred during execution");
            policyRepositoryMock.Setup(repo => repo.FindPolicies(query))
                .Throws(exception);
            validationServiceMock.Setup(service => service.Validate(It.IsAny<object>()))
                .ReturnsAsync(new InternalValidationResult());

            // Act
            var result = await policyService.FindPolicies(query);

            // Assert
            Assert.IsFalse(result.IsSuccess());
            Assert.AreEqual(QueryStatus.Failed, result.Status);
            Assert.AreEqual(exception.Message, result.ExecutionException?.Message);
        }
        
        
    }
}
