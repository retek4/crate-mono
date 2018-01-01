using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Crate;
using Crate.CrateSchema.Sys;
using Crate.Helpers;
using Crate.Methods;
using Crate.Types;
using cratemonotest.DC;
using cratemonotest.Utils;
using NUnit.Framework;

namespace cratemonotest
{
    [TestFixture()]
    internal class QueryTest:BaseSetup
    {
        [OneTimeSetUp]
        public void Init()
        {
            TableUtils.DropTable("ip_geopoint");
            TableUtils.CreateIpGeoTable();
            TableUtils.InsertIntoIpGeoTable();
        }

        [OneTimeTearDown]
        public void Cleaunp()
        {
            TableUtils.DropTable("ip_geopoint");
        }

        [Test()]
        public void TestWhere()
        {
            List<SysCluster> list;
            using (var conn = TestCrateConnection())
            {
                conn.Open();
                list = conn.Where<SysCluster>(t => !string.IsNullOrEmpty(t.Name));

            }
            Assert.GreaterOrEqual(1, list.Count);
        }

        [Test()]
        public void TestCount()
        {
            long c1 = 0, c2 = 0;
            using (var conn = TestCrateConnection())
            {
                conn.Open();
                c1 = conn.Count<SysCluster>(t => !string.IsNullOrEmpty(t.Name));
                c2 = conn.Count<SysCluster>();
            }
            Assert.GreaterOrEqual(c1, 1);
            Assert.GreaterOrEqual(c2, 1);
        }

        [Test()]
        public void TestInnerArray()
        {
            List<SysCluster> list;
            using (var conn = TestCrateConnection())
            {
                conn.Open();

                list = conn.Where<SysCluster>(t => t.Settings.Stats.JobsLogSize > 1);
            }
            Assert.GreaterOrEqual(list.Count, 1);
        }

        [Test()]
        public void TestScalarFormat()
        {
            List<SysCluster> list;
            using (var conn = TestCrateConnection())
            {
                conn.Open();

                list = conn.Execute<SysCluster, SysCluster>(t => from a in t
                                                                 select new SysCluster()
                                                                     {
                                                                         Id = a.Id,
                                                                         Name = string.Format("{0}_{1}_{0}", a.Name, a.Id)
                                                                     });
            }
            Assert.GreaterOrEqual(list.Count, 1);
        }

        [Test()]
        public void TestScalarSubstr()
        {
            const int fromS = 1, toS = 3;
            List<SysCluster> list;
            using (var conn = TestCrateConnection())
            {
                conn.Open();
                list = conn.Execute<SysCluster, SysCluster>(t => (from a in t
                                                                  select new SysCluster()
                                                                      {
                                                                          Id = a.Id,
                                                                          Name = a.Name.Substring(fromS),
                                                                          MasterNode = a.Name
                                                                      }));
            }
            Assert.GreaterOrEqual(1, list.Count);

            if (toS < list[0].MasterNode.Length)
                Assert.True(list[0].MasterNode.Substring(fromS == 0 ? 0 : fromS - 1) == list[0].Name);
            else
                Assert.True(list[0].MasterNode.Substring(fromS == 0 ? 0 : fromS - 1, toS) == list[0].Name);

        }

        private class Result
        {
            public string Test { get; set; }
            public int Number { get; set; }
            public int Count { get; set; }
        }

        [Test()]
        public void TestBasicExecute()
        {
            List<Result> res = null;
            using (var conn = TestCrateConnection())
            {
                conn.Open();
                res = conn.Execute<SysCluster, Result>(
                        scs =>
                            from sc in scs
                            where !string.IsNullOrEmpty(sc.Name) && sc.Name.Contains("crate")
                            select new Result() { Test = sc.Name });
            }
            Assert.NotNull(res);
            Assert.GreaterOrEqual(res.Count, 0);
            Assert.True(res[0].Test.Contains("crate"));
        }
        
        [Test()]
        public void TestRegex()
        {
            List<Result> res = null;
            using (var conn = TestCrateConnection())
            {
                conn.Open();
                res = conn.Execute<SysCluster, Result>(
                        scs =>
                            from sc in scs
                            where Regex.IsMatch(sc.Name, "([a-z]+)+")
                            select new Result() { Test = sc.Name });
            }
            Assert.NotNull(res);
            Assert.GreaterOrEqual(res.Count, 1);
            Assert.True(res[0].Test.Contains("crate"));
        }

        [Test]
        public void TestComplexExecute()
        {
            List<User> users = null;
            using (var conn = TestCrateConnection())
            {
                users = conn.Execute<Tweet, User>(tws => (from tw in tws
                                                          where tw.Text.Contains("love")
                                                          group tw by tw.User.Id into g
                                                          where g.Sum(p => p.User.FriendsCount) > 100
                                                          select new User()
                                                          {
                                                              Id = g.Key,
                                                              FriendsCount = g.Sum(t => t.User.FriendsCount),
                                                              FollowersCount = g.Count(),
                                                              CreatedAt = g.Arbitrary(t => t.User.CreatedAt)
                                                          }).OrderBy(t => t.Id).
                                                          ThenByDescending(t => t.FriendsCount).Take(5).Skip(5));
            }
            Assert.NotNull(users);
            Assert.True(5 == users.Count);
        }

        [Test()]
        public void TestScalarGeoPoint()
        {
            List<IpGeopoint> list;
            using (var conn = TestCrateConnection())
            {
                conn.Open();
                list = conn.Where<IpGeopoint>(t => !string.IsNullOrEmpty(t.IpAddr));
            }
            Assert.GreaterOrEqual(list.Count, 1);
            Assert.IsNotNull(list[0].Pin);
            Assert.IsTrue(list[0].Pin.Lat > 0);
            Assert.IsTrue(list[0].Pin.Lng > 0);
        }

        [Test]
        public void TestScalarGeoPointDistance()
        {
            List<IpGeopoint> list;
            using (var conn = TestCrateConnection())
            {
                conn.Open();
                list = conn.Where<IpGeopoint>(t => Geo.Distance(t.Pin, new GeoPoint(21, 2)) > 1);
            }
            Assert.GreaterOrEqual(list.Count, 1);
        }

        [Test]
        public void TestScalarGeoWithin()
        {
            List<IpGeopoint> list;
            using (var conn = TestCrateConnection())
            {
                conn.Open();
                list = conn.Where<IpGeopoint>(t => Geo.Within(t.Pin, new GeoPolygon(new GeoPoint(40, 20), new GeoPoint(49, 20), new GeoPoint(49, 30), new GeoPoint(40, 30), new GeoPoint(40, 20))));
            }
            Assert.GreaterOrEqual(list.Count, 1);
        }

        [Test]
        public void TestScalarMathAbsFloor()
        {
            List<Tweet> list;
            using (var conn = TestCrateConnection())
            {
                conn.Open();
                list = conn.Where<Tweet>(t => Math.Abs(t.User.FriendsCount) > 1 && Math.Floor((double)t.User.FriendsCount) <13);
            }
            Assert.GreaterOrEqual(list.Count, 1);
        }

        [Test]
        public void TestScalarMathRoundCeilSqrt()
        {
            List<Tweet> list;
            using (var conn = TestCrateConnection())
            {
                conn.Open();
                list = conn.Where<Tweet>(t =>Math.Round(t.User.FriendsCount + Math.Ceiling(Math.Sqrt(15))) == 9);
            }
            Assert.GreaterOrEqual(list.Count, 1);
        }

        [Test]
        public void TestScalarMathLnLogFloor()
        {
            List<Tweet> list;
            using (var conn = TestCrateConnection())
            {
                conn.Open();
                list = conn.Where<Tweet>(t => (Math.Log(8) + t.User.FriendsCount) > 10 && Math.Floor(Math.Log10(10000) + t.User.FriendsCount)<15);
            }
            Assert.GreaterOrEqual(list.Count, 1);
        }
        
        [Test]
        public void TestOperators()
        {
            List<Result> res = null;
            using (var conn = TestCrateConnection())
            {
                conn.Open();
                res = conn.Execute<Tweet, Result>(
                        ts =>
                            from t in ts
                            where ((1 + t.User.FriendsCount* 5 + 2) / 3 % 5+2)==3
                            select new Result() { Number = ((1 + t.User.FriendsCount) * 5 + 2) / 3 % 5+15,Count = t.User.FriendsCount});
            }
            Assert.NotNull(res);
            Assert.GreaterOrEqual(res.Count, 0);
            Assert.True(res[0].Number==17);
        }

    }
}
