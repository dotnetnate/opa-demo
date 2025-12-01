using CitizensFinancialGroup.Threvw.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Elements.CQRS.Events
{
    public class EventContext
    {
        public IdentityReference? InitiatingUser { get; set; }
        public IdentityReference? ImpersonatedUser { get; set; }
    }
}
