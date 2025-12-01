using System.Threading.Tasks;
using NOCO.Elements.Data.MongoDb.Configuration;
using NOCO.Threvw.Policies.Domain;
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
            // Generate initial ETag
            policy.ETag = Guid.NewGuid().ToString();
            await _policyCollection.InsertOneAsync(policy);
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey) {
            throw new InvalidOperationException($"Policy with resource '{policy.Resource.Identifier}' and type '{policy.Resource.Authority}' already exists.");
        }        
    }

    public async Task<bool> UpdatePolicy(Resource resource, Policy policy, string? expectedETag) {
        var filterBuilder = Builders<Policy>.Filter;
        var filter = filterBuilder.And(
            filterBuilder.Eq(p => p.Resource.Identifier, resource.Identifier),
            filterBuilder.Eq(p => p.Resource.Authority, resource.Authority)
        );

        // Add ETag check if provided
        if (expectedETag != null) {
            filter = filterBuilder.And(filter, filterBuilder.Eq(p => p.ETag, expectedETag));
        }

        // Generate new ETag
        policy.ETag = Guid.NewGuid().ToString();

        var result = await _policyCollection.ReplaceOneAsync(
            filter,
            policy,
            new ReplaceOptions { IsUpsert = false });

        return result.MatchedCount > 0;
    }

    public async Task<bool> DeletePolicy(Resource resource, string? expectedETag) {
        var filterBuilder = Builders<Policy>.Filter;
        var filter = filterBuilder.And(
            filterBuilder.Eq(p => p.Resource.Identifier, resource.Identifier),
            filterBuilder.Eq(p => p.Resource.Authority, resource.Authority)
        );

        // Add ETag check if provided
        if (expectedETag != null) {
            filter = filterBuilder.And(filter, filterBuilder.Eq(p => p.ETag, expectedETag));
        }

        var result = await _policyCollection.DeleteOneAsync(filter);

        return result.DeletedCount > 0;
    }

    public async Task<bool> AddOrUpdateRule(Resource resource, Rule rule, string? expectedETag) {
        var filterBuilder = Builders<Policy>.Filter;
        var filter = filterBuilder.And(
            filterBuilder.Eq(p => p.Resource.Identifier, resource.Identifier),
            filterBuilder.Eq(p => p.Resource.Authority, resource.Authority)
        );

        // Add ETag check if provided
        if (expectedETag != null) {
            filter = filterBuilder.And(filter, filterBuilder.Eq(p => p.ETag, expectedETag));
        }

        // Use compound key (authority + identifier) to match rules
        var update = Builders<Policy>.Update
            .PullFilter(p => p.Rules, r => 
                r.Subject.Identifier == rule.Subject.Identifier && 
                r.Subject.Authority == rule.Subject.Authority)
            .AddToSet(p => p.Rules, rule)
            .Set(p => p.ETag, Guid.NewGuid().ToString());

        var result = await _policyCollection.UpdateOneAsync(filter, update);

        if (result.MatchedCount == 0) {
            // Check if policy exists at all
            var existsFilter = filterBuilder.And(
                filterBuilder.Eq(p => p.Resource.Identifier, resource.Identifier),
                filterBuilder.Eq(p => p.Resource.Authority, resource.Authority)
            );
            var exists = await _policyCollection.Find(existsFilter).AnyAsync();
            
            if (!exists) {
                throw new KeyNotFoundException($"Policy with resource '{resource.Identifier}' and type '{resource.Authority}' not found.");
            }
            
            // Policy exists but ETag didn't match
            return false;
        }

        return true;
    }

    public async Task<bool> DeleteRule(Resource resource, Subject subject, string? expectedETag) {
        var filterBuilder = Builders<Policy>.Filter;
        var filter = filterBuilder.And(
            filterBuilder.Eq(p => p.Resource.Identifier, resource.Identifier),
            filterBuilder.Eq(p => p.Resource.Authority, resource.Authority)
        );

        // Add ETag check if provided
        if (expectedETag != null) {
            filter = filterBuilder.And(filter, filterBuilder.Eq(p => p.ETag, expectedETag));
        }

        // Use compound key (authority + identifier) to match rules
        var update = Builders<Policy>.Update
            .PullFilter(p => p.Rules, r => 
                r.Subject.Identifier == subject.Identifier && 
                r.Subject.Authority == subject.Authority)
            .Set(p => p.ETag, Guid.NewGuid().ToString());

        var result = await _policyCollection.UpdateOneAsync(filter, update);

        return result.MatchedCount > 0;
    }
}