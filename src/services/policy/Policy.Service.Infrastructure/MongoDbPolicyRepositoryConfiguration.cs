using CitizensFinancialGroup.Threvw.Policies.Domain;
using CitizensFinancialGroup.Threvw.Policies.Infrastructure;
using CitizensFinancialGroup.Threvw.Tenants.Domain;
using MongoDB.Bson;
using MongoDB.Bson.IO;
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

namespace CitizensFinancialGroup.Threvw.Tenants.Infrastructure {

    public class MongoDbPolicyRepositoryConfiguration {
        public static void Configure() {

            RegisterSerializers();

            RegisterConventions();
           
            RegisterClassMaps();

        }

        private static void RegisterClassMaps() {
        
            BsonClassMap.RegisterClassMap<Policy>(cm => {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id);                  
            });  
        }

        private static void RegisterSerializers() {

            var objectDiscriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(object));
            var objectSerializer = new ObjectSerializer(objectDiscriminatorConvention, GuidRepresentation.Standard);
            BsonSerializer.RegisterSerializer(objectSerializer);

            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            BsonSerializer.RegisterSerializer(typeof(Rule), new RuleBsonSerializer());

        }

        private static void RegisterConventions() {
            var jsonWriterSettings = new JsonWriterSettings {
                OutputMode = JsonOutputMode.CanonicalExtendedJson // Use "strict" JSON without extended format
            };

            JsonWriterSettings.Defaults = jsonWriterSettings;

            var conventionPack = new ConventionPack {
                new CamelCaseElementNameConvention(),
                new EnumRepresentationConvention(BsonType.String)
            };
            ConventionRegistry.Register("CamelCaseConventions", conventionPack, t => true);
            
        }
    }
}
