using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Crate.Attributes
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class CrateField:Attribute
    {
        public string Name { get; set; }
        
        public bool PrimaryKey { get; set; }
        
        public string CrateType { get; set; }
        public Type Type { get; set; }
        
        public IndexType Index { get; set; }
        
        public CrateField()
        {
            Index = IndexType.None;
            Type = null;
        }

        public CrateField(string name)
        {
            Name = name;
            Type = null;
        }

    }
}
