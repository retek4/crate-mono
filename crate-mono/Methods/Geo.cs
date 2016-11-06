using System;
using Crate.Types;

namespace Crate.Methods
{
    public static class Geo
    {
        private const double KilometerConst = 1.609344;
        private const double MileConst = 0.8684;

        private static double Deg2Rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }
        private static double Rad2Deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        //miles
        public static double Distance(GeoPoint p1, GeoPoint p2)
        {
            var dist = Math.Sin(Deg2Rad(p1.Lat)) * Math.Sin(Deg2Rad(p2.Lat)) + Math.Cos(Deg2Rad(p2.Lng)) * Math.Cos(Deg2Rad(p2.Lng)) * Math.Cos(Deg2Rad(p1.Lng - p2.Lng));

            dist = Rad2Deg(Math.Acos(dist));
            dist = dist * 60 * 1.1515 * MileConst;
            return dist;
        }


        public static bool Within(IGeoShape shape, IGeoShape inshape)
        {
            return false;
        }
    }
}
