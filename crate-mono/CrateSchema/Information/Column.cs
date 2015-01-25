using Crate.Attributes;

namespace Crate.CrateSchema.Information
{
    [CrateTable("information_schema.columns")]
    public class Column
    {
        [CrateField(Name = "schema_name", Type = typeof(string))]
        public string SchemaName { get; set; }

        [CrateField(Name = "table_name", Type = typeof(string))]
        public string TableName { get; set; }

        [CrateField(Name = "column_name", Type = typeof(string))]
        public string Name { get; set; }

        [CrateField(Name = "ordinal_position", Type = typeof(int))]
        public int OrdinalPosition { get; set; }

        [CrateField(Name = "data_type", Type = typeof(string))]
        public string DataType { get; set; }

    }
}