using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Tenants.Service.Http.Features.Tenants.Models { 

    public class SystemName
    {
        public required string DisplayName { get; set; } = string.Empty;
        public string? SysId { get; set; } 
    }

}
