using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crate.Exceptions
{
    public class CrateNotFound:Exception
    {
        public CrateNotFound(string message) : base(message)
        {
            
        }

        public CrateNotFound(string message, Exception innerex)
            : base(message, innerex)
        {
            
        }
    }
}
