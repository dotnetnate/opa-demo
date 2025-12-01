using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Policies.Definition {
    public class ConditionDefinition {
        
        private string _attributePath;        
        private string _dataType;

        public InputSources InputSource { get; set; }

        public required string ContextAttributePath {
            get => _attributePath;
            set => _attributePath = value ?? throw new ArgumentNullException(nameof(ContextAttributePath));            
        }

        public required string DataType {
            get => _dataType;
            set => _dataType = value ?? throw new ArgumentNullException(nameof(DataType));            
        }

        public bool Required { get; set; }
    }
}
