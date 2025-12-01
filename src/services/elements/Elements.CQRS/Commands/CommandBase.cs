using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Elements.CQRS.Commands
{
    public abstract class CommandBase
    {
        public string? SourceTraceId { get; set; }
    }
}
