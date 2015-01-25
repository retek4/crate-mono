using Crate.Attributes;
using Newtonsoft.Json.Linq;

namespace Crate.CrateSchema.Information
{
    [CrateTable("information_schema.table_partitions")]
    public class TablePartition
    {
        [CrateField(Name = "schema_name", Type = typeof(string))]
        public string SchemaName { get; set; }

        [CrateField(Name = "table_name", Type = typeof(string))]
        public string TableName { get; set; }

        [CrateField(Name = "partition_ident", Type = typeof(string))]
        public string PartitionIdent { get; set; }

        [CrateField(Name = "values", Type = typeof(JObject))]
        public JObject Values { get; set; }

    }
}