using System;
using System.Runtime.Caching;

namespace Crate.Helpers.Cache
{
    public abstract class DataCache
    {
        protected string CacheName { get; private set; }

        private static readonly object SafetyLock = new object();
#if (NET20 || NET35)
        protected Dictionary<string, object> Cache;
#else
        protected MemoryCache Cache;
#endif

        protected DataCache(string name = "CrateDataCaching")
        {
            CacheName = name;
#if (NET20 || NET35)
            Cache = new Dictionary<string, object>();
#else
            Cache = new MemoryCache(name);
#endif
        }

        public virtual void Set(string key, object value)
        {
            lock (SafetyLock)
            {
#if (NET20 || NET35)
                if (Cache.ContainsKey(key))
                    Cache[key] = value;
                else
                    Cache.Add(key, value);
#else
                Cache.Add(key, value, DateTimeOffset.MaxValue);
#endif
            }
        }

        public virtual void Remove(string key)
        {
            lock (SafetyLock)
            {
                Cache.Remove(key);
            }
        }

        public virtual object Get(string key)
        {
            lock (SafetyLock)
            {
#if (NET20 || NET35)
                object res=null;
                if (Cache.ContainsKey(key))
                    res=Cache[key];
#else
                var res = Cache[key];
#endif

                if (res != null)
                {
                    Cache.Remove(key);
                }

                return res;
            }
        }

    }
}
