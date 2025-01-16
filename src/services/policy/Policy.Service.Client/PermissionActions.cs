using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policy.Service.Client {
    public enum PermissionActions {
        [Description("deny")]
        Deny = 0,
        [Description("grant")]
        Grant = 1
    }
}
