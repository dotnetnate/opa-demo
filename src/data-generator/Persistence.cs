using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System.Text.Json;

namespace data_generator
{
    public interface IPersistence
    {
        void Persist(object data);
        void Flush();
    }


    public class StreamPersistence : IPersistence
    {

        private readonly StreamWriter _streamWriter;
        private bool _first = true;

        public StreamPersistence(StreamWriter streamWriter)
        {
            _streamWriter = streamWriter;
        }
        public void Flush()
        {
            _streamWriter.Flush();
        }

        public void Persist(object data)
        {
            if (!_first)
            {
                _streamWriter.WriteLine(",");
            }
            _first = false;
            _streamWriter.Write(JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
        }

        public void Complete()
        {
            _streamWriter.WriteLine();
            _streamWriter.WriteLine("]");
        }

    }

    public class MongoPersistence : IPersistence
    {
        private readonly IMongoCollection<PolicyWrapper> _collection;

        public MongoPersistence(IMongoCollection<PolicyWrapper> collection)
        {
            _collection = collection;
        }

        public void Flush()
        {
        }

        public void Persist(object data)
        {
            //var document = BsonDocument.Parse(JsonSerializer.Serialize(data));
            _collection.InsertOne((PolicyWrapper)data);
        }
    }

    public class MongoDbPolicyRepositoryConfiguration
    {
        public static void Configure()
        {

            RegisterSerializers();

            RegisterConventions();

            RegisterClassMaps();

        }

        private static void RegisterClassMaps()
        {
            BsonClassMap.RegisterClassMap<PolicyWrapper>(cm => {
                cm.AutoMap();
                cm.MapIdMember(c => c.id);                  
            });  
        }

        private static void RegisterSerializers()
        {

            var objectDiscriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(object));
            var objectSerializer = new ObjectSerializer(objectDiscriminatorConvention, GuidRepresentation.Standard);
            BsonSerializer.RegisterSerializer(objectSerializer);

            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        }

        private static void RegisterConventions()
        {
            var jsonWriterSettings = new JsonWriterSettings
            {
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