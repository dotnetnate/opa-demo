using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Elements.ApplicationModel.Events {
    public abstract class EventHandlerBase {


        public EventHandlerBase(ILogger logger) {
            this.Logger = logger;            
        }
        public abstract Task ProcessEvent(EventBase eventToProcess);
        public ILogger Logger { get; private set; }        
    }
}
