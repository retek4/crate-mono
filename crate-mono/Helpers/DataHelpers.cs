using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Crate.Exceptions;
using Crate.Helpers.Cache;
using Crate.Helpers.JsonHelpers;
using Crate.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Crate.Helpers
{
    public static class DataHelpers
    {
        private static readonly DateTime UnixDt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static DataTable GetSchemaTable(this CrateDataReader reader, Type type)
        {
            var lookup = CrateFieldCacheProvider.Instance.Get(type).Values.ToDictionary(t => t.Name, t => t.Type);
            var dt = new DataTable(AttributeHelpers.GetTableData(type).Name);

            foreach (var col in reader.SqlResponse.cols)
            {
                var dtype = lookup.ContainsKey(col) ? lookup[col] : typeof(string);
                dt.Columns.Add(col, dtype);
            }

            foreach (var row in reader.SqlResponse.rows)
            {
                for (var i = 0; i < dt.Columns.Count; i++)
                {
                    if (dt.Columns[i].DataType == typeof(string))
                        continue;
                    if (dt.Columns[i].DataType == typeof(DateTime))
                    {
                        if (!(row[i] is long)) continue;
                        row[i] = UnixDt.AddMilliseconds((long)row[i]);
                    }
                    else
                    {
                        if ((row[i] is JObject) && dt.Columns[i].DataType != typeof(JObject))
                        {
                            var settings = new JsonSerializerSettings()
                            {
                                ContractResolver = new CrateJsonContractResolver(dt.Columns[i].DataType),
                                DateParseHandling = DateParseHandling.None,
                            };
                            row[i] = JsonConvert.DeserializeObject(row[i].ToString(), dt.Columns[i].DataType,
                                settings);

                        }
                        else if ((row[i] is JArray) && dt.Columns[i].DataType == typeof(GeoPoint))
                        {
                            var list = (row[i] as JArray).ToObject<List<double>>();
                            if (list.Count == 2)
                            {
                                row[i] = new GeoPoint(list[0], list[1]);
                            }
                            else
                            {
                                row[i] = null;
                            }
                        }
                    }

                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        public static List<T> ToList<T>(this CrateDataReader reader) where T : class, new()
        {
            try
            {
                var list = new List<T>();

                var dt = reader.GetSchemaTable(typeof(T));
                var attr = CrateFieldCacheProvider.Instance.Get(typeof(T));

                foreach (DataRow row in dt.Rows)
                {
                    var obj = new T();

                    var type = obj.GetType();
                    foreach (var prop in type.GetProperties())
                    {
                        try
                        {
                            var name = attr[prop.Name].Name;

                            if (!dt.Columns.Contains(name)) continue;

                            var propertyInfo = type.GetProperty(prop.Name);

                            propertyInfo.SetValue(obj, Convert.ChangeType(row[name], propertyInfo.PropertyType), null);
                        }
                        catch (Exception ex)
                        {
                            throw new CrateDbException("Failed to convert row. See inner exception.", ex);
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new CrateDbException("Failed to convert data reader to list. See inner exception.", ex);
            }
        }
    }


}
