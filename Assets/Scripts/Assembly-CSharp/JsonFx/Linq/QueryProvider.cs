using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JsonFx.Json;
using JsonFx.Model;
using JsonFx.Serialization;

namespace JsonFx.Linq
{
	internal class QueryProvider : BaseQueryProvider
	{
		private readonly QueryEngine Engine;

		public QueryProvider(ITokenAnalyzer<ModelTokenType> analyzer, IQueryable<IEnumerable<Token<ModelTokenType>>> sequences)
		{
			if (analyzer == null)
			{
				throw new ArgumentNullException("analyzer");
			}
			if (sequences == null)
			{
				throw new ArgumentNullException("sequences");
			}
			Engine = new QueryEngine(analyzer, sequences);
		}

		public override object Execute(Expression expression)
		{
			Expression expression2 = Engine.Translate(expression);
			if (expression2.Type.IsValueType)
			{
				expression2 = Expression.Convert(expression2, typeof(object));
			}
			Func<object> func = Expression.Lambda<Func<object>>(expression2, new ParameterExpression[0]).Compile();
			return func();
		}

		public override string GetQueryText(Expression expression)
		{
			if (expression == null)
			{
				return string.Empty;
			}
			DataWriterSettings dataWriterSettings = new DataWriterSettings();
			dataWriterSettings.PrettyPrint = true;
			return new ExpressionWalker(new JsonWriter.JsonFormatter(dataWriterSettings)).GetQueryText(expression);
		}
	}
}
