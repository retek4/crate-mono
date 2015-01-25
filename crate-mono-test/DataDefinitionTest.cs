using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crate;
using Crate.CrateSchema.Information;
using cratemonotest.DC;
using NUnit.Framework;

namespace cratemonotest
{
    [TestFixture()]
    public class DataDefinitionTest
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            Helpers.TestSetupCleanupHelper.DropTable("ip_geopoint");
            Helpers.TestSetupCleanupHelper.CreateIpGeoTable();
        }
        
        [Test()]
        public void TestTableCreation()
        {
            var res = false;
            using (var conn = new CrateConnection())
            {
                conn.Open();
                conn.CreateTable<CreatTestTable>();
                res = conn.CheckIfTableExists<CreatTestTable>();
            }
            if(res)
                Helpers.TestSetupCleanupHelper.DropTable("testtable");
            Assert.IsTrue(res);
        } 
        
        [Test()]
        public void TestTableExists()
        {
            bool res = false;
            using (var conn = new CrateConnection())
            {
                conn.Open();
                res=conn.CheckIfTableExists<IpGeopoint>();
            }
            Assert.IsTrue(res);
        }
    }
}
