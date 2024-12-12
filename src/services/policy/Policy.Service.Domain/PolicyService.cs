using CitizensFinancialGroup.Elements.CQRS.Commands;
using CitizensFinancialGroup.Elements.CQRS.Queries;
using CitizensFinancialGroup.Threvw.Common.Identity;
using CitizensFinancialGroup.Threvw.Common.Validation;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {

    public class PolicyService<TIdentityContext> : ServiceBase<ClaimsIdentity, TIdentityContext>, IPolicyService {
        private readonly IPolicyRepository _policyRepository;

        private const int MAX_PAGE_SIZE = 50;
        private const int DEFAULT_PAGE_SIZE = 10;

        public PolicyService(
            IPolicyRepository policyRepository,
            IValidationService validationService,
            IIdentityService<ClaimsIdentity, TIdentityContext> identityService,
            ILogger<PolicyService<TIdentityContext>> logger)
            : base(validationService, logger, identityService) {
            _policyRepository = policyRepository;
        }

        // Query: Find Policies
        public async Task<QueryResult<IEnumerable<Policy>>> FindPolicies(FindPoliciesQuery query) {
            return await ExecuteQuery(query, async (query) => {
                var results = await _policyRepository.FindPolicies(
                    query.Filter,
                    query.PageNumber,
                    Math.Min(query.PageSize, MAX_PAGE_SIZE)
                );
                return QueryResult<IEnumerable<Policy>>.SuccessResult(results);
            });
        }

        // Query: Get Policy by Resource
        public async Task<QueryResult<Policy>> GetPolicyByResource(FindPolicyByResourceQuery query) {
            return await ExecuteQuery(query, async (query) => {
                var policy = await _policyRepository.GetPolicy(query.Resource);
                if (policy != null) {
                    return QueryResult<Policy>.SuccessResult(policy);
                }
                else {
                    return QueryResult<Policy>.NotFoundResult(policy);
                }
            });
        }

        // Command: Create Policy
        public async Task<CommandResult<Policy>> CreatePolicy(CreatePolicyCommand command) {
            return await ExecuteCommand(command, async (command) => {
                var policy = new Policy {
                    Resource = command.Resource,
                    Rules = command.Rules
                };

                try {
                    await _policyRepository.CreatePolicy(policy);
                    return CommandResult.SuccessResult(policy);
                }
                catch (InvalidOperationException ex) {
                    Logger.LogError(ex, "Error creating policy: {Policy}", policy);
                    return CommandResult.ResourceConflictResult<Policy>(validationResult: new InternalValidationResult { Errors = [new() { PropertyName = "policy.resource", ErrorMessage = "A policy with the same resource already exists." }] });


                }
            });
        }

        // Command: Update Policy
        public async Task<CommandResult<Policy>> UpdatePolicy(UpdatePolicyCommand command) {
            return await ExecuteCommand(command, async (command) => {
                var policy = new Policy {
                    Resource = command.Resource,
                    Rules = command.Rules
                };

                try {
                    await _policyRepository.UpdatePolicy(command.Resource, policy);
                    return CommandResult.SuccessResult(policy);
                }
                catch (InvalidOperationException ex) {
                    Logger.LogError(ex, "Error updating policy: {Policy}", policy);
                    return CommandResult.ResourceConflictResult<Policy>(
                        validationResult: new InternalValidationResult {
                            Errors =
                            [
                            new() {
                                PropertyName = "policy.resource",
                                ErrorMessage = "Failed to update the policy due to a conflict."
                            }
                            ]
                        });
                }
            });
        }

        // Command: Delete Policy
        public async Task<CommandResult> DeletePolicy(DeletePolicyCommand command) {
            return await ExecuteCommand(command, async (command) => {
                await _policyRepository.DeletePolicy(command.Resource);
                return CommandResult.SuccessResult();
            });
        }

        // Command: Add or Update Rule
        public async Task<CommandResult> AddOrUpdateRule(AddOrUpdateRuleCommand command) {
            return await ExecuteCommand(command, async (command) => {
                await _policyRepository.AddOrUpdateRule(command.Resource, command.Rule);
                return CommandResult.SuccessResult();
            });
        }

        // Command: Delete Rule
        public async Task<CommandResult> DeleteRule(DeleteRuleCommand command) {
            return await ExecuteCommand(command, async (command) => {
                await _policyRepository.DeleteRule(command.Resource, command.SubjectId);
                return CommandResult.SuccessResult();
            });
        }
    }
}