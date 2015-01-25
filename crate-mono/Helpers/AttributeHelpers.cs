using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Crate.Attributes;

namespace Crate.Helpers
{
    internal static class AttributeHelpers
    {
        internal static CrateTable GetTableData(Type t)
        {
            var tabledesc = (CrateTable)Attribute.GetCustomAttribute(t, typeof(CrateTable));

            return tabledesc ?? new CrateTable(t.Name);
        }
        internal static Dictionary<string, CrateField> GetTableFields(Type t)
        {
            var ret = new Dictionary<string, CrateField>();

            var uniqueLookupCrateFieldName = new List<string>();

            var props =t.GetProperties();
            foreach (var prop in props)
            {
                var propName = prop.Name;
                var objAttr = prop.GetCustomAttributes(true).FirstOrDefault(attr => attr is CrateField);
                var fielddesc = objAttr != null ? (CrateField)objAttr : new CrateField(propName);

                if (fielddesc.Type == null)
                    fielddesc.Type = prop.PropertyType;

                if (uniqueLookupCrateFieldName.Contains(fielddesc.Name))
                    throw new Exception("Crate field name not uniqe "+ fielddesc.Name);//TODO create ex

                ret.Add(propName, fielddesc);
            }
            return ret;
        }
    }
}
