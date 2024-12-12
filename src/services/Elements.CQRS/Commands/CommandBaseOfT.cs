using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Elements.CQRS.Commands
{
    public abstract class CommandBase<T> : CommandBase
    {
        /// <summary>
        /// Gets the payload for the command.
        /// </summary>
        public T? Payload { get; set; } = default;
    }
}
