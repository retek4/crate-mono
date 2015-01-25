using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Crate.Helpers
{

    public static class TypeHelpers
    {
        private static Dictionary<Type, ExtendedTypeCode> _codes = new Dictionary<Type, ExtendedTypeCode>()
        {
            {typeof(Guid),ExtendedTypeCode.Guid},
            {typeof(Int16),ExtendedTypeCode.Int16},
            {typeof(Int32),ExtendedTypeCode.Int32},
            {typeof(Int64),ExtendedTypeCode.Int64},
            {typeof(double),ExtendedTypeCode.Double},
            {typeof(float),ExtendedTypeCode.Float},
            {typeof(decimal),ExtendedTypeCode.Decimal},
            {typeof(bool),ExtendedTypeCode.Boolean},
            {typeof(string),ExtendedTypeCode.String},
            {typeof(byte),ExtendedTypeCode.Byte},
            {typeof(char),ExtendedTypeCode.Char},
            {typeof(DateTime),ExtendedTypeCode.DateTime},
            {typeof(JObject),ExtendedTypeCode.JObject},
        };

        public static ExtendedTypeCode GetExtendedTypeCode(this Type t)
        {
            return _codes.ContainsKey(t) ? _codes[t] : ExtendedTypeCode.Empty;
        }
    }
}
