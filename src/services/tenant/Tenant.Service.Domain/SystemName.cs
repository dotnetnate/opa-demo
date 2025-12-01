using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Tenants.Domain
{
    public class SystemName
    {
        public required string DisplayName { get; set; } = string.Empty;
        public string? SysId { get; set; } 
    }

}
