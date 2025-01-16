using CitizensFinancialGroup.Elements.Data.MongoDb.Configuration;
using CitizensFinancialGroup.Threvw.Tenants.Domain;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Tenants.Infrastructure {

    public class MongoDbTenantRepository : ITenantRepository {
        private readonly IMongoCollection<Tenant> _collection;        

        public MongoDbTenantRepository(IMongoClient client,  IOptions<MongoDbCollectionOptions> options) {

            var database = client.GetDatabase(options.Value.Database);
            
            _collection = database.GetCollection<Tenant>(options.Value.Collection);
        }

        public async Task<IEnumerable<Tenant>> FindTenants(int pageSize, int pageNumber) {
            return await _collection.Find(_ => true).Skip(pageSize * (pageNumber-1)).Limit(pageSize).ToListAsync();
        }

        public async Task<Tenant> FindTenantById(Guid id) {
            return await _collection.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateTenant(Tenant tenant) {

            try {

                await _collection.InsertOneAsync(tenant);
            }
            catch (MongoWriteException ex) {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey) {                    
                    throw new InvalidOperationException("A tenant with the same SysId or Name exists.");
                }
                throw;
            }
        }

        public async Task UpdateTenant(Tenant tenant) {

            // only allow these fields to be set directly on the tenant
            await _collection.UpdateOneAsync(t => t.Id == tenant.Id, new UpdateDefinitionBuilder<Tenant>()
                .Set(t => t.Name, tenant.Name)
                .Set(t => t.Description, tenant.Description));

            await _collection.ReplaceOneAsync(t => t.Id == tenant.Id, tenant);
        }

        public async Task DeleteTenant(Guid id) {
            await _collection.DeleteOneAsync(t => t.Id == id);
        }

        public async Task<TenantSettings?> GetSettings(Guid tenantId) {
            var tenant = await FindTenantById(tenantId);
            return tenant?.Settings;
        }

        public async Task<object?> GetSettingByKey(Guid tenantId, string key) {
            // Fetch the settings subdocument
            var filter = Builders<Tenant>.Filter.Eq(t => t.Id, tenantId);
            var projection = Builders<Tenant>.Projection.Include(t => t.Settings);
            var result = await _collection.Find(filter).Project<Tenant>(projection).FirstOrDefaultAsync();

            if (result?.Settings == null) return null;

            // Use reflection to find the property in a case-insensitive manner
            var property = result.Settings.GetType().GetProperties()
                .FirstOrDefault(p => string.Equals(p.Name, key, StringComparison.OrdinalIgnoreCase));

            if (property == null) return null;

            // Return the value of the property
            return property.GetValue(result.Settings);
        }

        public async Task UpdateSettings(Guid tenantId, TenantSettings settings) {
            var update = Builders<Tenant>.Update.Set(t => t.Settings, settings);
            await _collection.UpdateOneAsync(t => t.Id == tenantId, update);
        }

        public async Task UpdateSettingByKey(Guid tenantId, string key, object value) {
            var tenant = await FindTenantById(tenantId);
            if (tenant == null) return;

            var property = tenant.Settings.GetType().GetProperty(key);
            if (property == null) return;

            property.SetValue(tenant.Settings, value);
            await _collection.ReplaceOneAsync(t => t.Id == tenantId, tenant);
        }
    }
}
