using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {

    public interface IPolicyRepository {
        /// <summary>
        /// Retrieves a single policy based on the resource.
        /// </summary>
        /// <param name="resource">The resource used to identify the policy.</param>
        /// <returns>The policy if found, otherwise null.</returns>
        Task<Policy> GetPolicy(Resource resource);

        /// <summary>
        /// Finds policies based on filtering criteria and paging options.
        /// </summary>
        /// <param name="filter">The filtering criteria.</param>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The size of each page for pagination.</param>
        /// <returns>A collection of policies that match the criteria.</returns>
        Task<IEnumerable<Policy>> FindPolicies(PolicyFilter filter, int pageNumber, int pageSize);

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
        Task UpdatePolicy(Resource resource, Policy policy);

        /// <summary>
        /// Deletes a policy identified by the resource.
        /// </summary>
        /// <param name="resource">The resource used to identify the policy.</param>
        Task DeletePolicy(Resource resource);

        /// <summary>
        /// Adds or updates a rule within a policy identified by the resource.
        /// </summary>
        /// <param name="resource">The resource used to identify the policy.</param>
        /// <param name="rule">The rule to add or update.</param>
        Task AddOrUpdateRule(Resource resource, Rule rule);

        /// <summary>
        /// Deletes a rule from a policy identified by the resource and subject identifier.
        /// </summary>
        /// <param name="resource">The resource used to identify the policy.</param>
        /// <param name="subjectId">The identifier of the subject whose rule should be deleted.</param>
        Task DeleteRule(Resource resource, string subjectId);
    }

}