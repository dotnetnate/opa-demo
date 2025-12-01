using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Elements.ApplicationModel.Extensions.AspNetCore {
    public interface IIdentityService<TIdentity> {
        Task AcquireIdentitiesFromRequest(HttpRequest request, out TIdentity originatingIdentity, out TIdentity impersonatedIdentity);
    }
}
