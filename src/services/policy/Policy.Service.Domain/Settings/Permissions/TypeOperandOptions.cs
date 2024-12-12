using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Tenants.Domain.Settings.Permissions {
    internal class TypeOperandOptions {
        private static Dictionary<string, string[]> _typeOperandOptions = new Dictionary<string, string[]> {
            { "string", new string[]{ "gt", "lt", "eq", "neq", "gte", "lte" } },
            { "number", new string[]{ "gt", "lt", "eq", "neq", "gte", "lte" } },
            { "timestamp", new string[]{ "gt", "lt", "eq", "neq", "gte", "lte" }  },
            { "bool", new string[]{ "eq", "neq" } },
            { "datetime", new string[]{ "gt", "lt", "eq", "neq", "gte", "lte" } },
            { "guid", new string[]{ "eq", "neq" } }
        };

        public static string[]? GetOptionsForType(string type) {
            if (_typeOperandOptions.ContainsKey(type)) {
                return (string[]?)_typeOperandOptions[type];
            }

            return [];
        }
        public static string[] GetSupportedTypes() {
            return _typeOperandOptions.Keys.ToArray();
        }
    }
}
