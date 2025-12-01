using System;
using System.Collections.Generic;
using System.Text;

namespace NOCO.Elements.ApplicationModel.CQRS.Commands {
    public class CommandResultHandlerService<TResult> : ICommandResultHandlerService<TResult> {

        private readonly IEnumerable<ICommandResultHandler<TResult>> m_handlers;

        public CommandResultHandlerService(IEnumerable<ICommandResultHandler<TResult>> handlers) {
            m_handlers = handlers;
        }

        public TResult? HandleCommandResult(CommandResult result) {
            foreach( var handler in m_handlers) {

                var retVal = handler.HandleCommandResult(result);

                if(retVal != null) {
                    return retVal;
                }
            }

            return default;
        }

    }
}
