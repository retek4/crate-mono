using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crate.Attributes
{
    [AttributeUsage(AttributeTargets.Class |AttributeTargets.Struct,AllowMultiple = false)]
    public sealed class CrateTable : Attribute
    {
        public CrateTable(string name)
        {
            Name = name;
        }

        public CrateTable()
        {
            NumberOfReplicas=ReplicationType.None;
            NumberOfShards = 5;
        }

        public string Name { get; set; }

        public int NumberOfShards { get; set; }
        public ReplicationType NumberOfReplicas { get; set; }
    }
}
