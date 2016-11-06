using System;

namespace Crate.Exceptions
{
    public class CrateException:Exception
    {
        public CrateException(string message) : base(message)
        {
            
        }
        
        public CrateException(string message,Exception innerex) : base(message,innerex)
        {
            
        }
    }
}
