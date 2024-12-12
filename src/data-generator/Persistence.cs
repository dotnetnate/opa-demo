using MongoDB.Bson;
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

        public void Flush()
        {
            _streamWriter.Flush();
        }
    }
    public class MongoPersistence : IPersistence
    {
        private readonly IMongoCollection<BsonDocument> _collection;

        public MongoPersistence(IMongoCollection<BsonDocument> collection)
        {
            _collection = collection;
        }

        public void Flush()
        {
        }

        public void Persist(object data)
        {
            var document = BsonDocument.Parse(JsonSerializer.Serialize(data));
            _collection.InsertOne(document);
        }
    }
}