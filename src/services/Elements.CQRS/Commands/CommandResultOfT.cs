
using System;
using System.Collections.Generic;
using System.Text;

namespace CitizensFinancialGroup.Elements.CQRS.Commands {
    public class CommandResult<TResult> : CommandResult {      
        /// <summary>
        /// The result data from the execution of the command.
        /// </summary>
        public TResult?         Result { get; set; }


       
    }

    
}
