using NOCO.Elements.ApplicationModel.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Tenants.Domain {
    public class FindTenantSettingsQuery : QueryBase {
        public required Guid TenantId { get; set; }
        public required string SettingName { get; set; }
    }
}
