using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Crate.ExpressionTranslater
{
    internal class GroupByTranslater : BaseTranslater
    {
        //Stores groupby object main object relationship
        public Dictionary<string, Tuple<string, Type>> GroupByObject
        {
            get
            {
                //temp solution for group mapping. 3 cases: goup by single filed, by anonymus object and  by real object. If goup by single filed
                //stored in _members else (anonymus object and real object) stored in this dict)

                if (_grupByObject.Count != 0 || _members.Count <= 0) return _grupByObject;
                
                //hackish
                var s = GetCleanSql().Split(',');
                _grupByObject.Add(_members[0].Item1,
                    s.Length > 0 ? new Tuple<string, Type>(s[0], _members[0].Item2) : _members[0]);
                return _grupByObject;
            }
        }

        private string GetCleanSql()
        {
            return SqlExpressionBuilder.ToString().Trim().Trim(',');
        }

        private Dictionary<string, Tuple<string, Type>> _grupByObject = new Dictionary<string, Tuple<string, Type>>();
        private List<Tuple<string, Type>> _members = new List<Tuple<string, Type>>();
        public override string Translate(Expression expression)
        {
            _grupByObject = new Dictionary<string, Tuple<string, Type>>();
            _members = new List<Tuple<string, Type>>();

            SqlExpressionBuilder = new StringBuilder();
            Visit(expression);
            return GetCleanSql();
        }
        protected override Expression VisitMember(MemberExpression m)
        {
            _members.Add(Tuple.Create(m.Member.Name, m.Type));
            return base.VisitMember(m);
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            Visit(node.Expression);

            SqlExpressionBuilder.Append(", ");

            if (node.Expression is MemberExpression)
                SaveGroupByMappingData(node.Member.Name, node.Expression as MemberExpression);
            
            return node;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            for (var i = 0; i < node.Arguments.Count; i++)
            {
                var arg = node.Arguments[i];

                Visit(arg);
                SqlExpressionBuilder.Append(", ");

                if (arg is MemberExpression)
                    SaveGroupByMappingData(node.Members[i].Name, arg as MemberExpression);

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
                    throw new NotSupportedException(string.Format("The member '{0}', type:'{1}' is not supported", binding.Member.Name,binding.BindingType));
                }
            }
            return node;
        }

        private void SaveGroupByMappingData(string name, MemberExpression arg)
        {
            var s = GetCleanSql().Split(',');
            if (s.Length > 0)
            {
                _grupByObject.Add(name, Tuple.Create(s[s.Length - 1], arg.Type));
            }
            else
                _grupByObject.Add(name, Tuple.Create(arg.Member.Name, arg.Type));

        }

    }
}


