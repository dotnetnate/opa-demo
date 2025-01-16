using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizensFinancialGroup.Threvw.Policies.Domain;
using MongoDB.Bson.IO;
using CitizensFinancialGroup.Elements;

namespace CitizensFinancialGroup.Threvw.Policies.Infrastructure {
    public class RuleBsonSerializer : IBsonSerializer<Rule> {
        public Type ValueType => typeof(Rule);

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Rule value) {
            context.Writer.WriteStartDocument();

            // Serialize the Subject property
            context.Writer.WriteName("subject");
            BsonSerializer.Serialize(context.Writer, value.Subject);

            context.Writer.WriteName("privileges");
            context.Writer.WriteStartDocument();

            // Serialize the Privileges as properties
            foreach (var privilege in value.Privileges) {
                if (string.IsNullOrEmpty(privilege.PermissionName)) {
                    throw new BsonSerializationException("PrivilegeName cannot be null or empty.");
                }

                context.Writer.WriteName(privilege.PermissionName);
                context.Writer.WriteStartDocument();

                context.Writer.WriteName("effect");
                context.Writer.WriteString(Enum.GetName(typeof(PermissionActions), privilege.Effect));                

                if (privilege.Conditions != null) {
                    context.Writer.WriteName("conditions");
                    BsonSerializer.Serialize(context.Writer, privilege.Conditions);
                }

                if (privilege.ValidityPeriod != null) {
                    context.Writer.WriteName("validityPeriod");
                    BsonSerializer.Serialize(context.Writer, privilege.ValidityPeriod);
                }

                context.Writer.WriteEndDocument();
            }

            context.Writer.WriteEndDocument();

            context.Writer.WriteEndDocument();
        }

        public Rule Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) {
            Subject parsedSubject = null;
            List<Privilege> parsedPrivileges = new List<Privilege>();

            context.Reader.ReadStartDocument(); // { start of rule document

            while (context.Reader.State != BsonReaderState.Type || context.Reader.ReadBsonType() != BsonType.EndOfDocument) {
                var elementName = context.Reader.ReadName();

                if (elementName == "subject") {
                    // Deserialize the Subject
                    parsedSubject = BsonSerializer.Deserialize<Subject>(context.Reader);
                }
                else if (elementName == "privileges") {
                    // Start reading the privileges document
                    context.Reader.ReadStartDocument(); // { start of privileges document

                    while (context.Reader.State != BsonReaderState.Type || context.Reader.ReadBsonType() != BsonType.EndOfDocument) {
                        // Read privilege name (key)
                        var privilegeName = context.Reader.ReadName();



                        var privilege = new Privilege { PermissionName = privilegeName };

                        // Start reading the privilege details
                        context.Reader.ReadStartDocument();

                        if( context.Reader.FindElement("effect")) {
                            privilege.Effect = (PermissionActions)Enum.Parse(
                                typeof(PermissionActions),
                                context.Reader.ReadString(),
                                true // Ignore case
                            );
                        }
                        try {

                            // Read "conditions" if present
                            if (context.Reader.FindElement("conditions")) {
                                privilege.Conditions = BsonSerializer.Deserialize<List<Condition>>(context.Reader);
                            }

                            // Read "validityPeriod" if present
                            if (context.Reader.FindElement("validityPeriod")) {
                                privilege.ValidityPeriod = BsonSerializer.Deserialize<Range<DateTimeOffset>>(context.Reader);
                            }
                        }
                        catch {

                        }
                        finally{
                            context.Reader.ReadEndDocument();
                        }

                        //context.Reader.ReadEndDocument(); // End privilege details

                        parsedPrivileges.Add(privilege);
                    }

                    context.Reader.ReadEndDocument(); // End privileges document
                }
            }

            context.Reader.ReadEndDocument(); // End rule document

            return new Rule {
                Subject = parsedSubject,
                Privileges = parsedPrivileges
            };
        }

        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) {
            return Deserialize(context, args);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value) {
            Serialize(context, args, (Rule)value);
        }
    }
}
