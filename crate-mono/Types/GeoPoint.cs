using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crate.Types
{
    public class GeoPoint : IGeoShape
    {
        public double Lat { get; private set; }
        public double Lng { get; private set; }

        public GeoPoint(double lat, double lng)
        {
            Lat = lat;
            Lng = lng;
        }
    }
}
