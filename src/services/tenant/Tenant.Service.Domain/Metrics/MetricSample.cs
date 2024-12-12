using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Tenants.Domain {
    public class MetricSample {
        public DateTimeOffset Timestamp { get; set; }
        public decimal Value { get; set; }
    }
}
