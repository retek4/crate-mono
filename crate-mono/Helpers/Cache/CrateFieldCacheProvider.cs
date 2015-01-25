using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crate.Attributes;

namespace Crate.Helpers.Cache
{
    internal class CrateFieldCacheProvider : DataCache
    {
        private CrateFieldCacheProvider():base("CrateFieldAttribeCache")
        {

        }

        #region Singleton

#if (NET20 || NET35)
        private class Nested
        {
            static Nested()
            {
            }

            internal static readonly CrateFieldCacheProvider Instance = new CrateFieldCacheProvider();
        }

        public static CrateFieldCacheProvider Instance { get { return Nested.Instance; } }
#else
        private static readonly Lazy<CrateFieldCacheProvider> Lazy = new Lazy<CrateFieldCacheProvider>(() => new CrateFieldCacheProvider());
        public static CrateFieldCacheProvider Instance { get { return Lazy.Value; } }
#endif


        #endregion

        public Dictionary<string, CrateField> Get(Type t)
        {
            var ret = (Dictionary<string, CrateField>) Get(t.FullName);
            if (ret == null)
            {
                ret=AttributeHelpers.GetTableFields(t);
                Set(t.FullName,ret);
            }
            return ret;
        }

    }
}
