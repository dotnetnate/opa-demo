//using CitizensFinancialGroup.Threvw.Tenants.Domain;
//using MongoDB.Bson.Serialization.Serializers;
//using MongoDB.Bson.Serialization;
//using MongoDB.Bson;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CitizensFinancialGroup.Threvw.Tenants.Infrastructure {
//    public class TenantSettingsSerializer : SerializerBase<TenantSettings> {

//        private readonly Dictionary<string, Type> _propertyTypeMap = new();

//        public TenantSettingsSerializer()  {
//            InitializeMemberMap();
//        }        

//        public override TenantSettings Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) {
//            var document = BsonDocumentSerializer.Instance.Deserialize(context, args);
//            var settings = new TenantSettings();

//            var settingsType = typeof(TenantSettings);

//            // iterate over each key in the _propertyTypeMap dictionary
//            foreach (var property in _propertyTypeMap) {
//                // get the value of the property from the document

//                if(document.TryGetValue(property.Key, out var element)) {

//                    var deserializedValue = BsonSerializer.Deserialize(element.AsBsonDocument, property.Value);                    

//                    settingsType.GetProperty(property.Key)
//                                ?.SetValue(settings, deserializedValue);
//                }
//            }

//            return settings;
//        }

//        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TenantSettings value) {
//            var document = new BsonDocument();

//            var settingsType = typeof(TenantSettings);

//            // iterate over the properties of the TenantSettings class
//            foreach (var property in _propertyTypeMap) {
//                // get the value of the property
//                var propertyValue = settingsType.GetProperty(property.Key)?.GetValue(value);
//                // add the property name and value to the document
//                document[property.Key] = value.ToBsonDocument();
//            }
//            BsonDocumentSerializer.Instance.Serialize(context, document);
//        }

        

//        private void InitializeMemberMap() {
//            // get the list of all properties on the TenantSettings class
//            var properties = typeof(TenantSettings).GetProperties();
//            foreach (var property in properties) {
//                // get the type of the property
//                var propertyType = property.PropertyType;
//                // get the name of the property
//                var propertyName = property.Name.ToLowerInvariant();

//                // add the property name and type to the dictionary 
//                _propertyTypeMap.Add(propertyName, propertyType);
//            }

//        }
//    }
//}
