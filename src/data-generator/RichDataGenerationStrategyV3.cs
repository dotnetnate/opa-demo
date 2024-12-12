using System.Text.Json;

namespace data_generator
{

    public class RichDataGenerationStrategyV3 : IDataGenerationStrategy
    {


        private readonly IPersistence _persistence;

        public RichDataGenerationStrategyV3(IPersistence persistence)
        {
            _persistence = persistence;
        }

        public string PolicyName => "cfg.authz.rich.v3";

        public void GenerateSampleData(int numberOfResources, int numberOfSubjectsPerResource, int numberOfPermissionsPerSubject, string outputFolder)
        {
            for (int i = 0; i < numberOfResources; i++)
            {
                var resourceId = Guid.NewGuid().ToString();
                var policyId = resourceId + "-policy";

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

                    var privileges = new Dictionary<string, object>();

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
