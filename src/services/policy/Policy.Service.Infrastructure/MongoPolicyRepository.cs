using System.Threading.Tasks;
using CitizensFinancialGroup.Elements.Data.MongoDb.Configuration;
using CitizensFinancialGroup.Threvw.Policies.Domain;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class MongoDbPolicyRepository : IPolicyRepository {
    private readonly IMongoCollection<Policy> _policyCollection;

    public MongoDbPolicyRepository(IMongoClient client, IOptions<MongoDbCollectionOptions> options) {

        var database = client.GetDatabase(options.Value.Database);

        _policyCollection = database.GetCollection<Policy>(options.Value.Collection);
    }

    public async Task<Policy?> FindPolicyByResource(FindPolicyByResourceQuery query) {
        return await _policyCollection
            .Find(policy => policy.Resource.Identifier == query.Resource.Identifier && policy.Resource.Authority == query.Resource.Authority)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Policy>> FindPolicies(FindPoliciesQuery query) {
        var builder = Builders<Policy>.Filter;
        var filters = new List<FilterDefinition<Policy>>();


        if( query.ResourceFilter != null) {
            filters.Add(builder.Eq(p => p.Resource, query.ResourceFilter));
        }

        if( query.SubjectFilter != null) {
            filters.Add(builder.ElemMatch(p => p.Rules, r => r.Subject == query.SubjectFilter));
        }     

        var combinedFilter = filters.Count > 0 ? builder.And(filters) : builder.Empty;

        return await _policyCollection
            .Find(combinedFilter)
            .SortBy(p => p.Resource.Identifier)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Limit(query.PageSize)
            .ToListAsync();
    }

    public async Task CreatePolicy(Policy policy) {
        // since the resource is the _id field, we can utilize the invalid operation exception that will be thrown and allow it to
        // check for duplicates instead of fetching and checking
        try {
            await _policyCollection.InsertOneAsync(policy);
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey) {
            throw new InvalidOperationException($"Policy with resource '{policy.Resource.Identifier}' and type '{policy.Resource.Authority}' already exists.");
        }        
    }

    public async Task UpdatePolicy(Resource resource, Policy policy) {
        await _policyCollection.ReplaceOneAsync(
            p => p.Resource.Identifier == resource.Identifier &&
                 p.Resource.Authority == resource.Authority,
            policy,
            new ReplaceOptions { IsUpsert = false });
    }

    public async Task DeletePolicy(Resource resource) {
        await _policyCollection.DeleteOneAsync(policy =>
            policy.Resource.Identifier == resource.Identifier &&
            policy.Resource.Authority == resource.Authority);
    }

    public async Task AddOrUpdateRule(Resource resource, Rule rule) {
        var filter = Builders<Policy>.Filter.And(
            Builders<Policy>.Filter.Eq(p => p.Resource.Identifier, resource.Identifier),
            Builders<Policy>.Filter.Eq(p => p.Resource.Authority, resource.Authority)
        );

        var update = Builders<Policy>.Update
            .PullFilter(p => p.Rules, r => r.Subject.Identifier == rule.Subject.Identifier)
            .AddToSet(p => p.Rules, rule);

        var result = await _policyCollection.UpdateOneAsync(filter, update);

        if (result.MatchedCount == 0) {
            throw new KeyNotFoundException($"Policy with resource '{resource.Identifier}' and type '{resource.Authority}' not found.");
        }
    }

    public async Task DeleteRule(Resource resource, string subjectId) {
        var filter = Builders<Policy>.Filter.And(
            Builders<Policy>.Filter.Eq(p => p.Resource.Identifier, resource.Identifier),
            Builders<Policy>.Filter.Eq(p => p.Resource.Authority, resource.Authority)
        );

        var update = Builders<Policy>.Update
            .PullFilter(p => p.Rules, r => r.Subject.Identifier == subjectId);

        await _policyCollection.UpdateOneAsync(filter, update);
    }
}