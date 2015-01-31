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
