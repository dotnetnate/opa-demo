using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policy.Service.Client {
    /// <summary>
    /// Represents a privilege with a name, visibility settings, and associated conditions.
    /// </summary>
    public class Privilege {
        /// <summary>
        /// Gets or sets the name of the permission (e.g., "PERMISSION_1").
        /// </summary>
        public required string PermissionName { get; set; }
        /// <summary>
        /// Gets or sets the list of conditions associated with the privilege.
        /// </summary>
        public List<Condition> Conditions { get; set; } = new();
        /// <summary>
        /// Gets or sets the validity period of the privilege rule. If this is null, the rule is always valid,
        /// otherwise it is only checked as long as the execution time occurs within the validity period.
        /// </summary>
        public Range<DateTimeOffset>? ValidityPeriod { get; set; }
        /// <summary>
        /// Gets or sets the action to perform with respect to the permission.
        /// </summary>
        public PermissionActions Action { get; set; }
        /// <summary>
        /// Indicates if the validity period is passed. When new rules are added with invalid validity periods (i.e. periods that end before today)
        /// they will not pass validation.
        /// </summary>
        /// <returns>Returns <c>true</c> if the period has lapsed, <c>false</c> otherwise.</returns>
        public bool HasValidityPeriodLapsed() {
            return ValidityPeriod != null && DateTimeOffset.Now.CompareTo(ValidityPeriod.End) > 0;
        }
    }
}
