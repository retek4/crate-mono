using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using Crate.Attributes;
using Crate.Exceptions;
using Crate.Helpers;
using Crate.Helpers.Cache;
using Crate.Types;

namespace Crate.ExpressionTranslater
{
    internal abstract class BaseTranslater : ExpressionVisitor
    {
        protected StringBuilder SqlExpressionBuilder;

        protected bool ConstantWithoutAp = false;

        protected BaseTranslater()
        {
            SqlExpressionBuilder = new StringBuilder();
        }

        public abstract string Translate(Expression expression);
        public override Expression Visit(Expression exp)
        {
            return exp == null ? null : base.Visit(exp);
        }
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "get_Item":
                    {
                        Visit(m.Object);
                        SqlExpressionBuilder.Append("[");
                        Visit(m.Arguments[0]);
                        SqlExpressionBuilder.Append("]");
                        return m;
                    }

                #region Aggregation
                case "Sum":
                    {
                        if (m.Method.DeclaringType == null || m.Method.DeclaringType.Name != "Enumerable") break;
                        SqlExpressionBuilder.Append("SUM(");
                        Visit(m.Arguments[1]);
                        SqlExpressionBuilder.Append(")");
                        return Visit(m.Arguments[0]);
                    }
                case "Average":
                    {
                        if (m.Method.DeclaringType == null || m.Method.DeclaringType.Name != "Enumerable") break;
                        SqlExpressionBuilder.Append("AVG(");
                        Visit(m.Arguments[1]);
                        SqlExpressionBuilder.Append(")");
                        return Visit(m.Arguments[0]);
                    }
                case "Min":
                    {
                        if (m.Method.DeclaringType == null || m.Method.DeclaringType.Name != "Enumerable") break;
                        SqlExpressionBuilder.Append("MIN(");
                        Visit(m.Arguments[1]);
                        SqlExpressionBuilder.Append(")");
                        return Visit(m.Arguments[0]);
                    }
                case "Max":
                    {
                        if (m.Method.DeclaringType == null || m.Method.DeclaringType.Name != "Enumerable") break;
                        SqlExpressionBuilder.Append("MAX(");
                        Visit(m.Arguments[1]);
                        SqlExpressionBuilder.Append(")");
                        return Visit(m.Arguments[0]);
                    }
                case "Count":
                    {
                        if (m.Method.DeclaringType == null || m.Method.DeclaringType.Name != "Enumerable") break;
                        SqlExpressionBuilder.Append("COUNT(");
                        if (m.Arguments.Count > 1)
                            Visit(m.Arguments[1]);
                        else
                            SqlExpressionBuilder.Append("*");
                        SqlExpressionBuilder.Append(")");
                        return Visit(m.Arguments[0]);
                    }
                case "Arbitrary":
                    {
                        SqlExpressionBuilder.Append("ARBITRARY(");
                        Visit(m.Arguments[1]);
                        SqlExpressionBuilder.Append(")");
                        return Visit(m.Arguments[0]);
                    }
                #endregion

                #region Scalar
                case "Format":
                    {
                        if (m.Method.DeclaringType == null || m.Method.DeclaringType.Name.ToLower() != "string") break;
                        var regex = new Regex(@"({[^}]*})", RegexOptions.IgnoreCase);
                        var matches = regex.Matches(m.Arguments[0].ToString());

                        var args = new List<Expression>();

                        if (m.Arguments[1] is NewArrayExpression)
                        {
                            args.AddRange((m.Arguments[1] as NewArrayExpression).Expressions);
                        }
                        else
                        {
                            for (var i = 1; i < m.Arguments.Count; i++)
                            {
                                args.Add(m.Arguments[i]);
                            }
                        }

                        var stringformatorder = matches.Cast<Match>().Select(g => g.Value.Replace("{", "").Replace("}", "")).ToList();
                        var crateformatstring = regex.Replace(m.Arguments[0].ToString(), "%s").Replace("\"", "");


                        SqlExpressionBuilder.Append("FORMAT('").Append(crateformatstring).Append("',");
                        for (var i = 0; i < stringformatorder.Count; i++)
                        {
                            var index = 0;
                            if (!int.TryParse(stringformatorder[i], out index) || index > args.Count) continue;

                            Visit(args[index]);
                            if (i != stringformatorder.Count - 1)
                                SqlExpressionBuilder.Append(",");
                        }
                        SqlExpressionBuilder.Append(")");

                        return m;
                    }
                case "Substring":
                    {
                        if (m.Method.DeclaringType == null || m.Method.DeclaringType.Name.ToLower() != "string") break;
                        SqlExpressionBuilder.Append("SUBSTR(");
                        Visit(m.Object);
                        SqlExpressionBuilder.Append(",");
                        Visit(m.Arguments[0]);
                        SqlExpressionBuilder.Append(",");

                        if (m.Arguments.Count == 1)
                            SqlExpressionBuilder.Append(int.MaxValue);
                        else
                            Visit(m.Arguments[1]);
                        SqlExpressionBuilder.Append(")");
                        return m;

                    }
                case "Distance":
                    {
                        SqlExpressionBuilder.Append("DISTANCE(");
                        Visit(m.Arguments[0]);
                        SqlExpressionBuilder.Append(",");
                        Visit(m.Arguments[1]);
                        SqlExpressionBuilder.Append(")");
                        return m;
                    }
                case "Within":
                    {
                        SqlExpressionBuilder.Append("WITHIN(");
                        Visit(m.Arguments[0]);
                        SqlExpressionBuilder.Append(",");
                        Visit(m.Arguments[1]);
                        SqlExpressionBuilder.Append(")");
                        return m;
                    }
                case "Abs":
                    {
                        if (m.Method.DeclaringType == null || m.Method.DeclaringType.Name != "Math") break;
                        SqlExpressionBuilder.Append("ABS(");
                        Visit(m.Arguments[0]);
                        SqlExpressionBuilder.Append(")");
                        return m;
                    }
                case "Ceiling":
                    {
                        if (m.Method.DeclaringType == null || m.Method.DeclaringType.Name != "Math") break;
                        SqlExpressionBuilder.Append("CEIL(");
                        Visit(m.Arguments[0]);
                        SqlExpressionBuilder.Append(")");
                        return m;
                    }
                case "Floor":
                    {
                        if (m.Method.DeclaringType == null || m.Method.DeclaringType.Name != "Math") break;
                        SqlExpressionBuilder.Append("FLOOR(");
                        Visit(m.Arguments[0]);
                        SqlExpressionBuilder.Append(")");
                        return m;
                    }
                case "Log10":
                    {
                        if (m.Method.DeclaringType == null || m.Method.DeclaringType.Name != "Math") break;
                        SqlExpressionBuilder.Append("LOG(");
                        Visit(m.Arguments[0]);
                        SqlExpressionBuilder.Append(")");
                        return m;
                    }
                case "Log":
                    {
                        if (m.Method.DeclaringType == null || m.Method.DeclaringType.Name != "Math") break;
                        if (m.Arguments.Count < 2)
                        {
                            SqlExpressionBuilder.Append("LN(");
                            Visit(m.Arguments[0]);
                            SqlExpressionBuilder.Append(")");
                        }
                        else
                        {
                            SqlExpressionBuilder.Append("LN(");
                            Visit(m.Arguments[0]);
                            SqlExpressionBuilder.Append(", ");
                            Visit(m.Arguments[1]);
                            SqlExpressionBuilder.Append(")");
                        }
                        return m;
                    }
                case "Sqrt":
                    {
                        if (m.Method.DeclaringType == null || m.Method.DeclaringType.Name != "Math") break;
                        SqlExpressionBuilder.Append("SQRT(");
                        Visit(m.Arguments[0]);
                        SqlExpressionBuilder.Append(")");
                        return m;
                    }
                case "Round":
                    {
                        if (m.Method.DeclaringType == null || m.Method.DeclaringType.Name != "Math") break;
                        SqlExpressionBuilder.Append("ROUND(");
                        Visit(m.Arguments[0]);
                        SqlExpressionBuilder.Append(")");
                        return m;
                    }
                #endregion
            }

            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            var q = c.Value as IQueryable;

            if (q == null && c.Value == null)
            {
                SqlExpressionBuilder.Append("NULL");
            }
            else if (q == null)
            {
                switch (Type.GetTypeCode(c.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        SqlExpressionBuilder.Append(((bool)c.Value) ? 1 : 0);
                        break;

                    case TypeCode.String:
                        if (!ConstantWithoutAp) SqlExpressionBuilder.Append("'");
                        SqlExpressionBuilder.Append(c.Value);
                        if (!ConstantWithoutAp) SqlExpressionBuilder.Append("'");
                        break;

                    case TypeCode.DateTime:
                        if (!ConstantWithoutAp) SqlExpressionBuilder.Append("'");
                        SqlExpressionBuilder.Append(c.Value);
                        if (!ConstantWithoutAp) SqlExpressionBuilder.Append("'");
                        break;

                    case TypeCode.Object:
                        throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", c.Value));

                    default:
                        SqlExpressionBuilder.Append(c.Value);
                        break;
                }
            }
            ConstantWithoutAp = false;
            return c;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            SqlExpressionBuilder.Append("(");
            Visit(b.Left);

            switch (b.NodeType)
            {
                case ExpressionType.Add:
                    SqlExpressionBuilder.Append(" + ");
                    break;
                case ExpressionType.Subtract:
                    SqlExpressionBuilder.Append(" - ");
                    break;
                case ExpressionType.Multiply:
                    SqlExpressionBuilder.Append(" * ");
                    break;
                case ExpressionType.Divide:
                    SqlExpressionBuilder.Append(" / ");
                    break;
                case ExpressionType.Modulo:
                    SqlExpressionBuilder.Append(" % ");
                    break;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported",
                        b.NodeType));

            }

            Visit(b.Right);
            SqlExpressionBuilder.Append(")");
            return b;
        }
        protected override Expression VisitMember(MemberExpression m)
        {
            var name = GetMemberName(m);

            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                SqlExpressionBuilder.Append(name);
                return m;
            }
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.MemberAccess)
            {
                Visit(m.Expression);
                SqlExpressionBuilder.Append("['");
                SqlExpressionBuilder.Append(name);
                SqlExpressionBuilder.Append("']");
                return m;
            }
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Call)
            {
                Visit(m.Expression);
                SqlExpressionBuilder.Append("['");
                SqlExpressionBuilder.Append(name);
                SqlExpressionBuilder.Append("']");
                return m;
            }

            throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
        }

        protected override Expression VisitNew(NewExpression node)
        {
            if (node.Type == typeof(GeoPoint))
            {
                SqlExpressionBuilder.Append("'POINT (");
                Visit(node.Arguments[0]);
                SqlExpressionBuilder.Append(" ");
                Visit(node.Arguments[1]);
                SqlExpressionBuilder.Append(")'");
                return node;
            }
            if (node.Type == typeof(GeoPolygon) && node.Arguments[0] is NewArrayExpression)
            {
                SqlExpressionBuilder.Append("'POLYGON ((");


                var n = node.Arguments[0] as NewArrayExpression;

                for (var i = 0; i < n.Expressions.Count; i++)
                {
                    if (!(n.Expressions[i] is NewExpression)) continue;

                    var m = n.Expressions[i] as NewExpression;

                    if (m.Arguments.Count < 2) continue;

                    Visit(m.Arguments[0]);
                    SqlExpressionBuilder.Append(" ");
                    Visit(m.Arguments[1]);

                    if (i != n.Expressions.Count - 1)
                        SqlExpressionBuilder.Append(", ");
                }
                SqlExpressionBuilder.Append("))'");
                return node;
            }
            return base.VisitNew(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var body = Visit(node.Body);
            if (body == null || body.NodeType != ExpressionType.Lambda)
                return null;
            return body != node.Body ? Expression.Lambda(node.Type, body, node.Parameters) : node;
        }

        protected bool IsNullConstant(Expression exp)
        {
            return (exp.NodeType == ExpressionType.Constant && ((ConstantExpression)exp).Value == null);
        }

        protected string GetMemberName(MemberExpression m)
        {
            return GetMemberName(m.Member.Name, m.Expression.Type);
        }
        protected string GetMemberName(string name, Type type)
        {
            var t = CrateFieldCacheProvider.Instance.Get(type);
            if (t != null && t.ContainsKey(name) && t[name] != null)
            {
                name = t[name].Name;
            }
            return name;
        }
    }
}
