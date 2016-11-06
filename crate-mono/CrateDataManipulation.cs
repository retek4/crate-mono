using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Crate.CrateSchema.Information;
using Crate.CrateSchema.Sys;
using Crate.ExpressionTranslater;
using Crate.Helpers;
using Crate.Helpers.Cache;
using Crate.Types;
using Newtonsoft.Json.Linq;

namespace Crate
{
    public interface IUpdateObject
    {
        Type ValueType { get; set; }

    }

    internal static class UpdateObjectHelper
    {
        public static string ToSql<T>(this IUpdateObject val)
        {
            var t = val.ValueType;
            if (t == typeof(bool))
            { var cval = val as UpdateObject<T, bool>; }
            else if (t == typeof(long))
            { var cval = val as UpdateObject<T, int>; }
            else if (t == typeof(Int16))
            { var cval = val as UpdateObject<T, short>; }
            else if (t == typeof(float))
            { var cval = val as UpdateObject<T, float>; }
            else if (t == typeof(double))
            { var cval = val as UpdateObject<T, double>; }
            else if (t == typeof(string))
            { var cval = val as UpdateObject<T, string>; }
            else if (t == typeof(byte))
            { var cval = val as UpdateObject<T, byte>; }
            else if (t == typeof(DateTime))
            { var cval = val as UpdateObject<T, DateTime>; }
            else if (t == typeof(Guid))
            { var cval = val as UpdateObject<T, Guid>; }
            else if (t == typeof(GeoPoint))
            { var cval = val as UpdateObject<T, GeoPoint>; }
            else if (t == typeof(JObject))
            {
                var cval = val as UpdateObject<T, JObject>;
            }
            return null;
        }
    }

    public class UpdateObject<T, TVal> : IUpdateObject
    {
        public UpdateObject(Expression<Func<T, TVal>> s, Expression<Func<TVal>> v)
        {
            Set = s;
            Value = v;
            ValueType = typeof(TVal);
        }
        public Expression<Func<T, TVal>> Set { get; private set; }
        public Expression<Func<TVal>> Value { get; private set; }
        public Type ValueType { get; set; }
    }

    public static class CrateDataManipulation
    {
        public static void Insert<T>(this CrateConnection conn, T data)
        {

        }

        public static void Update<T>(this CrateConnection conn, Expression<Func<T, bool>> condition, params IUpdateObject[] vals)
        {
            var td = AttributeHelpers.GetTableData(typeof(T));
            var fields = CrateFieldCacheProvider.Instance.Get(typeof(T));
            /* foreach (var item in fields)
             {
                 var con=(new WhereTranslater()).Translate(condition);
             }*/
            var con = (new WhereTranslater()).Translate(condition);
            foreach (var val in vals)
            {
                var t = val.ValueType;
                /* if (t == typeof(string))
                 {
                     var cval = val as UpdateObject<T, string>;
                     var s = new SelectTranslater().Translate(cval.Set);
                 }
                 else if (t == typeof(bool))
                 {
                     var cval = val as UpdateObject<T, bool>;
                     var s = new SelectTranslater().Translate(cval.Set);
                 }
                 else if (t == typeof(DateTime))
                 {
                     var cval = val as UpdateObject<T, DateTime>;
                     var s = new SelectTranslater().Translate(cval.Set);
                 }
                 else
                 {
                    
                 }*/
                var cval = val.ToSql<T>();


            }
        }

        public static void GetMyProperties(object obj)
        {
            foreach (PropertyInfo pinfo in obj.GetType().GetProperties())
            {
                var getMethod = pinfo.GetGetMethod();
                if (getMethod.ReturnType.IsArray)
                {
                    var arrayObject = getMethod.Invoke(obj, null);
                    foreach (object element in (Array)arrayObject)
                    {
                        foreach (PropertyInfo arrayObjPinfo in element.GetType().GetProperties())
                        {
                            Console.WriteLine(arrayObjPinfo.Name + ":" + arrayObjPinfo.GetGetMethod().Invoke(element, null).ToString());
                        }
                    }
                }
            }
        }

        public static int Delete<T>(this CrateConnection conn, Expression<Func<T, bool>> exp)
        {
            var table = AttributeHelpers.GetTableData(typeof(T));

            var swhere = "DELETE FROM " + table.Name + " WHERE " + (new WhereTranslater()).Translate(exp);
            using (var cmd = new CrateCommand(swhere, conn))
            {
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
