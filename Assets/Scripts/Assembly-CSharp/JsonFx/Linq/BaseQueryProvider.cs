using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JsonFx.Serialization;

namespace JsonFx.Linq
{
	internal abstract class BaseQueryProvider : IQueryTextProvider, IQueryProvider
	{
		IQueryable<S> IQueryProvider.CreateQuery<S>(Expression expression)
		{
			return new Query<S>(this, expression);
		}

		IQueryable IQueryProvider.CreateQuery(Expression expression)
		{
			Type type = TypeCoercionUtility.GetElementType(expression.Type) ?? expression.Type;
			try
			{
				return (IQueryable)Activator.CreateInstance(typeof(Query<>).MakeGenericType(type), this, expression);
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
		}

		S IQueryProvider.Execute<S>(Expression expression)
		{
			return (S)(Execute(expression) ?? ((object)default(S)));
		}

		object IQueryProvider.Execute(Expression expression)
		{
			return Execute(expression);
		}

		public abstract object Execute(Expression expression);

		public abstract string GetQueryText(Expression expression);
	}
}
