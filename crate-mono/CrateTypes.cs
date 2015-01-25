using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crate
{
    public struct CrateTypes
    {
        public const string Boolean = "boolean";
        public const string String = "string";
        public const string Integer = "integer";
        public const string Long = "long";
        public const string Short = "short";
        public const string Double = "double";
        public const string Float = "float";
        public const string Byte = "byte";
        public const string Ip = "ip";
        public const string TimeStamp = "timestamp";
        public const string GeoPoint = "geo_point";
        public const string Object = "object";//object(dynamic)
        public const string ObjectStrict = "object(strict)";
        public const string ObjectDynamic = "object(dynamic)";
        public const string ObjectIgnored = "object(ignored)";
        public const string Array = "array"; 
    }
}
