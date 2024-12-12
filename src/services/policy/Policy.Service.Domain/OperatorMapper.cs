using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {
    public static class OperatorMapper {
        private static readonly Dictionary<Operator, string> OperatorToStringMap = new()
        {
        { Operator.GreaterThan, "gt" },
        { Operator.LessThan, "lt" },
        { Operator.Equal, "eq" },
        { Operator.NotEqual, "neq" },
        { Operator.GreaterThanOrEqual, "gte" },
        { Operator.LessThanOrEqual, "lte" }
    };

        private static readonly Dictionary<string, Operator> StringToOperatorMap = new()
        {
        { "gt", Operator.GreaterThan },
        { "lt", Operator.LessThan },
        { "eq", Operator.Equal },
        { "neq", Operator.NotEqual },
        { "gte", Operator.GreaterThanOrEqual },
        { "lte", Operator.LessThanOrEqual }
    };

        public static string ToString(Operator op) =>
            OperatorToStringMap.TryGetValue(op, out var result) ? result : throw new KeyNotFoundException();

        public static Operator FromString(string str) =>
            StringToOperatorMap.TryGetValue(str, out var result) ? result : throw new KeyNotFoundException();
    }
}
