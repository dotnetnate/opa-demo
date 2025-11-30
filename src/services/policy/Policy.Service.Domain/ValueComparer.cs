using System;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {
    /// <summary>
    /// Compares values for condition evaluation.
    /// Handles numeric, string, and DateTime comparisons.
    /// </summary>
    public interface IValueComparer {
        /// <summary>
        /// Tests equality between two values.
        /// </summary>
        bool AreEqual(object contextValue, object conditionValue);

        /// <summary>
        /// Compares two values for ordering (returns -1, 0, or 1).
        /// </summary>
        int Compare(object contextValue, object conditionValue);
    }

    public class ValueComparer : IValueComparer {
        public bool AreEqual(object contextValue, object conditionValue) {
            if (contextValue == null && conditionValue == null) return true;
            if (contextValue == null || conditionValue == null) return false;

            // Try direct equality first
            if (contextValue.Equals(conditionValue)) return true;

            // Handle numeric comparisons
            if (IsNumeric(contextValue) && IsNumeric(conditionValue)) {
                return Convert.ToDouble(contextValue) == Convert.ToDouble(conditionValue);
            }

            // String comparison (case-insensitive)
            return string.Equals(
                contextValue.ToString(),
                conditionValue.ToString(),
                StringComparison.OrdinalIgnoreCase);
        }

        public int Compare(object contextValue, object conditionValue) {
            if (contextValue == null) throw new ArgumentNullException(nameof(contextValue));
            if (conditionValue == null) throw new ArgumentNullException(nameof(conditionValue));

            // Numeric comparison
            if (IsNumeric(contextValue) && IsNumeric(conditionValue)) {
                var cv = Convert.ToDouble(contextValue);
                var cv2 = Convert.ToDouble(conditionValue);
                return cv.CompareTo(cv2);
            }

            // DateTime comparison
            if (contextValue is DateTime dt1 && conditionValue is DateTime dt2) {
                return dt1.CompareTo(dt2);
            }

            // DateTimeOffset comparison
            if (contextValue is DateTimeOffset dto1 && conditionValue is DateTimeOffset dto2) {
                return dto1.CompareTo(dto2);
            }

            // String comparison
            return string.Compare(
                contextValue.ToString(),
                conditionValue.ToString(),
                StringComparison.OrdinalIgnoreCase);
        }

        private bool IsNumeric(object value) {
            return value is sbyte || value is byte || value is short || value is ushort ||
                   value is int || value is uint || value is long || value is ulong ||
                   value is float || value is double || value is decimal;
        }
    }
}
