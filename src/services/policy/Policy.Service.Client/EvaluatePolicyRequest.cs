using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policy.Service.Client {

    public class ActionContext {
        /// <summary>
        /// The id of the action
        /// </summary>
        public string Id { get; set; }
        public Dictionary<string, object> Context { get; set; } = new Dictionary<string, object>();
    }

    public class EvaluatePolicyRequest {
        public string                       PolicyReference { get; set; }
        public Resource                     Resource { get; set; }
        public Subject                      Subject { get; set; }                             
        public ActionContext                Action{ get; set; } = new ActionContext(); 
    }
}
