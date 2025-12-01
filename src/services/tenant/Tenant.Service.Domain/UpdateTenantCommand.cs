using NOCO.Elements.ApplicationModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Tenants.Domain {
    public  class UpdateTenantCommand : CommandBase {
        public required Guid Id { get; set; }
        public required SystemName Name { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
