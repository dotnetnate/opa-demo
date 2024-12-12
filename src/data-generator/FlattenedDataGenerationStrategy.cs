using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace data_generator
{
    public class FlattenedDataGenerationStrategy : IDataGenerationStrategy
    {
        public string PolicyName => "cfg.authz.flattened";

        private readonly IPersistence _persistence;

        public FlattenedDataGenerationStrategy(IPersistence persistence)
        {
            _persistence = persistence;
        }


        public void GenerateSampleData(int numberOfResources, int numberOfSubjectsPerResource, int numberOfPermissionsPerSubject, string outputFolder)
        {
            for (int i = 0; i < numberOfResources; i++)
            {
                var resourceId = Guid.NewGuid().ToString();

                var resource = new
                {
                    resourceId,
                    resourceType = "account",
                    attributes = new List<object>()
                };

                var rules = new List<object>();

                for (int j = 0; j < numberOfSubjectsPerResource; j++)
                {
                    var subjectId = Guid.NewGuid().ToString();

                    var subject = new
                    {
                        identifier = subjectId,
                        type = "user",
                        authority = "https://auth0.com/"
                    };

                    for (int k = 0; k < numberOfPermissionsPerSubject; k++)
                    {
                        var permitPrivilege = new
                        {
                            effect = "permit",
                            conditions = new List<object>
                            {
                                new
                                {
                                    attribute = "amount",
                                    @operator = "eq",
                                    value = 500
                                }
                            }
                        };

                        var denyPrivilege = new
                        {
                            effect = "deny"
                        };

                        var permitRule = new
                        {
                            subject,
                            privileges = new Dictionary<string, object>
                            {
                                { $"PERMITTED_PERMISSION_{k}", permitPrivilege }
                            }
                        };

                        var denyRule = new
                        {
                            subject,
                            privileges = new Dictionary<string, object>
                            {
                                { $"DENIED_PERMISSION_{k}", denyPrivilege }
                            }
                        };

                        rules.Add(permitRule);
                        rules.Add(denyRule);
                    }
                }

                var completeObject = new
                {
                    resource,
                    rules
                };

                _persistence.Persist(completeObject);
            }

            _persistence.Flush();
        }
    }
}