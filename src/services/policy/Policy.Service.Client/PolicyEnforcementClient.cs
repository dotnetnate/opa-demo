using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Styra.Opa;

namespace CitizensFinancialGroup.Threvw.Policy.Service.Client {

    public class PolicyEnforcementClientOptions {
        public required Uri BaseUri { get; set; } = null!;
    }

    
    public class PolicyEnforcementClient {

        private readonly OpaClient _opaClient;        

        public PolicyEnforcementClient(IOptions<PolicyEnforcementClientOptions> options) {
            if (options == null) {
                throw new ArgumentNullException(nameof(options));
            }
            if (options.Value.BaseUri == null) {
                throw new ArgumentNullException(nameof(options.Value.BaseUri));
            }
            _opaClient = new OpaClient(options.Value.BaseUri.ToString());            
        }

        public PolicyEnforcementClient(OpaClient opaClient) {
            _opaClient = opaClient ?? throw new ArgumentNullException(nameof(opaClient));            
        }
        public async Task<PolicyEvaluationResult> Evaluate(EvaluatePolicyRequest request) {
            var wrapper = new { request.Action, request.Resource, request.Subject };
            var result = await _opaClient.evaluate<PolicyEvaluationResult>(request.PolicyReference, wrapper);
            return result;            
        }
    }
}
