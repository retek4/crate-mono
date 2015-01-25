using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Crate.Helpers;

namespace Crate.ExpressionTranslater
{
    internal class SelectTranslater : BaseTranslater
    {
        private Dictionary<string, Tuple<string, Type>> _groupByObject = new Dictionary<string, Tuple<string, Type>>();

        public override string Translate(Expression expression)
        {
            _groupByObject = new Dictionary<string, Tuple<string, Type>>();
            return TranslateWorker(expression);
        }

        private string TranslateWorker(Expression expression)
        {
            SqlExpressionBuilder = new StringBuilder();
            Visit(expression);
            var s = SqlExpressionBuilder.ToString().Trim();
            return s.EndsWith(",") ? s.Substring(0, s.Length - 1) : s;
        }

        public string Translate(Expression expression, Dictionary<string, Tuple<string, Type>> groupByObject)
        {
            _groupByObject = groupByObject;
            return TranslateWorker(expression);
        }
        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            Visit(node.Expression);
            SqlExpressionBuilder.Append(" AS ");

            SqlExpressionBuilder.Append(GetMemberName(node.Member.Name, node.Member.ReflectedType));

            SqlExpressionBuilder.Append(", ");
            return node;
        }
        protected override Expression VisitMember(MemberExpression m)
        {
            var name = GetMemberName(m);

            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                if (m.Expression.Type.Name.Contains("IGrouping"))
                {
                    if (_groupByObject.Count == 1 && m.Member.Name == "Key")
                    {
                        var temp = _groupByObject.First().Value;
                        if (temp.Item2 == m.Type)
                        {
                            SqlExpressionBuilder.Append(temp.Item1);
                            return m;
                        }

                    }
                }
                SqlExpressionBuilder.Append(name);
                return m;
            }
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.MemberAccess)
            {
                var ma = m.Expression as MemberExpression;
                if (ma != null && ma.Expression.Type.Name.Contains("IGrouping"))
                {
                    if (_groupByObject.ContainsKey(m.Member.Name))
                    {

                        SqlExpressionBuilder.Append(_groupByObject[m.Member.Name].Item1);
                        return m;
                    }
                }

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
            for (var i = 0; i < node.Arguments.Count; i++)
            {
                var arg = node.Arguments[i];
                //visiting
                Visit(arg);
                SqlExpressionBuilder.Append(", ");

            }
            return node;
        }
        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            foreach (var binding in node.Bindings)
            {
                if (binding.BindingType == MemberBindingType.Assignment)
                {
                    VisitMemberAssignment((MemberAssignment)binding);
                }
                else
                {
                    throw new NotSupportedException(string.Format("The member '{0}', type:'{1}' is not supported", binding.Member.Name, binding.BindingType));
                }
            }
            return node;
        }

    }
}
