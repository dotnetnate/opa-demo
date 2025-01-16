using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Elements.Security.Identity {
    public class NullIdentittyService : IIdentityService<ClaimsIdentity, object> {
        public async Task<(ClaimsIdentity? originatingIdentity, ClaimsIdentity? impersonatedIdentity)> AcquireIdentitiesFromRequest(object context) {
            return (null, null);
        }
    }
}
