using System;
using Crate;
using NUnit.Framework;

namespace cratemonotest
{
	[TestFixture ()]
	internal class ConnectionTest:BaseSetup
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
			using (var conn = TestCrateConnection()) {
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

