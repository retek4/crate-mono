using System;
using System.Collections.Generic;
using Crate;
using Crate.Attributes;
using Crate.CrateSchema.Information;
using Crate.Helpers;
using cratemonotest.DC;
using cratemonotest.Utils;
using NUnit.Framework;

namespace cratemonotest
{
    [TestFixture()]
    internal class DataDefinitionTest:BaseSetup
    {
        [SetUp]
        public void Init()
        {
            TableUtils.DropTable("ip_geopoint");
            TableUtils.CreateIpGeoTable();
            TableUtils.InsertIntoIpGeoTable();
        }

        [TearDown]
        public void Cleaunp()
        {
            TableUtils.DropTable("ip_geopoint");
        }

        [Test]
        public void TestTableCreate()
        {
            CrateDataReader reader = null;
            List<Table> schemadata = null;

            var tabledesc = (CrateTable)Attribute.GetCustomAttribute(typeof(CreatTestTable), typeof(CrateTable));
            Assert.IsNotNull(tabledesc);
            
            TableUtils.DropTable(tabledesc.Name);

            using (var conn = TestCrateConnection())
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
            using (var conn = TestCrateConnection())
            {
                conn.Open();
                res=conn.CheckIfTableExists<IpGeopoint>();
            }
            Assert.IsTrue(res);
        }
    }
}
