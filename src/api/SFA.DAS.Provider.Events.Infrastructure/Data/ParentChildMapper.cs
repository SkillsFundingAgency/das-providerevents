using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SFA.DAS.Provider.Events.Infrastructure.Data
{
    public sealed class ParentChildrenMapper<TParent, TChild> where TParent : class
    {
        public Func<TParent, TChild, TParent> Map<T>(
            Dictionary<T, TParent> lookup,
            Func<TParent, T> parentIdentifierProperty,
            Expression<Func<TParent, IList<TChild>>> parentChildrenProperty) 
        {
            if (lookup == null)
            {
                throw new ArgumentNullException(nameof(lookup));
            }
            
            return (x, y) =>
            {
                TParent parent;
                if (!lookup.TryGetValue(parentIdentifierProperty.Invoke(x), out parent))
                {
                    lookup.Add(parentIdentifierProperty.Invoke(x), parent = x);
                }

                if (y != null)
                {
                    var children = parentChildrenProperty.Compile().Invoke(parent);
                    if (children == null)
                    {
                        children = new List<TChild>();
                        var property = (PropertyInfo)((MemberExpression)parentChildrenProperty.Body).Member;
                        property.SetValue(parent, children, null);
                    }
                    children.Add(y);
                }

                return parent;
            };
        }
    }
}
