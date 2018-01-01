using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crate.Attributes;

namespace cratemonotest.DC
{
    [CrateTable("tweets")]
    class Tweet
    {
        //created_at	id	retweeted	source	text	user
        [CrateField(Name = "created_at", Type = typeof(DateTime))]
        public DateTime CreatedAt { get; set; }

        [CrateField(Name="id", Type=typeof(string))]
        public string Id { get; set; }

        [CrateField(Name="retweeted", Type=typeof(bool))]
        public bool Retweeted { get; set; }

        [CrateField(Name="source", Type=typeof(string))]
        public string Source { get; set; }

        [CrateField(Name="text", Type=typeof(string))]
        public string Text { get; set; }

        [CrateField(Name= "account_user", Type=typeof(User))]
        public User User { get; set; }
    }

    public class User
    {
        [CrateField(Name="created_at", Type=typeof(DateTime))]
        public DateTime CreatedAt { get; set; }

        [CrateField(Name="id", Type=typeof(string))]
        public string Id { get; set; }

        [CrateField(Name="description", Type=typeof(string))]
        public string Description { get; set; }

        [CrateField(Name="followers_count", Type=typeof(int))]
        public int FollowersCount { get; set; }

        [CrateField(Name="friends_count", Type=typeof(int))]
        public int FriendsCount { get; set; }

        [CrateField(Name="location", Type=typeof(string))]
        public string Location { get; set; }

        [CrateField(Name="statuses_count", Type=typeof(string))]
        public int StatusesCount { get; set; }

        [CrateField(Name="verified", Type=typeof(bool))]
        public bool Verified { get; set; }

    }
}
