using NOCO.Elements.ApplicationModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Tenants.Domain {
    public class UpdateOptionsGroupCommand : CommandBase {
        public Guid TenantId { get; set; }
        public string OptionsGroupName { get; set; }

        public object NewValue { get; set; }
    }
}
