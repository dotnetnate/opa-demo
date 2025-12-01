using NOCO.Threvw.Tenants.Domain;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Tenants.Infrastructure {

    public class MongoDbTenantRepositoryConfiguration {
        public static void Configure() {
            RegisterConventions();
            RegisterClassMaps();
                        
            var objectDiscriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(object));
            var objectSerializer = new ObjectSerializer(objectDiscriminatorConvention, GuidRepresentation.Standard);
            BsonSerializer.RegisterSerializer(objectSerializer);
            
        }

        private static void RegisterClassMaps() {
            BsonClassMap.RegisterClassMap<Tenant>(cm => {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id)
                  .SetIdGenerator(GuidGenerator.Instance)
                  .SetSerializer(new MongoDB.Bson.Serialization.Serializers.GuidSerializer(MongoDB.Bson.BsonType.String));
            });
        }

        private static void RegisterConventions() {
            var conventionPack = new ConventionPack {
                new CamelCaseElementNameConvention()                
            };
            ConventionRegistry.Register("CamelCaseConventions", conventionPack, t => true);
            
        }
    }
}
