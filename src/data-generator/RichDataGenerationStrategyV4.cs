using System.Runtime.CompilerServices;
using System.Text.Json;

namespace data_generator
{

    public class RichDataGenerationStrategyV4 : IDataGenerationStrategy
    {                

        private readonly IPersistence _persistence;

        public RichDataGenerationStrategyV4(IPersistence persistence)
        {
            _persistence = persistence;
        }

        public string PolicyName => "cfg.authz.rich.v4";

        public void GenerateSampleData(int numberOfResources, int numberOfSubjectsPerResource, int numberOfPermissionsPerSubject, string outputFolder)
        {
            Guid tenantId = Guid.NewGuid();

            for (int i = 0; i < numberOfResources; i++)
            {
                var resourceId = Guid.NewGuid().ToString();
                var policyId = resourceId + "-policy";

                var resource = new
                {
                    identifier = resourceId,
                    authority = "https://myapp.com/types/account"                    
                };

                var rules = new List<object>();

                for (int j = 0; j < numberOfSubjectsPerResource; j++)
                {
                    var subjectId = Guid.NewGuid().ToString();

                    var subject = new
                    {
                        identifier = subjectId,                        
                        authority = "https://auth0.com/"
                    };

                    var privileges = new Dictionary<string, object>();

                    for (int k = 0; k < numberOfPermissionsPerSubject; k++)
                    {
                        var permitPrivilege = new
                        {
                            effect = "grant",
                            conditions = new List<object>
                    {
                        new
                        {
                            contextAttributePath = "amount",
                            @operator = "eq",
                            value = 500
                        }
                    }
                        };

                        var denyPrivilege = new
                        {
                            effect = "deny"
                        };

                        privileges[$"PERMITTED_PERMISSION_{k}"] = permitPrivilege;
                        privileges[$"DENIED_PERMISSION_{k}"] = denyPrivilege;
                    }

                    var rule = new
                    {
                        subject,
                        privileges
                    };

                    rules.Add(rule);
                }

                var completeObject = new PolicyWrapper
                {
                    id = Guid.NewGuid(), 
                    tenantId = tenantId,
                    resource = resource,
                    rules = rules
                };

                _persistence.Persist(completeObject);
            }

            _persistence.Flush();
        }
    }
}
