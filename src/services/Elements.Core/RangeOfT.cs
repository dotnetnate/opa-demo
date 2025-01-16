using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Elements {

    /// <summary>
    /// Represents a range of values with a specified start and end.
    /// </summary>
    /// <typeparam name="T">The type of the values in the range, which must implement IComparable&lt;T&gt;.</typeparam>
    public class Range<T>
        where T : IComparable<T> {

        /// <summary>
        /// Specifies the type of the range.
        /// </summary>
        public enum RangeType {
            /// <summary>
            /// The range includes both the start and end values.
            /// </summary>
            Inclusive = 0,
            /// <summary>
            /// The range excludes both the start and end values.
            /// </summary>
            Exclusive = 1,
            /// <summary>
            /// The range excludes the start value but includes the end value.
            /// </summary>
            ExclusiveStart = 2,
            /// <summary>
            /// The range includes the start value but excludes the end value.
            /// </summary>
            ExclusiveEnd = 3
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Range{T}"/> class with the specified start and end values.
        /// </summary>
        /// <param name="start">The start value of the range.</param>
        /// <param name="end">The end value of the range.</param>
        /// <exception cref="ArgumentException">Thrown when the start value is greater than the end value.</exception>
        public Range(T start, T end) {

            if (start.CompareTo(end) > 0)
                throw new ArgumentException("End must be greater than or equal to start.");

            Start = start;
            End = end;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Range{T}"/> class with the specified start and end values and range type.
        /// </summary>
        /// <param name="start">The start value of the range.</param>
        /// <param name="end">The end value of the range.</param>
        /// <param name="type">The type of the range.</param>
        /// <exception cref="ArgumentException">Thrown when the start and end values are equal and the range type is not inclusive.</exception>
        public Range(T start, T end, RangeType type) : this(start, end) {

            if (start.CompareTo(end) == 0 && type != RangeType.Inclusive)
                throw new ArgumentException("Start and end are equal, so type must be inclusive.");

            Type = type;
        }

        /// <summary>
        /// Gets or sets the start value of the range.
        /// </summary>
        public T Start { get; set; }

        /// <summary>
        /// Gets or sets the end value of the range.
        /// </summary>
        public T End { get; set; }

        /// <summary>
        /// Gets or sets the type of the range.
        /// </summary>
        public required RangeType Type { get; set; } = RangeType.Inclusive;

    }
}
