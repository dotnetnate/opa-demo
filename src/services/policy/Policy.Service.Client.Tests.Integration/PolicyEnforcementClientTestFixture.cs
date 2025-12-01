using Castle.Core.Logging;
using NOCO.Threvw.Policy.Service.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Styra.Opa;
using System.Security.Authentication.ExtendedProtection;

namespace Policy.Service.Client.Tests.Integration {
    [TestClass]
    public sealed class PolicyEnforcementClientTestFixture {
        [TestMethod]
        public async Task TestMethod1() {



            PolicyEnforcementClient client = new(CreateOpaClient());

            var x = await client.Evaluate(new EvaluatePolicyRequest                 
            { 
                    Action = new ActionContext { Id = "PERMITTED_PERMISSION_0", Context = new Dictionary<string, object> { { "amount", 500 } } },                
                    PolicyReference = "cfg/authz/policy/remotemongo2",
                    Subject = new Subject { Identifier = "153e2ad6-e65c-4c5c-8583-45a6166887d7", Authority = "https://auth0.com/" }, 
                    Resource = new Resource { Identifier = "14ae3aa7-4be1-4880-9e29-f5461e9ea293", Authority = "https://myapp.com/types/account" } 
                });

            Assert.AreEqual(Decisions.Permit, x.Decision );

        }

        private OpaClient CreateOpaClient() {

            JsonSerializerSettings settings = new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            string url = "http://127.0.0.1:8181/";

            using var loggerFactory = LoggerFactory.Create(builder => {                
                builder
                    .AddDebug()
                    .SetMinimumLevel(LogLevel.Debug);
            });
            ILogger<OpaClient> logger = loggerFactory.CreateLogger<OpaClient>();

            var client = new OpaClient(url, logger: logger, jsonSerializerSettings: settings);

            return client;
        }
    }
}
