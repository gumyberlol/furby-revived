using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace JsonFx.Linq
{
	public class Query<T> : IEnumerable, IQueryable, IOrderedQueryable, IQueryable<T>, IEnumerable<T>, IOrderedQueryable<T>
	{
		private readonly IQueryProvider Provider;

		private readonly Expression Expression;

		private object result;

		private bool hasResult;

		Expression IQueryable.Expression
		{
			get
			{
				return Expression;
			}
		}

		Type IQueryable.ElementType
		{
			get
			{
				return typeof(T);
			}
		}

		IQueryProvider IQueryable.Provider
		{
			get
			{
				return Provider;
			}
		}

		public Query(IQueryProvider provider)
			: this(provider, (Expression)null)
		{
		}

		public Query(IQueryProvider provider, Expression expression)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (expression != null && !typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
			{
				throw new ArgumentException("expression");
			}
			Provider = provider;
			Expression = ((expression == null) ? Expression.Constant(this) : expression);
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			if (!hasResult)
			{
				result = Provider.Execute<IEnumerable<T>>(Expression);
				hasResult = true;
			}
			return ((IEnumerable<T>)result).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			if (!hasResult)
			{
				result = Provider.Execute(Expression);
				hasResult = true;
			}
			return ((IEnumerable)result).GetEnumerator();
		}

		public override string ToString()
		{
			IQueryTextProvider queryTextProvider = Provider as IQueryTextProvider;
			if (queryTextProvider == null)
			{
				return string.Empty;
			}
			return queryTextProvider.GetQueryText(Expression);
		}
	}
}
