using System.Threading.Tasks;

namespace NOCO.Threvw.Policies.Domain {

    /// <summary>
    /// Repository for managing policies and related constructs such as policy definitions, etc.
    /// </summary>
    public interface IPolicyRepository {
        /// <summary>
        /// Retrieves a single policy based on the resource.
        /// </summary>
        /// <param name="resource">The resource used to identify the policy.</param>
        /// <returns>The policy if found, otherwise null.</returns>
        Task<Policy?> FindPolicyByResource(FindPolicyByResourceQuery query);

        /// <summary>
        /// Finds policies based on filtering criteria and paging options.
        /// </summary>
        /// <param name="filter">The filtering criteria.</param>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The size of each page for pagination.</param>
        /// <returns>A collection of policies that match the criteria.</returns>
        Task<IEnumerable<Policy>> FindPolicies(FindPoliciesQuery query);

        /// <summary>
        /// Creates a new policy in the repository.
        /// </summary>
        /// <param name="policy">The policy to create.</param>
        Task CreatePolicy(Policy policy);

        /// <summary>
        /// Updates an existing policy identified by the resource.
        /// </summary>
        /// <param name="resource">The resource used to identify the policy.</param>
        /// <param name="policy">The updated policy data.</param>
        /// <param name="expectedETag">The expected ETag for optimistic concurrency control. Null to skip check.</param>
        /// <returns>True if update succeeded, false if ETag mismatch.</returns>
        Task<bool> UpdatePolicy(Resource resource, Policy policy, string? expectedETag);

        /// <summary>
        /// Deletes a policy identified by the resource.
        /// </summary>
        /// <param name="resource">The resource used to identify the policy.</param>
        /// <param name="expectedETag">The expected ETag for optimistic concurrency control. Null to skip check.</param>
        /// <returns>True if delete succeeded, false if ETag mismatch.</returns>
        Task<bool> DeletePolicy(Resource resource, string? expectedETag);

        /// <summary>
        /// Adds or updates a rule within a policy identified by the resource.
        /// Replaces entire rule if it exists (matched by subject authority + identifier).
        /// </summary>
        /// <param name="resource">The resource used to identify the policy.</param>
        /// <param name="rule">The rule to add or update.</param>
        /// <param name="expectedETag">The expected ETag for optimistic concurrency control. Null to skip check.</param>
        /// <returns>True if update succeeded, false if ETag mismatch.</returns>
        Task<bool> AddOrUpdateRule(Resource resource, Rule rule, string? expectedETag);

        /// <summary>
        /// Deletes a rule from a policy identified by the resource and subject (authority + identifier).
        /// </summary>
        /// <param name="resource">The resource used to identify the policy.</param>
        /// <param name="subject">The subject (authority + identifier) whose rule should be deleted.</param>
        /// <param name="expectedETag">The expected ETag for optimistic concurrency control. Null to skip check.</param>
        /// <returns>True if delete succeeded, false if ETag mismatch.</returns>
        Task<bool> DeleteRule(Resource resource, Subject subject, string? expectedETag);
    }

}