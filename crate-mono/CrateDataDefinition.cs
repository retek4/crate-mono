using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crate.CrateSchema.Information;
using Crate.Helpers;
using Crate.Helpers.Cache;
using Crate.Types;
using Newtonsoft.Json.Linq;

namespace Crate
{
    public static class CrateDataDefinition
    {
        public static void CreateTable<T>(this CrateConnection conn)
        {
             conn.CreateTable(typeof(T));
        }

        public static void CreateTable(this CrateConnection conn, Type t)
        {
            var tabledata = AttributeHelpers.GetTableData(t);
            var tablecols = CrateFieldCacheProvider.Instance.Get(t);

            var sb = new StringBuilder();
            sb.Append("CREATE TABLE ").Append(tabledata.Name).Append(" ( ");
            sb.Append(LoopObject(t));

            var pk = tablecols.Where(tc => tc.Value.PrimaryKey).Select(tc => tc.Value.Name).ToArray();
            if (pk.Any())
                sb.Append("primary key (").Append(string.Join(" ,", pk)).Append(" )");
            else
            sb.Remove(sb.Length - 2, 2);
            sb.Append(" ) ");

            sb.Append(string.Format("clustered into {0} shards ", tabledata.NumberOfShards));
            sb.Append(string.Format(" with (number_of_replicas = '{0}') ", EnumHelpers.Replication(tabledata.NumberOfReplicas)));

            var sql = sb.ToString();
            using (var cmd = new CrateCommand(sql, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private static string LoopObject(Type t)
        {
            var tablecols = CrateFieldCacheProvider.Instance.Get(t);
            var sb = new StringBuilder();

            foreach (var col in tablecols)
            {
                sb.Append(BuildFields(col.Value.Name, col.Value.Type, col.Value.CrateType,col.Value.Index));
            }
            return sb.ToString();
        }

        private static string BuildFields(string name, Type t, string crateType,IndexType indextype)
        {
            var sb = new StringBuilder();

            if (string.IsNullOrEmpty(crateType))
                crateType = GetCrateType(t);
            if (string.IsNullOrWhiteSpace(crateType))
                return string.Empty;
            string index = string.Empty;
            switch (crateType)
            {
                case CrateTypes.String:
                case CrateTypes.Integer:
                case CrateTypes.Double:
                case CrateTypes.Float:
                case CrateTypes.Byte:
                case CrateTypes.Boolean:
                case CrateTypes.Long:
                case CrateTypes.TimeStamp:
                case CrateTypes.Short:
                {
                    index = EnumHelpers.Index(indextype);
                    break;
                }
                case CrateTypes.Array:
                    {
                        var tl = t.GetGenericArguments();
                        if (tl.Any())
                        {
                            if (tl[0]!=typeof(string) && tl[0].GetInterface(typeof(IEnumerable<>).FullName) != null)
                                throw new NotSupportedException("Crate exception! Arrays cannot be nested.");

                            var inner = BuildFields(string.Empty, tl[0], null,IndexType.None);
                            crateType += "( " + inner.Trim().Trim(',') + " )";
                        }
                        else
                            return string.Empty;
                        break;
                    }
                case CrateTypes.Object:
                case CrateTypes.ObjectDynamic:
                case CrateTypes.ObjectIgnored:
                case CrateTypes.ObjectStrict:
                    {
                        var inner = LoopObject(t);
                        crateType += " AS (" + inner.Trim().Trim(',') + " ) ";
                        break;
                    }
            }

            sb.Append(name).Append(" ").Append(crateType).Append(index).Append(", ");

            return sb.ToString();
        }


        private static string GetCrateType(Type t)
        {
            string ct = null;

            //TODO 

            if (t == typeof(Int32))
            { ct = CrateTypes.Integer; }
            else if (t == typeof(Int64))
            { ct = CrateTypes.Long; }
            else if (t == typeof(Int16))
            { ct = CrateTypes.Short; }
            else if (t == typeof(float))
            { ct = CrateTypes.Float; }
            else if (t == typeof(double))
            { ct = CrateTypes.Double; }
            else if (t == typeof(string))
            { ct = CrateTypes.String; }
            else if (t == typeof(byte))
            { ct = CrateTypes.Byte; }
            else if (t == typeof(DateTime))
            { ct = CrateTypes.TimeStamp; }
            else if (t == typeof(Guid))
            { ct = CrateTypes.String; }
            else if (t == typeof(GeoPoint))
            { ct = CrateTypes.GeoPoint; }
            else if (t.GetInterface(typeof(IEnumerable<>).FullName) != null)
            {
                ct = CrateTypes.Array;
            }
            else
            {
                ct = t == typeof(JObject) ? CrateTypes.ObjectDynamic : CrateTypes.ObjectStrict;
            }
            return ct;
        }

        public static bool CheckIfTableExists<T>(this CrateConnection conn)
        {

            var tabledata = AttributeHelpers.GetTableData(typeof(T));
            return conn.CheckIfTableExists(tabledata.Name);
        }
        public static bool CheckIfTableExists(this CrateConnection conn,string name)
        {
            var ret = new List<Table>();

            var sql = "SELECT * FROM information_schema.tables WHERE table_name= '" + name + "'";
            using (var cmd = new CrateCommand(sql, conn))
            {
                var reader = (CrateDataReader)cmd.ExecuteReader();
                ret = reader.ToList<Table>();
            }

            return ret.Count > 0;
        }

    }
}
