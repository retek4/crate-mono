using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Crate.ExpressionTranslater
{
    /*
     *    SELECT [ ALL | DISTINCT ] * | expression [ [ AS ] output_name ] [, ...]
     *    FROM table_ident [ [AS] table_alias ]
     *    [ WHERE condition ]
     *    [ GROUP BY expression [, ...] [HAVING condition] ]
     *    [ ORDER BY expression [ ASC | DESC ] [ NULLS { FIRST | LAST } ] [, ...] ]
     *    [ LIMIT num_results ]
     *    [ OFFSET start ]
     */
    internal class Translater : BaseTranslater
    {
        private int? _skip;
        private int? _take;
        private string _whereClause = string.Empty;
        private string _havingClause = string.Empty;
        private string _selectStatement = string.Empty;
        private string _groupbyStatement = string.Empty;
        private List<Tuple<string, OrderDirection>> _orderbyStatement = new List<Tuple<string, OrderDirection>>();

        private string _tableName;
        public Translater(string tableName)
        {
            _tableName = tableName;
        }
        public override string Translate(Expression expression)
        {
            Visit(expression);

            var gt = new GroupByTranslater();
            if (_grouByExpression != null)
            {
                _groupbyStatement = gt.Translate(_grouByExpression);
            }
            if (_selectExpression != null)
            {
                var st = new SelectTranslater();
                _selectStatement=st.Translate(_selectExpression,gt.GroupByObject);
            }

            if (string.IsNullOrEmpty(_selectStatement))
                _selectStatement = "*";
            var sb = new StringBuilder();
            sb.Append("SELECT ").Append(_selectStatement).Append(" ");

            sb.Append("FROM ").Append(_tableName).Append(" ");

            if (!string.IsNullOrEmpty(_whereClause))
                sb.Append("WHERE ").Append(_whereClause).Append(" ");
            if (!string.IsNullOrEmpty(_groupbyStatement))
                sb.Append("GROUP BY ").Append(_groupbyStatement).Append(" ");
            if (!string.IsNullOrEmpty(_havingClause))
                sb.Append("HAVING ").Append(_havingClause).Append(" ");
            if (_orderbyStatement.Count > 0)
            {
                var order = "";
                foreach (var item in _orderbyStatement)
                {
                    switch (item.Item2)
                    {
                        case OrderDirection.Asc:
                            order += " " + item.Item1 + "  asc,";
                            break;
                        case OrderDirection.Desc:
                            order += " " + item.Item1 + "  desc,";
                            break;
                        default: 
                            order += " " + item.Item1 + ",";
                            break;
                    }
                }

                sb.Append("ORDER BY ").Append(order.Trim(',')).Append(" ");
            }
            if (_take.HasValue)
            {
                sb.Append("LIMIT ").Append(_take.Value).Append(" ");
            }
            if (_skip.HasValue)
            {
                sb.Append("OFFSET ").Append(_skip.Value).Append(" ");
            }
            return sb.ToString().Trim();
        }

        private Expression _selectExpression = null;
        private Expression _grouByExpression = null;

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "Take":
                    if (ParseTakeExpression(m))
                    {
                        return Visit(m.Arguments[0]);
                    }
                    break;
                case "Skip":
                    if (ParseSkipExpression(m))
                    {
                        return Visit(m.Arguments[0]);
                    }
                    break;
                case "ThenBy":
                case "OrderBy":
                    var s = (new OrderByTranslater()).Translate(m.Arguments[1]);
                    foreach (var item in s.Split(','))
                    {
                        _orderbyStatement.Insert(0, Tuple.Create(item.Trim(), OrderDirection.Asc));
                    }
                    return Visit(m.Arguments[0]);
                case "ThenByDescending":
                case "OrderByDescending":
                    var sd = (new OrderByTranslater()).Translate(m.Arguments[1]);
                    foreach (var item in sd.Split(','))
                    {
                        _orderbyStatement.Insert(0, Tuple.Create(item.Trim(), OrderDirection.Desc));
                    }
                    return Visit(m.Arguments[0]);
                case "Select":
                    {
                        _selectExpression=m.Arguments[1];
                        return Visit(m.Arguments[0]);
                    }
                case "GroupBy":
                    {
                        if (!string.IsNullOrEmpty(_whereClause))
                        {
                            _havingClause = _whereClause;
                            _whereClause = null;
                        }
                        _grouByExpression = m.Arguments[1];
                        return Visit(m.Arguments[0]);
                    }
                case "Where":
                    {
                        _whereClause = (new WhereTranslater()).Translate(m.Arguments[1]);
                        return Visit(m.Arguments[0]);
                    }
            }

            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }
        private bool ParseTakeExpression(MethodCallExpression expression)
        {
            var sizeExpression = (ConstantExpression)expression.Arguments[1];

            int size;
            if (!int.TryParse(sizeExpression.Value.ToString(), out size)) return false;
            _take = size;
            return true;
        }

        private bool ParseSkipExpression(MethodCallExpression expression)
        {
            var sizeExpression = (ConstantExpression)expression.Arguments[1];

            int size;
            if (!int.TryParse(sizeExpression.Value.ToString(), out size)) return false;
            _skip = size;
            return true;
        }
    }
}
