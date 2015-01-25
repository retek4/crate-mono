using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crate.Helpers
{
    public static class EnumHelpers
    {
        public static string Replication(ReplicationType type)
        {
            switch (type)
            {
                case ReplicationType.Nine:
                    return "9";
                case ReplicationType.Eight:
                    return "8";
                case ReplicationType.Seven:
                    return "7";
                case ReplicationType.Six:
                    return "6";
                case ReplicationType.Five:
                    return "5";
                case ReplicationType.Four:
                    return "4";
                case ReplicationType.Three:
                    return "3";
                case ReplicationType.Two:
                    return "2";
                case ReplicationType.One:
                    return "1";
                case ReplicationType.Zero:
                    return "0";

                case ReplicationType.None:
                case ReplicationType.All:
                default:
                    return "0-all";
            }
        }

        public static string Index(IndexType type)
        {
            switch (type)
            {
                    case IndexType.Plain:
                    return " INDEX using plain";
                    case IndexType.FullText:
                    return " INDEX using fulltext";
                    case IndexType.FullTextWithEnglishAnalyzer:
                    return " INDEX using fulltext with (analyzer = 'english')";
            }
            return string.Empty;
        }
    }
}
