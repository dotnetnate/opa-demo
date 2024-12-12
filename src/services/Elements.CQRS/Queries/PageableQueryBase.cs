using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Elements.CQRS.Queries
{
    public abstract class PageableQueryBase : QueryBase
    {        
        public required int PageSize { get; set; }
        public required int PageNumber { get; set; }       
    }
}
