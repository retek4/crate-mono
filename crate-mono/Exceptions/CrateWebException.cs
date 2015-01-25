using System;
using System.Net;

namespace Crate.Exceptions
{
    public class CrateWebException:WebException
    {
        public CrateWebException(string message): base(message)
        {
        }
        
        public CrateWebException(string message,Exception innerex): base(message,innerex)
        {
        }
    }
}
