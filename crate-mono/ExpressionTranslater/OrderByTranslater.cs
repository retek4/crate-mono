using System;
using System.Linq.Expressions;
using System.Text;

namespace Crate.ExpressionTranslater
{
    internal class OrderByTranslater : BaseTranslater
    {
        public override string Translate(Expression expression)
        {
            SqlExpressionBuilder = new StringBuilder();
            Visit(expression);
            var s = SqlExpressionBuilder.ToString().Trim();
            return s.EndsWith(",") ? s.Substring(0, s.Length - 1) : s;
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            Visit(node.Expression);
            SqlExpressionBuilder.Append(", ");
            return node;
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
     }
}
