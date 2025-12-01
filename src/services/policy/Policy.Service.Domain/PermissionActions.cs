using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Policies.Domain {

    public enum PermissionActions {
        [Description("deny")]
        Deny = 0,
        [Description("permit")]
        Permit = 1
    }
}
