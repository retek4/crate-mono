using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crate;
using Crate.Attributes;
using Crate.Types;

namespace cratemonotest.DC
{
    [CrateTable(Name = "testtable", NumberOfReplicas = ReplicationType.All, NumberOfShards = 3)]
    public class CreatTestTable
    {
        [CrateField(Name = "id",PrimaryKey = true)]
        public Guid Id { get; set; }

        [CrateField(Name = "name", PrimaryKey = true,Index = IndexType.Plain)]
        public string Name { get; set; }

        [CrateField(Name = "pin")]
        public GeoPoint Pin { get; set; }

        [CrateField(Name = "ip_addr",CrateType = CrateTypes.Ip)]
        public string Ip { get; set; }

        [CrateField(Name = "textlist")]
        public List<string> TextList{ get; set; }


        [CrateField(Name = "datalist")]
        public List<CrateInnerObject> Datalist { get; set; }


    }

    public class CrateInnerObject
    {
        [CrateField(Name = "df", Index = IndexType.FullTextWithEnglishAnalyzer)]
        public string DataField { get; set; }

        [CrateField(Name = "dl")]
        public GeoPoint DataLocation { get; set; }

        [CrateField(Name = "innertextlist")]
        public List<string> TextList { get; set; }
    }
}
