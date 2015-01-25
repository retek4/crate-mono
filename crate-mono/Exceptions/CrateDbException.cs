using System;
using System.Data.Common;

namespace Crate.Exceptions
{
	public class CrateDbException : DbException
	{
		public CrateDbException (string message) : base(message)
		{
		}
        
        public CrateDbException (string message,Exception innerException) : base(message,innerException)
		{
		}
	}
}

