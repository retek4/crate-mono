using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crate;
using Crate.Attributes;
using Crate.CrateSchema.Information;
using Crate.Helpers;
using cratemonotest.DC;
using NUnit.Framework;

namespace cratemonotest
{
    [TestFixture()]
    public class DataDefinitionTest
    {

        [Test]
        public void TestTableCreate()
        {
            CrateDataReader reader = null;
            List<Table> schemadata = null;

            var tabledesc = (CrateTable)Attribute.GetCustomAttribute(typeof(CreatTestTable), typeof(CrateTable));
            Assert.IsNotNull(tabledesc);
            
            CrateQueryTest.DropTable(tabledesc.Name);

            using (var conn = new CrateConnection())
            {
                conn.Open();
               conn.CreateTable<CreatTestTable>();

               var sql = "SELECT * FROM information_schema.tables WHERE table_name= '" + tabledesc.Name + "'";
               using (var cmd = new CrateCommand(sql, conn))
               {
                   reader = (CrateDataReader)cmd.ExecuteReader();
                   schemadata = reader.ToList<Table>();

               }
            }
            Assert.IsNotNull(reader);
            Assert.IsNotNull(schemadata);
            Assert.Greater(schemadata.Count,0);
            Assert.AreEqual(EnumHelpers.Replication(tabledesc.NumberOfReplicas), schemadata[0].NumberOfReplicas);
            Assert.AreEqual(tabledesc.NumberOfShards, schemadata[0].NumberOfShards);
        }

        [Test]
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
