using System;
using System.Collections.Generic;
using Crate.Attributes;
using Crate.Helpers.Cache;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Crate.Helpers.JsonHelpers
{
    public class CrateJsonContractResolver : DefaultContractResolver
    {
        private readonly Dictionary<string, CrateField> _lookup;
        public CrateJsonContractResolver(Type type)
        {
            _lookup = CrateFieldCacheProvider.Instance.Get(type);
        }
        protected override string ResolvePropertyName(string propertyName)
        {
            if (_lookup.ContainsKey(propertyName))
                return _lookup[propertyName].Name;
            return propertyName;
        }

        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (objectType == typeof(DateTime))
                return new CrateJsonDateTimeConverte();
            return base.ResolveContractConverter(objectType);
        }
    }
}
