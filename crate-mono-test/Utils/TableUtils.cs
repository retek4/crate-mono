using System;
using Crate;

namespace cratemonotest.Utils
{
    internal static class TableUtils
    {
        public static void DropTable(string name)
        {
            try
            {
                using (var conn = BaseSetup.TestCrateConnection())
                {
                    using (var cmd = new CrateCommand("drop table " + name, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                //Bad solution for drop if exists
            }
        }

        public static void CreateIpGeoTable()
        {
            using (var conn = BaseSetup.TestCrateConnection())
            {
                conn.Open();

                using (var cmd = new CrateCommand("CREATE TABLE ip_geopoint ( id integer, ip_addr string, pin geo_point,dateutc timestamp) with (number_of_replicas = 0)", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public static void InsertIntoIpGeoTable()
        {
            using (var conn = BaseSetup.TestCrateConnection())
            {
                conn.Open();

                using (var cmd = new CrateCommand("insert into ip_geopoint (id,ip_addr,pin,dateutc) values ('1','192.168.2.1',[ 46.5, 26.4 ],'1969-12-06')", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
