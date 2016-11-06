using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crate;
using cratemonotest.DC;
using NUnit.Framework;

namespace cratemonotest
{
    [TestFixture()]
    internal class DataManipulationTest
    {
        [Test]
        public void TestUpdate()
        {
            using (var conn = new CrateConnection())
            {
                conn.Open();
                conn.Update<Tweet>(t => t.Id == "test",
                    new UpdateObject<Tweet, bool>(t => t.Retweeted, () => false),
                    new UpdateObject<Tweet, string>(t => t.Source, () => "me"),
                    new UpdateObject<Tweet, DateTime>(t => t.CreatedAt, () => DateTime.Now),
                    new UpdateObject<Tweet, int>(t => t.User.FollowersCount, () => 5));

            }
        }
    }
}
