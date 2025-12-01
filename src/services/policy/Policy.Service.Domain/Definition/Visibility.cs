using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Policies.Definition {
    public class Visibility {
        public required bool Enabled { get; set; }
        public DateTimeOffset? ActivationDate { get; set; }
        public DateTimeOffset? DeactivationDate { get; set; }
    }
}
