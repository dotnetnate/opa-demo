using CitizensFinancialGroup.Elements.ApplicationModel.Queries;
using CitizensFinancialGroup.Threvw.Policies.Domain;

namespace CitizensFinancialGroup.Threvw.Policies.Application {
    public class FindPoliciesQuery : QueryBase {
        public Resource? ResourceFilter { get; set; }
        public Subject? SubjectFilter { get; set; }        
        public int PageNumber { get; set; } = 1; // The page number for pagination
        public int PageSize { get; set; } = 10; // The number of items per page
    }
}
