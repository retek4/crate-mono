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
    [CrateTable( Name="ip_geopoint" ,NumberOfReplicas = ReplicationType.Five,NumberOfShards = 3)]
    public class IpGeopoint
    {
        [CrateField(Name="id")]
        public int Id { get; set; }

        [CrateField(Name="ip_addr")]
        public string IpAddr { get; set; }

        [CrateField(Name = "pin")]
        public GeoPoint Pin { get; set; }
    }
}
