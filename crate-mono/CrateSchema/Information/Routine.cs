using Crate.Attributes;

namespace Crate.CrateSchema.Information
{
    [CrateTable("information_schema.routines")]
    public class Routine
    {
        [CrateField(Name = "routine_name", Type = typeof(string))]
        public string Name { get; set; }

        [CrateField(Name = "routine_type", Type = typeof(string))]
        public string Type { get; set; }

    }
}