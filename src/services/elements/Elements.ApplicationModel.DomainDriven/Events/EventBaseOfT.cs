using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Elements.CQRS.Events
{
    public abstract class EventBase<T> : EventBase
    {

        public EventBase(EventContext context, T model) : base(context)
        {
            Model = model;
        }
        public T Model { get; set; }
    }
}
