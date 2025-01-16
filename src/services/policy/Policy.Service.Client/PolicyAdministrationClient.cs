using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Options;



#region Validators


#endregion

#region PolicyClient
namespace CitizensFinancialGroup.Threvw.Policy.Service.Client {


    public class PolicyAdministrationClientOptions {
        public required Uri BaseUri { get; set; }
    }



    public class PolicyAdministrationClient {
        private readonly HttpClient _httpClient;

        public PolicyAdministrationClient(IOptions<PolicyAdministrationClientOptions> options) {
            if(options == null) {
                throw new ArgumentNullException(nameof(options));
            }
            if(options.Value.BaseUri == null) {
                throw new ArgumentNullException(nameof(options.Value.BaseUri));
            }
            _httpClient = new HttpClient {
                BaseAddress = options.Value.BaseUri
            };  
        }

        public PolicyAdministrationClient(HttpClient httpClient) {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IEnumerable<Policy>> FindPolicies(FindPoliciesQuery query) {
            var response = await _httpClient.GetAsync($"/api/policies?{ToQueryString(query)}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<Policy>>();
        }

        public async Task<Policy> GetPolicyByResource(Resource resource) {
            var response = await _httpClient.GetAsync($"/api/policies/{resource.Authority} / {resource.Identifier}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Policy>();
        }

        public async Task CreatePolicy(Policy policy) {
            var response = await _httpClient.PostAsJsonAsync("/api/policies", policy);
            response.EnsureSuccessStatusCode();
        }

        public async Task<PolicyEvaluationResult> Evaluate(EvaluatePolicyRequest request) {
            return new PolicyEvaluationResult();
        }

        public async Task UpdatePolicy(Resource resource, Policy policy) {
            var response = await _httpClient.PutAsJsonAsync($"/api/policies/{resource.Authority} / {resource.Identifier }", policy);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeletePolicy(Resource resource) {
            var response = await _httpClient.DeleteAsync($"/api/policies/{resource.Authority}/{resource.Identifier}");
            response.EnsureSuccessStatusCode();
        }

        private static string ToQueryString(object obj) {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj) != null
                             select $"{Uri.EscapeDataString(p.Name)}={Uri.EscapeDataString(p.GetValue(obj).ToString())}";
            return string.Join("&", properties);
        }
    }

    public class FindPoliciesQuery {
        public string ResourceId { get; set; } // Optional filter for resource ID
        public string ResourceType { get; set; } // Optional filter for resource type
        public string SubjectId { get; set; } // Optional filter for subject ID
        public int PageNumber { get; set; } = 1; // Pagination: page number
        public int PageSize { get; set; } = 10; // Pagination: page size
    }

    #endregion

}