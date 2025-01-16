using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policies.Definition {

    public class PermissionTarget {
        public required string Scope { get; set; }
        public required ICollection<string> ObjectTypes { get; set; } = new List<string>();

    }
}
