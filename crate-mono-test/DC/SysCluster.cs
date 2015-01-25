using System;
using Crate.Attributes;
using Newtonsoft.Json.Linq;

namespace cratemonotest.DC
{
    [CrateTable("sys.cluster")]
    public class SysCluster
    {
        [CrateField(Name = "id")]
        public Guid Id { get; set; }

        [CrateField(Name = "name")]
        public string Name { get; set; }

        [CrateField(Name = "master_node")]
        public string MasterNode { get; set; }

        [CrateField(Name = "settings")]
        public Settings Settings { get; set; }

    }

    public class Settings
    {
        [CrateField(Name = "cluster")]
        public JObject Cluster { get; set; }

        [CrateField(Name = "indices")]
        public JObject Indices { get; set; }

        [CrateField(Name = "discovery")]
        public JObject Discovery { get; set; }

        [CrateField(Name = "stats")]
        public Stat Stats { get; set; }
    }

    public class Stat
    {
        [CrateField(Name = "enabled")]
        public bool Enabled { get; set; }

        [CrateField(Name = "jobs_log_size")]
        public int JobsLogSize { get; set; }

        [CrateField(Name = "operations_log_size")]
        public int OperationsLogSize { get; set; }
    }
}
