using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Policies.Definition {
    public enum InputSources {
        TransactionContext = 0,
        Data = 1,
        JWT = 2,
        Subject = 3,
        Resource = 4
    }
}
