using CitizensFinancialGroup.Elements.CQRS.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Tenants.Domain {
    public class FindTenantByIdQuery : QueryBase {
        public Guid Id { get; set; }
    }
}
