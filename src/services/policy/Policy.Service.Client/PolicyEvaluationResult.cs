using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Policy.Service.Client {
    public class PolicyEvaluationResult {
        public Decisions Decision { get; set; }
        public ICollection<Advice> Advices { get; set; } = new List<Advice>();
        public ICollection<Obligation> Obligations { get; set; } = new List<Obligation>();

    }

    public class Advice {
        public required string Type { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }

    public enum Decisions {        
        Deny = 0,
        Permit = 1,
        Indeterminate = 2,
        NotApplicable = 3
    }

    public class Obligation {
        public required string Type { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }
}

    
