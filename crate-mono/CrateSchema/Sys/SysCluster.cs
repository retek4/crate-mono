using System;
using Crate.Attributes;
using Newtonsoft.Json.Linq;

namespace Crate.CrateSchema.Sys
{
    [CrateTable("sys.cluster")]
    public class SysCluster
    {
        [CrateField(Name = "id", Type=typeof(Guid))]
        public Guid Id { get; set; }

        [CrateField(Name = "name", Type=typeof(string))]
        public string Name { get; set; }

        [CrateField(Name = "master_node", Type=typeof(string))]
        public string MasterNode { get; set; }

        [CrateField(Name = "settings", Type=typeof(Settings))]
        public Settings Settings { get; set; }

    }

    public class Settings
    {
        [CrateField(Name = "cluster", Type=typeof(JObject))]
        public JObject Cluster { get; set; }

        [CrateField(Name = "indices", Type=typeof(JObject))]
        public JObject Indices { get; set; }

        [CrateField(Name = "discovery", Type=typeof(JObject))]
        public JObject Discovery { get; set; }

        [CrateField(Name = "stats", Type = typeof(Stat))]
        public Stat Stats { get; set; }
    }

    public class Stat
    {
        [CrateField(Name = "enabled", Type = typeof(bool))]
        public bool Enabled { get; set; }

        [CrateField(Name = "jobs_log_size", Type = typeof(int))]
        public int JobsLogSize { get; set; }

        [CrateField(Name = "operations_log_size", Type = typeof(int))]
        public int OperationsLogSize { get; set; }
    }
}
