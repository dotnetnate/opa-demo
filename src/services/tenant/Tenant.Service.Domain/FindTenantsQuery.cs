using CitizensFinancialGroup.Elements.ApplicationModel.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Tenants.Domain {
    public class FindTenantsQuery : PageableQueryBase{
        public string? SysIdFilter { get; set; }
        public string? NameFilter { get; set; }
        public string? DescriptionFilter { get; set; }
    }
}
