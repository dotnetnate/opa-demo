using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Tenants.Service.Http.Features.Settings.Models.Permissions
{
    internal class TypeOperandOptions
    {
        private static Hashtable _typeOperandOptions = new Hashtable {
            { "string", new string[]{ "gt", "lt", "eq", "neq" } },
            { "number", new string[]{ "gt", "lt", "eq", "neq" } },
            { "timestamp", new string[]{ "gt", "lt", "eq", "neq" } },
            { "bool", new string[]{ "eq", "neq" } },
            { "datetime", new string[]{ "gt", "lt", "eq", "neq" } },
            { "guid", new string[]{ "eq", "neq" } }
        };

        public static string[]? GetOptionsForType(string type)
        {
            if (_typeOperandOptions.ContainsKey(type))
            {
                return (string[]?)_typeOperandOptions[type];
            }
            return [];
        }
    }
}
