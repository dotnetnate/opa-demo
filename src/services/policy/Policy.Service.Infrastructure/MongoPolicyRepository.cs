using System.Threading.Tasks;
using CitizensFinancialGroup.Threvw.Common.Data.MongoDb;
using CitizensFinancialGroup.Threvw.Policies.Domain;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class MongoDbPolicyRepository : IPolicyRepository {
    private readonly IMongoCollection<Policy> _policyCollection;

    public MongoDbPolicyRepository(IMongoClient client, IOptions<MongoDbCollectionOptions> options) {

        var database = client.GetDatabase(options.Value.Database);

        _policyCollection = database.GetCollection<Policy>(options.Value.Collection);
    }

    public async Task<Policy> GetPolicy(Resource resource) {
        return await _policyCollection
            .Find(policy => policy.Resource.ResourceId == resource.ResourceId &&
                            policy.Resource.ResourceType == resource.ResourceType)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Policy>> FindPolicies(PolicyFilter filter, int pageNumber, int pageSize) {
        var builder = Builders<Policy>.Filter;
        var filters = new List<FilterDefinition<Policy>>();

        if (!string.IsNullOrEmpty(filter.ResourceId)) {
            filters.Add(builder.Eq(p => p.Resource.ResourceId, filter.ResourceId));
        }

        if (!string.IsNullOrEmpty(filter.ResourceType)) {
            filters.Add(builder.Eq(p => p.Resource.ResourceType, filter.ResourceType));
        }

        if (!string.IsNullOrEmpty(filter.SubjectId)) {
            filters.Add(builder.ElemMatch(p => p.Rules, r => r.Subject.Identifier == filter.SubjectId));
        }

        var combinedFilter = filters.Count > 0 ? builder.And(filters) : builder.Empty;

        return await _policyCollection
            .Find(combinedFilter)
            .SortBy(p => p.Resource.ResourceId)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task CreatePolicy(Policy policy) {
        var existingPolicy = await GetPolicy(policy.Resource);
        if (existingPolicy != null) {
            throw new InvalidOperationException($"A policy with resource '{policy.Resource.ResourceId}' and type '{policy.Resource.ResourceType}' already exists.");
        }

        await _policyCollection.InsertOneAsync(policy);
    }

    public async Task UpdatePolicy(Resource resource, Policy policy) {
        await _policyCollection.ReplaceOneAsync(
            p => p.Resource.ResourceId == resource.ResourceId &&
                 p.Resource.ResourceType == resource.ResourceType,
            policy,
            new ReplaceOptions { IsUpsert = false });
    }

    public async Task DeletePolicy(Resource resource) {
        await _policyCollection.DeleteOneAsync(policy =>
            policy.Resource.ResourceId == resource.ResourceId &&
            policy.Resource.ResourceType == resource.ResourceType);
    }

    public async Task AddOrUpdateRule(Resource resource, Rule rule) {
        var filter = Builders<Policy>.Filter.And(
            Builders<Policy>.Filter.Eq(p => p.Resource.ResourceId, resource.ResourceId),
            Builders<Policy>.Filter.Eq(p => p.Resource.ResourceType, resource.ResourceType)
        );

        var update = Builders<Policy>.Update
            .PullFilter(p => p.Rules, r => r.Subject.Identifier == rule.Subject.Identifier)
            .AddToSet(p => p.Rules, rule);

        var result = await _policyCollection.UpdateOneAsync(filter, update);

        if (result.MatchedCount == 0) {
            throw new KeyNotFoundException($"Policy with resource '{resource.ResourceId}' and type '{resource.ResourceType}' not found.");
        }
    }

    public async Task DeleteRule(Resource resource, string subjectId) {
        var filter = Builders<Policy>.Filter.And(
            Builders<Policy>.Filter.Eq(p => p.Resource.ResourceId, resource.ResourceId),
            Builders<Policy>.Filter.Eq(p => p.Resource.ResourceType, resource.ResourceType)
        );

        var update = Builders<Policy>.Update
            .PullFilter(p => p.Rules, r => r.Subject.Identifier == subjectId);

        await _policyCollection.UpdateOneAsync(filter, update);
    }
}