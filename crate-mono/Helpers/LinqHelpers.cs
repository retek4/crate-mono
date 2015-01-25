using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crate.Helpers
{
    public static class LinqHelpers
    {
        //returns an undefined value of all the values in the argument column. If used outside crate it will be NULL
        public static TOut Arbitrary<T, TOut>(this IEnumerable<T> source, Func<T, TOut> selector)
        {
            return default(TOut);
        }

    }
}
