using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Elements.CQRS.Queries
{
    public abstract class QueryBase
    {
        public ClaimsIdentity OriginatingUser { get; set; }
        public ClaimsIdentity ImpersonatedUser { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
