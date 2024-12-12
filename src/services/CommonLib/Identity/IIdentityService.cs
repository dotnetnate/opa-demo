using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Common.Identity {
    public interface IIdentityService<TIdentity,TContext> {
        Task<(TIdentity? originatingIdentity, TIdentity? impersonatedIdentity)> AcquireIdentitiesFromRequest(TContext context);
    }
}
