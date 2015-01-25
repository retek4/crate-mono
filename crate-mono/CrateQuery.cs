using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Crate.ExpressionTranslater;
using Crate.Helpers;

namespace Crate
{
    public static class CrateQuery
    {
        public static List<T> Where<T>(this CrateConnection cnn, Expression<Func<T, bool>> exp) where T : class,new()
        {
            List<T> ret;

            var table = AttributeHelpers.GetTableData(typeof(T));

            var swhere = "SELECT * FROM " + table.Name + " WHERE " + (new WhereTranslater()).Translate(exp);
            using (var cmd = new CrateCommand(swhere, cnn))
            {
                var reader = (CrateDataReader)cmd.ExecuteReader();
                ret = reader.ToList<T>();
            }

            return ret;
        }

        public static long Count<T>(this CrateConnection cnn, Expression<Func<T, bool>> exp = null) where T : class,new()
        {
            long ret = 0;
            var table = AttributeHelpers.GetTableData(typeof (T));

            var swhere = "SELECT count(*) FROM " + table.Name;
           
            if(exp!=null)
                swhere+=" WHERE " + (new WhereTranslater()).Translate(exp);
           
            using (var cmd = new CrateCommand(swhere, cnn))
            {
                var reader = (CrateDataReader) cmd.ExecuteReader();
                if (reader.Read())
                {
                    ret = reader.GetLong(0);
                }
            }

            return ret;
        }

        public static List<TOut> Execute<T, TOut>(this CrateConnection cnn, Expression<Func<List<T>, IEnumerable<TOut>>> exp)
            where T : class,new()
            where TOut : class,new()
        {
            List<TOut> ret;

            var table = AttributeHelpers.GetTableData(typeof(T));

            var sql = (new Translater(table.Name)).Translate(exp);
            using (var cmd = new CrateCommand(sql, cnn))
            {
                var reader = (CrateDataReader)cmd.ExecuteReader();
                ret = reader.ToList<TOut>();
            }

            return ret;
        }

    }
}
