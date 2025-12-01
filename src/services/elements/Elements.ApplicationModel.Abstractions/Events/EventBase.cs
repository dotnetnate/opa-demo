using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Elements.ApplicationModel.Events
{
    public abstract class EventBase
    {
        public EventBase(EventContext context)
        {
            Context = context;
        }
        public Guid CorrelationId { get; set; }
        public EventContext Context { get; set; }
        public abstract string EventName { get; }
    }
}
