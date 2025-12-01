using NOCO.Elements.ApplicationModel.Commands;
using NOCO.Elements.ApplicationModel.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace NOCO.Threvw.Policies.Domain {
    public interface IPolicyService {
        /// <summary>
        /// Finds policies based on filtering criteria and pagination options.
        /// </summary>
        /// <param name="query">The query containing filtering and pagination options.</param>
        /// <returns>A collection of policies matching the criteria.</returns>
        Task<QueryResult<IEnumerable<Policy>>> FindPolicies(FindPoliciesQuery query);

        /// <summary>
        /// Retrieves a policy based on the specified resource.
        /// </summary>
        /// <param name="query">The query containing the resource information.</param>
        /// <returns>The policy if found, otherwise a not-found result.</returns>
        Task<QueryResult<Policy>> GetPolicyByResource(FindPolicyByResourceQuery query);

        /// <summary>
        /// Creates a new policy in the system.
        /// </summary>
        /// <param name="command">The command containing the policy details to create.</param>
        /// <returns>The created policy.</returns>
        Task<CommandResult<Policy>> CreatePolicy(CreatePolicyCommand command);

        /// <summary>
        /// Updates an existing policy.
        /// </summary>
        /// <param name="command">The command containing the updated policy details.</param>
        /// <returns>The updated policy.</returns>
        Task<CommandResult<Policy>> UpdatePolicy(UpdatePolicyCommand command);

        /// <summary>
        /// Deletes a policy identified by the specified resource.
        /// </summary>
        /// <param name="command">The command containing the resource identifying the policy to delete.</param>
        /// <returns>A command result indicating success or failure.</returns>
        Task<CommandResult> DeletePolicy(DeletePolicyCommand command);

        /// <summary>
        /// Adds or updates a rule in a specific policy.
        /// </summary>
        /// <param name="command">The command containing the rule and resource details.</param>
        /// <returns>A command result indicating success or failure.</returns>
        Task<CommandResult> AddOrUpdateRule(AddOrUpdateRuleCommand command);

        /// <summary>
        /// Deletes a specific rule from a policy.
        /// </summary>
        /// <param name="command">The command containing the resource and subject ID of the rule to delete.</param>
        /// <returns>A command result indicating success or failure.</returns>
        Task<CommandResult> DeleteRule(DeleteRuleCommand command);
    }
}