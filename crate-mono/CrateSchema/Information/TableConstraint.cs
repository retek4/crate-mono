using Crate.Attributes;

namespace Crate.CrateSchema.Information
{
    [CrateTable("information_schema.table_constraints")]
    public class TableConstraint
    {
        [CrateField(Name = "schema_name", Type = typeof(string))]
        public string SchemaName { get; set; }

        [CrateField(Name = "table_name", Type = typeof(string))]
        public string TableName { get; set; }

        [CrateField(Name = "constraint_name", Type = typeof(string))]
        public string Name { get; set; }

        [CrateField(Name = "constraint_type", Type = typeof(string))]
        public string ConstraintType { get; set; }

    }
}