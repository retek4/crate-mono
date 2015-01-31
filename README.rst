==========
Crate-mono
==========

Crate-mono is a Mono/.NET client driver implementing the ADO.NET interface for
`crate data <https://crate.io>`_

Added linq translator  supporting:
    - scalar functions 
    - aggregations
    - group by clause
    - arithmetic operators
    - geo point functions
    - inner/ nested objects and arrays  

Sample
======    
        
 1. Simple query 

    ::

        List<SysCluster> list;
        using (var conn = new CrateConnection())
        {
            conn.Open();
            list = conn.Where<SysCluster>(t => t.Settings.Stats.JobsLogSize > 1);
        }    

 
 2. Query with translated query

    ::
        
            List<User> users = null;
            using (var conn = new CrateConnection())
            {
                 users = conn.Execute<Tweet, User>(tws => 
                 (from tw in tws where tw.Text.Contains("love")
                                  group tw by tw.User.Id into g
                                  where g.Sum(p => p.User.FriendsCount) > 100
                                  select new User()
                                  { 
                                      Id = g.Key,
                                      FriendsCount = g.Sum(t => t.User.FriendsCount),
                                      FollowersCount = g.Count(),
                                      CreatedAt = g.Arbitrary(t => t.User.CreatedAt)
                                  }).OrderBy(t => t.Id).
                                    ThenByDescending(t => t.FriendsCount).
                                    Take(5).
                                    Skip(5));
            }
   
 
    ::

        SELECT user['id'] AS id, SUM(user['friends_count']) AS friends_count, 
        COUNT(*) AS followers_count, ARBITRARY(user['created_at']) AS created_at 
        FROM tweets 
        WHERE text LIKE '%love%' GROUP BY user['id'] 
        HAVING (SUM(user['friends_count']) > 100) 
        ORDER BY  id  asc, friends_count  desc LIMIT 5 OFFSET 5
