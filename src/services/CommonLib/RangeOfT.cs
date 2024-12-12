using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Common {
    public class Range<T>
        where T : IComparable<T> {

        public enum RangeType {
            Inclusive = 0,
            Exclusive = 1,
            ExclusiveStart = 2,
            ExclusiveEnd = 3
        }

        public Range(T start, T end) {

            if(start.CompareTo(end) > 0)
                throw new ArgumentException("End must be greater than or equal to start.");
            
            Start = start;
            End = end;
        }
        public Range(T start, T end, RangeType type) : this(start,end) {

            if( start.CompareTo(end) == 0 && type != RangeType.Inclusive)
                throw new ArgumentException("Start and end are equal, so type must be inclusive.");

            Type = type;
        }
        public T Start { get; set; }
        public T End { get; set; }

        public required RangeType Type { get; set; } = RangeType.Inclusive;

    }
}
