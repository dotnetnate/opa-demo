using AutoMapper;
using CitizensFinancialGroup.Threvw.Policies.Domain;
using CitizensFinancialGroup.Threvw.Policy.Service.Http.Features.Policies.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policy.Service.Http.Features.Policies.Controllers {
    /// <summary>
    /// Policy Decision Point (PDP) controller.
    /// Evaluates policy decisions as an alternative to OPA.
    /// </summary>
    [ApiController]
    [Route("api/policy-decisions")]
    public class PolicyDecisionController : ControllerBase {
        private readonly IPolicyEvaluationService _evaluationService;
        private readonly ILogger<PolicyDecisionController> _logger;
        private readonly IMapper _mapper;

        public PolicyDecisionController(
            IPolicyEvaluationService evaluationService,
            ILogger<PolicyDecisionController> logger,
            IMapper mapper) {
            _evaluationService = evaluationService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Evaluates a policy decision for a resource, subject, and action.
        /// </summary>
        /// <param name="request">The evaluation request containing resource, subject, and action details.</param>
        /// <returns>A policy decision with obligations and advice.</returns>
        [HttpPost("evaluate")]
        [ProducesResponseType(typeof(EvaluatePolicyResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> EvaluatePolicy([FromBody] EvaluatePolicyRequest request) {
            if (request == null) {
                return BadRequest("Request body is required");
            }

            if (request.Resource is null || request.Subject is null || request.Action is null) {
                return BadRequest("Resource, Subject, and Action are required");
            }

            if (string.IsNullOrEmpty(request.Action.Id)) {
                return BadRequest("Action ID is required");
            }

            _logger.LogInformation(
                "Evaluating policy for Resource={ResourceId}:{ResourceType}, Subject={SubjectId}:{SubjectType}, Action={ActionId}",
                request.Resource.Identifier,
                request.Resource.Authority,
                request.Subject.Identifier,
                request.Subject.Authority,
                request.Action.Id);

            try {
                // Map request models to domain models
                var resource = _mapper.Map<Resource>(request.Resource);
                var subject = _mapper.Map<Subject>(request.Subject);

                // Evaluate policy
                var result = await _evaluationService.EvaluatePolicy(
                    resource,
                    subject,
                    request.Action.Id,
                    request.Action.Context);

                // Map result to response model
                var response = new EvaluatePolicyResponse {
                    Decision = (Models.PolicyDecision)(int)result.Decision, // Enum mapping by value
                    Obligations = _mapper.Map<List<ObligationModel>>(result.Obligations),
                    Advice = _mapper.Map<List<AdviceModel>>(result.Advice),
                    EvaluationDetails = result.EvaluationDetails
                };

                _logger.LogInformation(
                    "Policy evaluation completed: Decision={Decision}, Obligations={ObligationCount}, Advice={AdviceCount}",
                    response.Decision,
                    response.Obligations?.Count ?? 0,
                    response.Advice?.Count ?? 0);

                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error during policy evaluation");
                return StatusCode(500, new {
                    error = "An error occurred during policy evaluation",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Health check endpoint to verify the PDP is operational.
        /// </summary>
        [HttpGet("health")]
        [ProducesResponseType(200)]
        public IActionResult HealthCheck() {
            return Ok(new {
                status = "healthy",
                service = "Policy Decision Point",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
