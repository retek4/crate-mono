using Crate.Attributes;
using Crate.CrateSchema.Sys;

namespace Crate.CrateSchema.Information
{
    [CrateTable("information_schema.tables")]
    public class Table
    {
        [CrateField(Name = "schema_name", Type = typeof(string))]
        public string SchemaName { get; set; }

        [CrateField(Name = "table_name", Type = typeof(string))]
        public string Name { get; set; }

        [CrateField(Name = "number_of_shards", Type = typeof(int))]
        public int NumberOfShards { get; set; }

        [CrateField(Name = "number_of_replicas", Type = typeof(string))]
        public string NumberOfReplicas { get; set; }

    }
}
