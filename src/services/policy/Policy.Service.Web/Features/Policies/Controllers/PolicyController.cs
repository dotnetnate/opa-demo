using AutoMapper;
using CitizensFinancialGroup.Elements.CQRS.Commands;
using CitizensFinancialGroup.Elements.CQRS.Queries;

using CitizensFinancialGroup.Threvw.Policies.Domain;
using CitizensFinancialGroup.Threvw.Policy.Service.Http.Features.Policies.Models;
using CitizensFinancialGroup.Threvw.Policy.Service.Http.Features.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using InternalValidationResult = CitizensFinancialGroup.Threvw.Policy.Service.Http.Features.Policies.Models.InternalValidationResult;

namespace CitizensFinancialGroup.Threvw.Policy.Service.Http.Features.Policies.Controllers {
    [ApiController]
    [Route("api/policies")]
    public class PolicyController : ControllerBase {
        private readonly IPolicyService _policyService;
        private readonly ILogger<PolicyController> _logger;
        private readonly IMapper _mapper;

        public PolicyController(IPolicyService policyService, ILogger<PolicyController> logger, IMapper mapper) {
            _policyService = policyService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> FindPolicies([FromQuery] FindPoliciesRequest request) {
            var query = _mapper.Map<FindPoliciesQuery>(request);

            var result = await _policyService.FindPolicies(query);

            if (result.IsSuccess()) {
                var retVal = _mapper.Map<IEnumerable<PolicyModel>>(result.Result);
                return Ok(retVal);
            }
            else {
                return BuildErrorResponse(result);
            }
        }

        [HttpGet("{resourceId}/{resourceType}")]
        public async Task<IActionResult> GetPolicyByResource([FromRoute] FindPolicyByResourceRequest request) {
            var query = _mapper.Map<FindPolicyByResourceQuery>(request);

            var result = await _policyService.GetPolicyByResource(query);

            if (result.IsSuccess()) {
                var retVal = _mapper.Map<PolicyModel>(result.Result);
                return Ok(retVal);
            }
            else {
                return BuildErrorResponse(result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePolicy([FromBody] CreatePolicyRequest model) {
            _logger.LogInformation("Creating policy for resource: {Resource}", model.Resource);

            var command = _mapper.Map<CreatePolicyCommand>(model);
            var result = await _policyService.CreatePolicy(command);

            if (result.IsSuccess()) {
                var retVal = _mapper.Map<PolicyModel>(result.Result);
                return CreatedAtAction(nameof(GetPolicyByResource), new { resourceId = retVal.Resource.ResourceId, resourceType = retVal.Resource.ResourceType }, retVal);
            }
            else {
                return BuildErrorResponse(result);
            }
        }

        [HttpPut("{resourceId}/{resourceType}")]
        public async Task<IActionResult> UpdatePolicy(string resourceId, string resourceType, [FromBody] UpdatePolicyRequest model) {
            var command = _mapper.Map<UpdatePolicyCommand>(model);
            command.Resource = new Resource { ResourceId = resourceId, ResourceType = resourceType };

            var result = await _policyService.UpdatePolicy(command);

            if (result.IsSuccess()) {
                var retVal = _mapper.Map<PolicyModel>(result.Result);
                return Ok(retVal);
            }
            else {
                return BuildErrorResponse(result);
            }
        }

        [HttpDelete("{resourceId}/{resourceType}")]
        public async Task<IActionResult> DeletePolicy(string resourceId, string resourceType) {
            var command = new DeletePolicyCommand {
                Resource = new Resource { ResourceId = resourceId, ResourceType = resourceType }
            };

            var result = await _policyService.DeletePolicy(command);

            if (result.IsSuccess()) {
                return NoContent();
            }
            else {
                return BuildErrorResponse(result);
            }
        }

        [HttpPost("{resourceId}/{resourceType}/rules")]
        public async Task<IActionResult> AddOrUpdateRule(string resourceId, string resourceType, [FromBody] AddOrUpdateRuleRequest model) {
            var command = _mapper.Map<AddOrUpdateRuleCommand>(model);
            command.Resource = new Resource { ResourceId = resourceId, ResourceType = resourceType };

            var result = await _policyService.AddOrUpdateRule(command);

            if (result.IsSuccess()) {
                return NoContent();
            }
            else {
                return BuildErrorResponse(result);
            }
        }

        [HttpDelete("{resourceId}/{resourceType}/rules/{subjectId}")]
        public async Task<IActionResult> DeleteRule(string resourceId, string resourceType, string subjectId) {
            var command = new DeleteRuleCommand {
                Resource = new Resource { ResourceId = resourceId, ResourceType = resourceType },
                SubjectId = subjectId
            };

            var result = await _policyService.DeleteRule(command);

            if (result.IsSuccess()) {
                return NoContent();
            }
            else {
                return BuildErrorResponse(result);
            }
        }

        private IActionResult BuildErrorResponse<T>(QueryResult<T> result) {
            // Handle QueryResult error responses
            if (result.ValidationResult?.IsValid() == false) {
                return BadRequest(new ErrorResult {
                    ValidationResult = _mapper.Map<InternalValidationResult>(result.ValidationResult),
                    Errors = result.ErrorMessages
                });
            }

            if (result.FailureCategory == QueryFailureCategory.ResourceNotFound) {
                return NotFound();
            }

            return Problem("An unknown error occurred");
        }

        private IActionResult BuildErrorResponse<T>(CommandResult<T> result) {
            // Handle CommandResult<T> error responses
            if (result.ValidationResult?.IsValid() == false) {
                return BadRequest(new ErrorResult {
                    ValidationResult = _mapper.Map<InternalValidationResult>(result.ValidationResult),
                    Errors = result.ErrorMessages
                });
            }

            if (result.FailureCategory == CommandFailureCategory.ResourceConflict) {
                return Conflict();
            }

            return Problem("An unknown error occurred");
        }

        private IActionResult BuildErrorResponse(CommandResult result) {
            // Handle CommandResult error responses
            if (result.ValidationResult?.IsValid() == false) {
                return BadRequest(new ErrorResult {
                    ValidationResult = _mapper.Map<InternalValidationResult>(result.ValidationResult),
                    Errors = result.ErrorMessages
                });
            }

            if (result.FailureCategory == CommandFailureCategory.ResourceConflict) {
                return Conflict();
            }

            return Problem("An unknown error occurred");
        }
    }
}
