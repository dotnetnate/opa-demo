using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policies.Definition {
    public class PermissionDefinition {
        public required string Name { get; set; }
        public required Visibility Visibility { get; set; }
        public required List<PermissionTarget> AppliesTo { get; set; } = [];
        public required List<ConditionDefinition> AllowedConditions { get; set; } = [];
    }
}
