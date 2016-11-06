using System;
using Crate;
using NUnit.Framework;

namespace cratemonotest
{
    public class CrateQueryTest
    {

        [SetUp]
        public void Init()
        {
            //DropTable("ip_geopoint");
            //CreateIpGeoTable();
            //InsertIntoIpGeoTable();
        }

        [TearDown]
        public void Cleaunp()
        {
            //DropTable("ip_geopoint");
        }

        internal static void DropTable(string name)
        {
            try
            {
                using (var conn = new CrateConnection())
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

        private static void CreateIpGeoTable()
        {
            try
            {
                using (var conn = new CrateConnection())
                {
                    conn.Open();

                    using (
                        var cmd =
                            new CrateCommand(
                                "CREATE TABLE ip_geopoint ( id integer, ip_addr string, pin geo_point,dateutc timestamp) with (number_of_replicas = 0)",
                                conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                
            }
        }
        private static void InsertIntoIpGeoTable()
        {
            using (var conn = new CrateConnection())
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
