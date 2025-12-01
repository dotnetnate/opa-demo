using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Elements.Security.Identity {
    public class IdentityContext<TIdentity>{
        public TIdentity? ServiceIdentity { get;set;}
        public TIdentity? OriginatingUser { get;set;}
        public TIdentity? ImpersonatedUser { get;set;}
    }
}