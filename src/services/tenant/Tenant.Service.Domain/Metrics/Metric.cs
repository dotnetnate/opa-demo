using NOCO.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Tenants.Domain {
    public class Metric {
        public required string Name { get; set; }
        public Range<DateTimeOffset>? ReportingRange { get; set; }
        public IEnumerable<MetricSample> Samples { get; set; } = new List<MetricSample>();

        public decimal Average() {
            return Samples.Average(s => s.Value);
        }
        public decimal Min() {
            return Samples.Min(s => s.Value);
        }
        public decimal Max() {
            return Samples.Max(s => s.Value);
        }        

    }
}
