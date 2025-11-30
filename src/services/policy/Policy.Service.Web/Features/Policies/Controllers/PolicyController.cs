using AutoMapper;
using CitizensFinancialGroup.Elements.ApplicationModel.Commands;
using CitizensFinancialGroup.Elements.ApplicationModel.Queries;

using CitizensFinancialGroup.Threvw.Policies.Domain;
using CitizensFinancialGroup.Threvw.Policy.Service.Http.Features.Policies.Models;
using CitizensFinancialGroup.Threvw.Policy.Service.Http.Features.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Web;


namespace CitizensFinancialGroup.Threvw.Policy.Service.Http.Features.Policies.Controllers {
    [ApiController]
    [Route("api/policies")]
    public class PolicyController(IPolicyService policyService, ILogger<PolicyController> logger, IMapper mapper) : ControllerBase {

        private readonly IPolicyService _policyService = policyService;
        private readonly ILogger<PolicyController> _logger = logger;
        private readonly IMapper _mapper = mapper;

        [HttpPost]
        public async Task<IActionResult> FindPolicies([FromBody] FindPoliciesRequest request) {
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

        [HttpGet("{authority}/{resourceId}")]
        public async Task<IActionResult> GetPolicyForResource([FromRoute] FindPolicyByResourceRequest request) {

            request.Authority = HttpUtility.UrlDecode(request.Authority);

            var query = _mapper.Map<FindPolicyByResourceQuery>(request);

            var result = await _policyService.GetPolicyByResource(query);

            if (result.IsSuccess()) {
                var retVal = _mapper.Map<PolicyModel>(result.Result);
                // Return ETag in response header
                if (!string.IsNullOrEmpty(retVal.ETag)) {
                    Response.Headers.Add("ETag", $"\"{retVal.ETag}\"");
                }
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
                return CreatedAtAction(nameof(GetPolicyForResource), new {  authority = retVal.Resource.Authority, resourceId = retVal.Resource.Identifier }, retVal);
            }
            else {
                return BuildErrorResponse(result);
            }
        }

        /// <summary>
        /// Updates a policy for an existing resource, overwriting the existing rules with the new rules.
        /// </summary>
        /// <param name="resourceId">The id of the resource.</param>
        /// <param name="authority">The authority of the resource id.</param>
        /// <param name="request">The request body.</param>
        /// <returns></returns>
        [HttpPut("{authority}/{resourceId}")]
        public async Task<IActionResult> UpdatePolicy(string resourceId, string authority, [FromBody] UpdatePolicyRequest request) {
            
            var command = _mapper.Map<UpdatePolicyCommand>(request);
            
            command.Resource = new Resource { Identifier = resourceId, Authority = authority};
            
            // Get ETag from If-Match header if not in body
            if (string.IsNullOrEmpty(command.ETag) && Request.Headers.ContainsKey("If-Match")) {
                command.ETag = Request.Headers["If-Match"].ToString().Trim('"');
            }
            
            var result = await _policyService.UpdatePolicy(command);

            if (result.IsSuccess()) {
                var retVal = _mapper.Map<PolicyModel>(result.Result);
                // Return new ETag in response
                if (!string.IsNullOrEmpty(retVal.ETag)) {
                    Response.Headers.Add("ETag", $"\"{retVal.ETag}\"");
                }
                return Ok(retVal);
            }
            else {
                return BuildErrorResponse(result);
            }
        }

        [HttpDelete("{authority}/{resourceId}")]
        public async Task<IActionResult> DeletePolicyForResource(string authority, string resourceId) {
            var command = new DeletePolicyCommand {
                Resource = new Resource { Identifier = resourceId, Authority = authority }
            };

            // Get ETag from If-Match header
            if (Request.Headers.ContainsKey("If-Match")) {
                command.ETag = Request.Headers["If-Match"].ToString().Trim('"');
            }

            var result = await _policyService.DeletePolicy(command);

            if (result.IsSuccess()) {
                return NoContent();
            }
            else {
                return BuildErrorResponse(result);
            }
        }

        [HttpPost("{authority}/{resourceId}/rules")]
        public async Task<IActionResult> AddOrUpdateRule(string authority, string resourceId,[FromBody] AddOrUpdateRuleRequest model) {
            var command = _mapper.Map<AddOrUpdateRuleCommand>(model);
            command.Resource = new Resource { Identifier = resourceId, Authority = authority};

            // Get ETag from If-Match header if not in body
            if (string.IsNullOrEmpty(command.ETag) && Request.Headers.ContainsKey("If-Match")) {
                command.ETag = Request.Headers["If-Match"].ToString().Trim('"');
            }

            var result = await _policyService.AddOrUpdateRule(command);

            if (result.IsSuccess()) {
                return NoContent();
            }
            else {
                return BuildErrorResponse(result);
            }
        }

        [HttpDelete("{authority}/{resourceId}/rules/{subjectAuthority}/{subjectId}")]
        public async Task<IActionResult> DeleteRule(string authority, string resourceId, string subjectAuthority, string subjectId) {
            var command = new DeleteRuleCommand {
                Resource = new Resource { Identifier = resourceId, Authority = authority },
                Subject = new Subject { Authority = subjectAuthority, Identifier = subjectId }
            };

            // Get ETag from If-Match header
            if (Request.Headers.ContainsKey("If-Match")) {
                command.ETag = Request.Headers["If-Match"].ToString().Trim('"');
            }

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

                if (result.FailureCategory == CommandFailureCategory.ResourceConflict) {
                    return Conflict(new ErrorResult {
                        ValidationResult = _mapper.Map<InternalValidationResult>(result.ValidationResult),
                        Errors = result.ErrorMessages
                    });
                }
                else {

                    return BadRequest(new ErrorResult {
                        ValidationResult = _mapper.Map<InternalValidationResult>(result.ValidationResult),
                        Errors = result.ErrorMessages
                    });
                }
            }

            if (result.FailureCategory == CommandFailureCategory.ResourceConflict) {
                return Conflict();
            }

            return Problem("An unknown error occurred");
        }

        private IActionResult BuildErrorResponse(CommandResult result) {
            // Handle CommandResult<T> error responses
            if (result.ValidationResult?.IsValid() == false) {

                if (result.FailureCategory == CommandFailureCategory.ResourceConflict) {
                    return Conflict(new ErrorResult {
                        ValidationResult = _mapper.Map<InternalValidationResult>(result.ValidationResult),
                        Errors = result?.ErrorMessages
                    });
                }
                else {

                    return BadRequest(new ErrorResult {
                        ValidationResult = _mapper.Map<InternalValidationResult>(result.ValidationResult),
                        Errors = result?.ErrorMessages
                    });
                }
            }

            if (result.FailureCategory == CommandFailureCategory.ResourceConflict) {
                return Conflict();
            }

            return Problem("An unknown error occurred");
        }
    }
}
