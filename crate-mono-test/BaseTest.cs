
using Crate.Helpers;
using cratemonotest;
using cratemonotest.DC;
using Dapper;
using NUnit.Framework;
using System;
using System.Data;
using System.Linq;
using Crate;

namespace Crate
{
	[TestFixture ()]
	public class BaseTest
	{
		[Test ()]
		public void TestDefaultConnection ()
		{
			var server = new CrateServer();
			Assert.AreEqual("http", server.Scheme);
			Assert.AreEqual("localhost", server.Hostname);
			Assert.AreEqual(4200, server.Port);
		}

		[Test ()]
		public void TestServerWithoutScheme ()
		{
			var server = new CrateServer("localhost:4200");
			Assert.AreEqual("http", server.Scheme);
			Assert.AreEqual("localhost", server.Hostname);
			Assert.AreEqual(4200, server.Port);
		}

		[Test ()]
		public void TestServerWithoutPort ()
		{
			var server = new CrateServer("localhost");
			Assert.AreEqual("http", server.Scheme);
			Assert.AreEqual("localhost", server.Hostname);
			Assert.AreEqual(4200, server.Port);
		}

		[Test ()]
		public void TestSelect ()
		{
			using (var conn = new CrateConnection()) {
				conn.Open();

				using (var cmd = new CrateCommand("select name from sys.cluster", conn)) {
					var reader = cmd.ExecuteReader();
					reader.Read();
					string clusterName = reader.GetString(0);
					Assert.AreEqual(clusterName, "crate");
				}
			}
		}

		[Test ()]
		public void TestSelectServerRoundrobin()
		{
			using (var conn = new CrateConnection("localhost:9999, localhost:4200")) {
				conn.Open();

				for (int i = 0; i < 10; i++) {
					string clusterName = conn.Query<string>("select name from sys.cluster").First();
					Assert.AreEqual("crate", clusterName);
				}
			}
		}

		[Test ()]
		public void TestWithDapper()
		{
			using (var conn = new CrateConnection()) {
				conn.Open();
				var clusterName = conn.Query<string>("select name from sys.cluster").First();
				Assert.AreEqual(clusterName, "crate");

				clusterName = conn.Query<string>(
					"select name from sys.cluster where name = ?", new { Name = "crate" }).First();
				Assert.AreEqual(clusterName, "crate");

				conn.Execute(
					"create table foo (id int primary key, name string) with (number_of_replicas='0-1')");
				Assert.AreEqual(1,
					conn.Execute("insert into foo (id, name) values (?, ?)", new { id = 1, name = "foo"}));

				int rowsAffected = conn.Execute(
					"insert into foo (id, name) values (?, ?), (?, ?)",
					new { id1 = 2, name1 = "zwei", id2 = 3, name2 = "drei"}
				);
				Assert.AreEqual(2, rowsAffected);
				conn.Execute("drop table foo");
			}
		}

		[Test ()]
		public void TestGetDateTime()
		{
			var reader = new CrateDataReader(new SqlResponse() { 
				rows = new object[][] { new object[] { 1388534400000 } },
				cols = new string[] { "dt" }
			});
			reader.Read();
			var dt = new DateTime(2014, 01, 01);
			Assert.AreEqual(dt, reader.GetDateTime(0));
		}
	}
}

