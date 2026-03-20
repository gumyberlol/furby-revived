using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JsonFx.Model;
using JsonFx.Serialization;
using JsonFx.Serialization.Resolvers;

namespace JsonFx.Linq
{
	internal class QueryEngine : ExpressionVisitor<QueryEngine.QueryContext>
	{
		public class QueryContext
		{
			internal IQueryable<IEnumerable<Token<ModelTokenType>>> Input { get; set; }

			internal Type InputType { get; set; }

			internal IDictionary<string, ParameterExpression> Parameters { get; set; }
		}

		private static readonly MethodInfo MemberAccess;

		private readonly ResolverCache Resolver;

		private readonly ITokenAnalyzer<ModelTokenType> Analyzer;

		private readonly IQueryable<IEnumerable<Token<ModelTokenType>>> Source;

		public QueryEngine(ITokenAnalyzer<ModelTokenType> analyzer, IQueryable<IEnumerable<Token<ModelTokenType>>> input)
		{
			if (analyzer == null)
			{
				throw new ArgumentNullException("analyzer");
			}
			if (input == null)
			{
				throw new ArgumentNullException("values");
			}
			Analyzer = analyzer;
			Resolver = analyzer.Settings.Resolver;
			Source = input;
		}

		static QueryEngine()
		{
			MemberAccess = typeof(ModelSubsequencer).GetMethod("Property", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy, null, new Type[2]
			{
				typeof(IEnumerable<Token<ModelTokenType>>),
				typeof(DataName)
			}, null);
		}

		public Expression Translate(Expression expression)
		{
			Expression expression2 = Visit(expression, new QueryContext
			{
				Input = Source
			});
			if (expression != null && expression2 != null && expression.Type != expression2.Type)
			{
				Type type = expression.Type;
				bool asSingle = true;
				if ((type.IsGenericType && typeof(IQueryable).IsAssignableFrom(type)) || typeof(IEnumerable).IsAssignableFrom(type))
				{
					asSingle = false;
					type = type.GetGenericArguments()[0];
				}
				expression2 = CallAnalyze(type, expression2, asSingle);
			}
			return expression2;
		}

		protected override Expression VisitConstant(ConstantExpression constant, QueryContext context)
		{
			Type type = constant.Type;
			Type[] array = ((!type.IsGenericType) ? null : type.GetGenericArguments());
			Type typeFromHandle = typeof(Query<>);
			if (array != null && array.Length == 1 && typeFromHandle.MakeGenericType(array).IsAssignableFrom(type))
			{
				context.InputType = array[0];
				return Expression.Constant(context.Input);
			}
			return base.VisitConstant(constant, context);
		}

		protected override Expression VisitMethodCall(MethodCallExpression m, QueryContext context)
		{
			if (m.Method.DeclaringType != typeof(Queryable) && m.Method.DeclaringType != typeof(Enumerable))
			{
				return base.VisitMethodCall(m, context);
			}
			Type[] array = ((!m.Method.IsGenericMethod) ? Type.EmptyTypes : m.Method.GetGenericArguments());
			QueryContext queryContext = new QueryContext();
			queryContext.Input = context.Input;
			queryContext.InputType = ((array.Length <= 0) ? null : array[0]);
			QueryContext context2 = queryContext;
			Expression[] array2 = VisitExpressionList(m.Arguments, context2).ToArray();
			Expression expression = array2[0];
			if (expression != null && expression.Type != m.Arguments[0].Type)
			{
				Type type = expression.Type;
				if (type.IsGenericType && typeof(IQueryable).IsAssignableFrom(type))
				{
					type = type.GetGenericArguments()[0];
				}
				array[0] = type;
			}
			return Expression.Call(m.Method.DeclaringType, m.Method.Name, array, array2);
		}

		protected override Expression VisitLambda(LambdaExpression lambda, QueryContext context)
		{
			int count = lambda.Parameters.Count;
			IDictionary<string, ParameterExpression> parameters = context.Parameters;
			try
			{
				context.Parameters = new Dictionary<string, ParameterExpression>(count);
				for (int i = 0; i < count; i++)
				{
					if (lambda.Parameters[i].Type == context.InputType)
					{
						context.Parameters[lambda.Parameters[i].Name] = Expression.Parameter(typeof(IEnumerable<Token<ModelTokenType>>), lambda.Parameters[i].Name);
					}
					else
					{
						context.Parameters[lambda.Parameters[i].Name] = lambda.Parameters[i];
					}
				}
				Type[] genericArguments = lambda.Type.GetGenericArguments();
				count = genericArguments.Length;
				for (int j = 0; j < count; j++)
				{
					if (genericArguments[j] == context.InputType)
					{
						genericArguments[j] = typeof(IEnumerable<Token<ModelTokenType>>);
					}
				}
				Expression body = Visit(lambda.Body, context);
				return Expression.Lambda(lambda.Type.GetGenericTypeDefinition().MakeGenericType(genericArguments), body, context.Parameters.Values);
			}
			finally
			{
				context.Parameters = parameters;
			}
		}

		protected override Expression VisitMemberAccess(MemberExpression m, QueryContext context)
		{
			MemberInfo member = m.Member;
			Type targetType = ((member is PropertyInfo) ? ((PropertyInfo)member).PropertyType : ((!(member is FieldInfo)) ? null : ((FieldInfo)member).FieldType));
			MemberMap memberMap = Resolver.LoadMemberMap(member);
			ParameterExpression parameterExpression = (ParameterExpression)m.Expression;
			MethodCallExpression sequence = Expression.Call(MemberAccess, context.Parameters[parameterExpression.Name], Expression.Constant(memberMap.DataName));
			return CallAnalyze(targetType, sequence, true);
		}

		private Expression CallAnalyze(Type targetType, Expression sequence, bool asSingle)
		{
			if (sequence.Type == typeof(IEnumerable<IEnumerable<Token<ModelTokenType>>>))
			{
				sequence = Expression.Call(typeof(Queryable), "AsQueryable", new Type[1] { typeof(IEnumerable<Token<ModelTokenType>>) }, sequence);
			}
			Expression expression;
			if (sequence.Type == typeof(IQueryable<IEnumerable<Token<ModelTokenType>>>) || sequence.Type == typeof(IOrderedQueryable<IEnumerable<Token<ModelTokenType>>>))
			{
				sequence = Expression.Call(typeof(Queryable), "DefaultIfEmpty", new Type[1] { typeof(IEnumerable<Token<ModelTokenType>>) }, sequence, Expression.Call(typeof(Enumerable), "Empty", new Type[1] { typeof(Token<ModelTokenType>) }));
				ParameterExpression parameterExpression = Expression.Parameter(typeof(IEnumerable<Token<ModelTokenType>>), "sequence");
				expression = Expression.Lambda(Expression.Call(Expression.Constant(Analyzer), "Analyze", new Type[1] { targetType }, parameterExpression), parameterExpression);
				expression = Expression.Call(typeof(Queryable), "SelectMany", new Type[2]
				{
					typeof(IEnumerable<Token<ModelTokenType>>),
					targetType
				}, sequence, expression);
			}
			else
			{
				sequence = Expression.Coalesce(sequence, Expression.Call(typeof(Enumerable), "Empty", new Type[1] { typeof(Token<ModelTokenType>) }));
				expression = Expression.Call(Expression.Constant(Analyzer), "Analyze", new Type[1] { targetType }, sequence);
			}
			if (asSingle)
			{
				expression = Expression.Call(typeof(Enumerable), "FirstOrDefault", new Type[1] { targetType }, expression);
			}
			return expression;
		}
	}
}
