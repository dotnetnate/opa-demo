using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Elements.ApplicationModel.Commands { 
    public class CommandBaseWithIdentity<TIdentity> : CommandBase {
        public TIdentity? OriginatingUser { get; set; }
        public TIdentity? ImpersonatedUser { get; set; }
    }
}
