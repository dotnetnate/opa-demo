using CitizensFinancialGroup.Elements.Security.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Elements.ApplicationModel.Events
{
    public class EventContext
    {
        public IdentityReference? InitiatingUser { get; set; }
        public IdentityReference? ImpersonatedUser { get; set; }
    }
}
