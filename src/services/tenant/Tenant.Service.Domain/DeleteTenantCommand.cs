using CitizensFinancialGroup.Elements.ApplicationModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Tenants.Domain {
    public class DeleteTenantCommand : CommandBase{
        public Guid Id { get; set; }
    }
}
