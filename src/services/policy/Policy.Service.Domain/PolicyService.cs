using NOCO.Elements.ApplicationModel.Commands;
using NOCO.Elements.ApplicationModel.Queries;
using NOCO.Elements.Security.Identity;
using NOCO.Elements.Validation;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NOCO.Threvw.Policies.Domain {

    /// <summary>
    /// Service class for managing policies.
    /// </summary>
    /// <typeparam name="TIdentityContext">The type of the identity context.</typeparam>
    public class PolicyService<TIdentityContext> : ServiceBase<ClaimsIdentity, TIdentityContext>, IPolicyService {
        private readonly IPolicyRepository _policyRepository;

        private const int MAX_PAGE_SIZE = 50;
        private const int DEFAULT_PAGE_SIZE = 10;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyService{TIdentityContext}"/> class.
        /// </summary>
        /// <param name="policyRepository">The policy repository.</param>
        /// <param name="validationService">The validation service.</param>
        /// <param name="identityService">The identity service.</param>
        /// <param name="logger">The logger.</param>
        public PolicyService(
            IPolicyRepository policyRepository,
            IValidationService validationService,
            IIdentityService<ClaimsIdentity, TIdentityContext> identityService,
            ILogger<PolicyService<TIdentityContext>> logger)
            : base(validationService, logger, identityService) {
            _policyRepository = policyRepository;
        }

        /// <summary>
        /// Finds policies based on the specified query.
        /// </summary>
        /// <param name="query">The query containing filtering and pagination options.</param>
        /// <returns>A collection of policies matching the criteria.</returns>
        public async Task<QueryResult<IEnumerable<Policy>>> FindPolicies(FindPoliciesQuery query) {
            return await ExecuteQuery(query, async (query) => {
                var results = await _policyRepository.FindPolicies(
                    query
                );
                return QueryResult<IEnumerable<Policy>>.SuccessResult(results);
            });
        }

        /// <summary>
        /// Retrieves a policy based on the specified resource.
        /// </summary>
        /// <param name="query">The query containing the resource information.</param>
        /// <returns>The policy if found, otherwise a not-found result.</returns>
        public async Task<QueryResult<Policy>> GetPolicyByResource(FindPolicyByResourceQuery query) {
            return await ExecuteQuery(query, async (query) => {
                var policy = await _policyRepository.FindPolicyByResource(query);
                if (policy != null) {
                    return QueryResult<Policy>.SuccessResult(policy);
                }
                else {
                    return QueryResult<Policy>.NotFoundResult(policy);
                }
            });
        }

        /// <summary>
        /// Creates a new policy in the system.
        /// </summary>
        /// <param name="command">The command containing the policy details to create.</param>
        /// <returns>The created policy.</returns>
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
                    return CommandResult.ResourceConflictResult<Policy>(validationResult: new InternalValidationResult { Errors = [new() { PropertyName = "policy.resource", ErrorMessage = "A policy for the same resource already exists." }] });
                }
            });
        }

        /// <summary>
        /// Updates an existing policy.
        /// </summary>
        /// <param name="command">The command containing the updated policy details.</param>
        /// <returns>The updated policy.</returns>
        public async Task<CommandResult<Policy>> UpdatePolicy(UpdatePolicyCommand command) {
            return await ExecuteCommand(command, async (command) => {
                var policy = new Policy {
                    Resource = command.Resource,
                    Rules = command.Rules
                };

                try {
                    var success = await _policyRepository.UpdatePolicy(command.Resource, policy, command.ETag);
                    
                    if (!success) {
                        // ETag mismatch - concurrent modification detected
                        return CommandResult.VersionConflictResult<Policy>(
                            validationResult: new InternalValidationResult {
                                Errors =
                                [
                                new() {
                                        PropertyName = "policy.etag",
                                        ErrorMessage = "The policy has been modified by another request. Please refresh and try again."
                                    }
                                ]
                            });
                    }
                    
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

        /// <summary>
        /// Deletes a policy identified by the specified resource.
        /// </summary>
        /// <param name="command">The command containing the resource identifying the policy to delete.</param>
        /// <returns>A command result indicating success or failure.</returns>
        public async Task<CommandResult> DeletePolicy(DeletePolicyCommand command) {
            return await ExecuteCommand(command, async (command) => {
                var success = await _policyRepository.DeletePolicy(command.Resource, command.ETag);
                
                if (!success) {
                    // ETag mismatch - concurrent modification detected
                    return CommandResult.VersionConflictResult(
                        validationResult: new InternalValidationResult {
                            Errors =
                            [
                            new() {
                                    PropertyName = "policy.etag",
                                    ErrorMessage = "The policy has been modified by another request. Please refresh and try again."
                                }
                            ]
                        });
                }
                
                return CommandResult.SuccessResult();
            });
        }

        /// <summary>
        /// Adds or updates a rule in a specific policy.
        /// </summary>
        /// <param name="command">The command containing the rule and resource details.</param>
        /// <returns>A command result indicating success or failure.</returns>
        public async Task<CommandResult> AddOrUpdateRule(AddOrUpdateRuleCommand command) {
            return await ExecuteCommand(command, async (command) => {
                try {
                    var success = await _policyRepository.AddOrUpdateRule(command.Resource, command.Rule, command.ETag);
                    
                    if (!success) {
                        // ETag mismatch - concurrent modification detected
                        return CommandResult.VersionConflictResult(
                            validationResult: new InternalValidationResult {
                                Errors =
                                [
                                new() {
                                        PropertyName = "policy.etag",
                                        ErrorMessage = "The policy has been modified by another request. Please refresh and try again."
                                    }
                                ]
                            });
                    }
                    
                    return CommandResult.SuccessResult();
                }
                catch (KeyNotFoundException ex) {
                    Logger.LogError(ex, "Policy not found for resource: {Resource}", command.Resource);
                    return CommandResult.FailureResult(
                        validationResult: new InternalValidationResult {
                            Errors =
                            [
                            new() {
                                    PropertyName = "policy.resource",
                                    ErrorMessage = "The specified policy does not exist."
                                }
                            ]
                        });
                }
            });
        }

        /// <summary>
        /// Deletes a specific rule from a policy.
        /// </summary>
        /// <param name="command">The command containing the resource and subject (authority + identifier) of the rule to delete.</param>
        /// <returns>A command result indicating success or failure.</returns>
        public async Task<CommandResult> DeleteRule(DeleteRuleCommand command) {
            return await ExecuteCommand(command, async (command) => {
                var success = await _policyRepository.DeleteRule(command.Resource, command.Subject, command.ETag);
                
                if (!success) {
                    // ETag mismatch - concurrent modification detected
                    return CommandResult.VersionConflictResult(
                        validationResult: new InternalValidationResult {
                            Errors =
                            [
                            new() {
                                    PropertyName = "policy.etag",
                                    ErrorMessage = "The policy has been modified by another request. Please refresh and try again."
                                }
                            ]
                        });
                }
                
                return CommandResult.SuccessResult();
            });
        }
    }
}