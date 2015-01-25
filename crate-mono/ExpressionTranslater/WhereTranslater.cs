using System;
using System.Linq.Expressions;

namespace Crate.ExpressionTranslater
{
    internal class WhereTranslater : BaseTranslater
    {

        private bool _negateNext = false;

        public override string Translate(Expression expression)
        {
            Visit(expression);
            return SqlExpressionBuilder.ToString();
        }
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "Contains":
                    {
                        if (m.Object == null) break;
                        ConstantWithoutAp = true;
                        if (m.Object.NodeType == ExpressionType.Parameter)
                        {
                            SqlExpressionBuilder.Append("'%");
                            Visit(m.Arguments[0]);
                            SqlExpressionBuilder.Append("%'");
                            SqlExpressionBuilder.Append(" LIKE ");
                            return m;
                        }

                        Visit(m.Object);
                        SqlExpressionBuilder.Append(" LIKE ");
                        SqlExpressionBuilder.Append("'%");
                        Visit(m.Arguments[0]);
                        SqlExpressionBuilder.Append("%'");
                        return m;
                    }
                case "StartsWith":
                    {
                        Visit(m.Object);
                        SqlExpressionBuilder.Append(" LIKE ");
                        ConstantWithoutAp = true;
                        SqlExpressionBuilder.Append("'");
                        Visit(m.Arguments[0]);
                        SqlExpressionBuilder.Append("%'");
                        return m;
                    }
                case "EndsWith":
                    {
                        Visit(m.Object); 
                        ConstantWithoutAp = true;
                        SqlExpressionBuilder.Append(" LIKE ");
                        SqlExpressionBuilder.Append("'%");
                        Visit(m.Arguments[0]);
                        SqlExpressionBuilder.Append("'");
                        return m;
                    }
                case "IsMatch":
                    {
                        Visit(m.Arguments[0]);
                        SqlExpressionBuilder.Append(_negateNext ? " !~ " : " ~ ");
                        Visit(m.Arguments[1]);
                        return m;
                    }
                case "Any":
                    {
                        Visit(m.Arguments[1]);
                        SqlExpressionBuilder.Append(" ANY(");
                        Visit(m.Arguments[0]);
                        SqlExpressionBuilder.Append(")");
                        return m;
                    }
                case "IsNullOrEmpty":
                    {
                        SqlExpressionBuilder.Append("( ");
                        Visit(m.Arguments[0]);
                        SqlExpressionBuilder.Append("= '' OR ");
                        Visit(m.Arguments[0]);
                        SqlExpressionBuilder.Append(" IS NULL )");
                        return m;
                    }
            }

            return base.VisitMethodCall(m);

        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:

                    if (u.Operand.NodeType == ExpressionType.Call && u.Operand is MethodCallExpression)
                    {
                        var mc = u.Operand as MethodCallExpression;
                        if (mc.Method.Name == "IsMatch")
                        {
                            _negateNext = true;
                        }
                        else
                        {
                            SqlExpressionBuilder.Append(" NOT ");
                        }
                    }
                    else
                    {
                        SqlExpressionBuilder.Append(" NOT ");
                    }
                    Visit(u.Operand);
                    break;
                case ExpressionType.Convert:
                    Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported",
                        u.NodeType));
            }
            return u;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            SqlExpressionBuilder.Append("(");
            Visit(b.Left);

            switch (b.NodeType)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.And:
                    SqlExpressionBuilder.Append(" AND ");
                    break;

                case ExpressionType.OrElse:
                case ExpressionType.Or:
                    SqlExpressionBuilder.Append(" OR ");
                    break;

                case ExpressionType.Equal:
                    if (IsNullConstant(b.Right))
                    {
                        SqlExpressionBuilder.Append(" IS ");
                    }
                    else
                    {
                        SqlExpressionBuilder.Append(" = ");
                    }
                    break;

                case ExpressionType.NotEqual:
                    if (IsNullConstant(b.Right))
                    {
                        SqlExpressionBuilder.Append(" IS NOT ");
                    }
                    else
                    {
                        SqlExpressionBuilder.Append(" <> ");
                    }
                    break;

                case ExpressionType.LessThan:
                    SqlExpressionBuilder.Append(" < ");
                    break;

                case ExpressionType.LessThanOrEqual:
                    SqlExpressionBuilder.Append(" <= ");
                    break;

                case ExpressionType.GreaterThan:
                    SqlExpressionBuilder.Append(" > ");
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    SqlExpressionBuilder.Append(" >= ");
                    break;
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

    }
}
