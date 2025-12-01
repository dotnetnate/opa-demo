using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Policy.Service.Client {
    public enum InputSources {
        [Description("transactionContext")]
        TransactionContext = 0,
        [Description("data")]
        Data = 1,
        [Description("jwt")]
        JWT = 2,
        [Description("subject")]
        Subject = 3,
        [Description("resource")]
        Resource = 4
    }
}
