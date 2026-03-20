using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using JsonFx.Model;
using JsonFx.Serialization;

namespace JsonFx.Linq
{
	internal class ExpressionWalker : ExpressionVisitor<List<Token<ModelTokenType>>>, IQueryTextProvider, IObjectWalker<ModelTokenType>
	{
		private readonly ITextFormatter<ModelTokenType> Formatter;

		public ExpressionWalker(ITextFormatter<ModelTokenType> formatter)
		{
			if (formatter == null)
			{
				throw new ArgumentNullException("formatter");
			}
			Formatter = formatter;
		}

		IEnumerable<Token<ModelTokenType>> IObjectWalker<ModelTokenType>.GetTokens(object value)
		{
			return GetTokens(value as Expression);
		}

		protected override Expression Visit(Expression expression, List<Token<ModelTokenType>> tokens)
		{
			if (expression == null)
			{
				tokens.Add(ModelGrammar.TokenNull);
			}
			return base.Visit(expression, tokens);
		}

		protected override Expression VisitUnknown(Expression expression, List<Token<ModelTokenType>> tokens)
		{
			tokens.Add(ModelGrammar.TokenPrimitive(expression.NodeType));
			return expression;
		}

		protected override ElementInit VisitElementInitializer(ElementInit init, List<Token<ModelTokenType>> tokens)
		{
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			tokens.Add(ModelGrammar.TokenProperty("ElementInit"));
			tokens.Add(ModelGrammar.TokenArrayBeginUnnamed);
			try
			{
				VisitExpressionList(init.Arguments, tokens);
				return init;
			}
			finally
			{
				tokens.Add(ModelGrammar.TokenArrayEnd);
				tokens.Add(ModelGrammar.TokenObjectEnd);
			}
		}

		protected override Expression VisitUnary(UnaryExpression expression, List<Token<ModelTokenType>> tokens)
		{
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			tokens.Add(ModelGrammar.TokenProperty(expression.NodeType));
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			try
			{
				tokens.Add(ModelGrammar.TokenProperty("Type"));
				tokens.Add(ModelGrammar.TokenPrimitive(GetTypeName(expression.Type)));
				tokens.Add(ModelGrammar.TokenProperty("Method"));
				tokens.Add(ModelGrammar.TokenPrimitive(GetMemberName(expression.Method)));
				tokens.Add(ModelGrammar.TokenProperty("Operand"));
				Visit(expression.Operand, tokens);
				return expression;
			}
			finally
			{
				tokens.Add(ModelGrammar.TokenObjectEnd);
				tokens.Add(ModelGrammar.TokenObjectEnd);
			}
		}

		protected override Expression VisitBinary(BinaryExpression expression, List<Token<ModelTokenType>> tokens)
		{
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			tokens.Add(ModelGrammar.TokenProperty(expression.NodeType));
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			try
			{
				tokens.Add(ModelGrammar.TokenProperty("Left"));
				Visit(expression.Left, tokens);
				tokens.Add(ModelGrammar.TokenProperty("Right"));
				Visit(expression.Right, tokens);
				tokens.Add(ModelGrammar.TokenProperty("Conversion"));
				Visit(expression.Conversion, tokens);
				tokens.Add(ModelGrammar.TokenProperty("IsLiftedToNull"));
				tokens.Add(ModelGrammar.TokenPrimitive(expression.IsLiftedToNull));
				tokens.Add(ModelGrammar.TokenProperty("Method"));
				tokens.Add(ModelGrammar.TokenPrimitive(GetMemberName(expression.Method)));
				return expression;
			}
			finally
			{
				tokens.Add(ModelGrammar.TokenObjectEnd);
				tokens.Add(ModelGrammar.TokenObjectEnd);
			}
		}

		protected override Expression VisitTypeIs(TypeBinaryExpression expression, List<Token<ModelTokenType>> tokens)
		{
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			tokens.Add(ModelGrammar.TokenProperty(expression.NodeType));
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			try
			{
				tokens.Add(ModelGrammar.TokenProperty("TypeOperand"));
				tokens.Add(ModelGrammar.TokenPrimitive(expression.TypeOperand));
				tokens.Add(ModelGrammar.TokenProperty("Expression"));
				Visit(expression.Expression, tokens);
				return expression;
			}
			finally
			{
				tokens.Add(ModelGrammar.TokenObjectEnd);
				tokens.Add(ModelGrammar.TokenObjectEnd);
			}
		}

		protected override Expression VisitConstant(ConstantExpression expression, List<Token<ModelTokenType>> tokens)
		{
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			tokens.Add(ModelGrammar.TokenProperty(expression.NodeType));
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			try
			{
				tokens.Add(ModelGrammar.TokenProperty("Type"));
				tokens.Add(ModelGrammar.TokenPrimitive(GetTypeName(expression.Type)));
				tokens.Add(ModelGrammar.TokenProperty("Value"));
				if (expression.Type == null || expression.Value == null)
				{
					tokens.Add(ModelGrammar.TokenNull);
				}
				else if (typeof(IQueryable).IsAssignableFrom(expression.Type))
				{
					tokens.Add(ModelGrammar.TokenPrimitive("[ ... ]"));
				}
				else
				{
					tokens.Add(ModelGrammar.TokenPrimitive(Token<ModelTokenType>.ToString(expression.Value)));
				}
				return expression;
			}
			finally
			{
				tokens.Add(ModelGrammar.TokenObjectEnd);
				tokens.Add(ModelGrammar.TokenObjectEnd);
			}
		}

		protected override Expression VisitConditional(ConditionalExpression expression, List<Token<ModelTokenType>> tokens)
		{
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			tokens.Add(ModelGrammar.TokenProperty(expression.NodeType));
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			try
			{
				tokens.Add(ModelGrammar.TokenProperty("Test"));
				Visit(expression.Test, tokens);
				tokens.Add(ModelGrammar.TokenProperty("IfTrue"));
				Visit(expression.IfTrue, tokens);
				tokens.Add(ModelGrammar.TokenProperty("IfFalse"));
				Visit(expression.IfFalse, tokens);
				return expression;
			}
			finally
			{
				tokens.Add(ModelGrammar.TokenObjectEnd);
				tokens.Add(ModelGrammar.TokenObjectEnd);
			}
		}

		protected override ParameterExpression VisitParameter(ParameterExpression expression, List<Token<ModelTokenType>> tokens)
		{
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			tokens.Add(ModelGrammar.TokenProperty(expression.NodeType));
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			try
			{
				tokens.Add(ModelGrammar.TokenProperty("Type"));
				tokens.Add(ModelGrammar.TokenPrimitive(GetTypeName(expression.Type)));
				tokens.Add(ModelGrammar.TokenProperty("Name"));
				tokens.Add(ModelGrammar.TokenPrimitive(expression.Name));
				return expression;
			}
			finally
			{
				tokens.Add(ModelGrammar.TokenObjectEnd);
				tokens.Add(ModelGrammar.TokenObjectEnd);
			}
		}

		protected override IEnumerable<ParameterExpression> VisitParameterList(IList<ParameterExpression> list, List<Token<ModelTokenType>> tokens)
		{
			tokens.Add(ModelGrammar.TokenArrayBeginUnnamed);
			try
			{
				foreach (ParameterExpression item in list)
				{
					VisitParameter(item, tokens);
				}
				return list;
			}
			finally
			{
				tokens.Add(ModelGrammar.TokenArrayEnd);
			}
		}

		protected override Expression VisitMemberAccess(MemberExpression expression, List<Token<ModelTokenType>> tokens)
		{
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			tokens.Add(ModelGrammar.TokenProperty(expression.NodeType));
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			try
			{
				tokens.Add(ModelGrammar.TokenProperty("Member"));
				tokens.Add(ModelGrammar.TokenPrimitive(GetMemberName(expression.Member)));
				tokens.Add(ModelGrammar.TokenProperty("Expression"));
				Visit(expression.Expression, tokens);
				return expression;
			}
			finally
			{
				tokens.Add(ModelGrammar.TokenObjectEnd);
				tokens.Add(ModelGrammar.TokenObjectEnd);
			}
		}

		protected override Expression VisitMethodCall(MethodCallExpression expression, List<Token<ModelTokenType>> tokens)
		{
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			tokens.Add(ModelGrammar.TokenProperty(expression.NodeType));
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			try
			{
				tokens.Add(ModelGrammar.TokenProperty("Object"));
				Visit(expression.Object, tokens);
				tokens.Add(ModelGrammar.TokenProperty("Method"));
				tokens.Add(ModelGrammar.TokenPrimitive(GetMemberName(expression.Method)));
				tokens.Add(ModelGrammar.TokenProperty("Arguments"));
				VisitExpressionList(expression.Arguments, tokens);
				return expression;
			}
			finally
			{
				tokens.Add(ModelGrammar.TokenObjectEnd);
				tokens.Add(ModelGrammar.TokenObjectEnd);
			}
		}

		protected override IEnumerable<Expression> VisitExpressionList(IList<Expression> list, List<Token<ModelTokenType>> tokens)
		{
			tokens.Add(ModelGrammar.TokenArrayBeginUnnamed);
			try
			{
				foreach (Expression item in list)
				{
					Visit(item, tokens);
				}
				return list;
			}
			finally
			{
				tokens.Add(ModelGrammar.TokenArrayEnd);
			}
		}

		protected override MemberAssignment VisitMemberAssignment(MemberAssignment assignment, List<Token<ModelTokenType>> tokens)
		{
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			tokens.Add(ModelGrammar.TokenProperty("MemberAssignment"));
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			try
			{
				tokens.Add(ModelGrammar.TokenProperty("Member"));
				tokens.Add(ModelGrammar.TokenPrimitive(GetMemberName(assignment.Member)));
				tokens.Add(ModelGrammar.TokenProperty("Expression"));
				Visit(assignment.Expression, tokens);
				return assignment;
			}
			finally
			{
				tokens.Add(ModelGrammar.TokenObjectEnd);
				tokens.Add(ModelGrammar.TokenObjectEnd);
			}
		}

		protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding, List<Token<ModelTokenType>> tokens)
		{
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			tokens.Add(ModelGrammar.TokenProperty("MemberMemberBinding"));
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			try
			{
				tokens.Add(ModelGrammar.TokenProperty("Member"));
				tokens.Add(ModelGrammar.TokenPrimitive(GetMemberName(binding.Member)));
				tokens.Add(ModelGrammar.TokenProperty("Bindings"));
				VisitBindingList(binding.Bindings, tokens);
				return binding;
			}
			finally
			{
				tokens.Add(ModelGrammar.TokenObjectEnd);
				tokens.Add(ModelGrammar.TokenObjectEnd);
			}
		}

		protected override MemberListBinding VisitMemberListBinding(MemberListBinding binding, List<Token<ModelTokenType>> tokens)
		{
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			tokens.Add(ModelGrammar.TokenProperty("MemberListBinding"));
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			try
			{
				tokens.Add(ModelGrammar.TokenProperty("Member"));
				tokens.Add(ModelGrammar.TokenPrimitive(GetMemberName(binding.Member)));
				tokens.Add(ModelGrammar.TokenProperty("Initializers"));
				VisitElementInitializerList(binding.Initializers, tokens);
				return binding;
			}
			finally
			{
				tokens.Add(ModelGrammar.TokenObjectEnd);
				tokens.Add(ModelGrammar.TokenObjectEnd);
			}
		}

		protected override IEnumerable<MemberBinding> VisitBindingList(IList<MemberBinding> list, List<Token<ModelTokenType>> tokens)
		{
			tokens.Add(ModelGrammar.TokenArrayBeginUnnamed);
			try
			{
				foreach (MemberBinding item in list)
				{
					VisitBinding(item, tokens);
				}
				return list;
			}
			finally
			{
				tokens.Add(ModelGrammar.TokenArrayEnd);
			}
		}

		protected override IEnumerable<ElementInit> VisitElementInitializerList(IList<ElementInit> list, List<Token<ModelTokenType>> tokens)
		{
			tokens.Add(ModelGrammar.TokenArrayBeginUnnamed);
			try
			{
				foreach (ElementInit item in list)
				{
					VisitElementInitializer(item, tokens);
				}
				return list;
			}
			finally
			{
				tokens.Add(ModelGrammar.TokenArrayEnd);
			}
		}

		protected override Expression VisitLambda(LambdaExpression expression, List<Token<ModelTokenType>> tokens)
		{
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			tokens.Add(ModelGrammar.TokenProperty(expression.NodeType));
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			try
			{
				tokens.Add(ModelGrammar.TokenProperty("Type"));
				tokens.Add(ModelGrammar.TokenPrimitive(GetTypeName(expression.Type)));
				tokens.Add(ModelGrammar.TokenProperty("Body"));
				Visit(expression.Body, tokens);
				tokens.Add(ModelGrammar.TokenProperty("Parameters"));
				tokens.Add(ModelGrammar.TokenArrayBeginUnnamed);
				foreach (ParameterExpression parameter in expression.Parameters)
				{
					VisitParameter(parameter, tokens);
				}
				tokens.Add(ModelGrammar.TokenArrayEnd);
				return expression;
			}
			finally
			{
				tokens.Add(ModelGrammar.TokenObjectEnd);
				tokens.Add(ModelGrammar.TokenObjectEnd);
			}
		}

		protected override NewExpression VisitNew(NewExpression expression, List<Token<ModelTokenType>> tokens)
		{
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			tokens.Add(ModelGrammar.TokenProperty(expression.NodeType));
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			try
			{
				tokens.Add(ModelGrammar.TokenProperty("Arguments"));
				VisitExpressionList(expression.Arguments, tokens);
				tokens.Add(ModelGrammar.TokenProperty("Members"));
				if (expression.Members == null)
				{
					tokens.Add(ModelGrammar.TokenNull);
				}
				else
				{
					tokens.Add(ModelGrammar.TokenArrayBeginUnnamed);
					foreach (MemberInfo member in expression.Members)
					{
						tokens.Add(ModelGrammar.TokenPrimitive(GetMemberName(member)));
					}
					tokens.Add(ModelGrammar.TokenArrayEnd);
				}
				return expression;
			}
			finally
			{
				tokens.Add(ModelGrammar.TokenObjectEnd);
				tokens.Add(ModelGrammar.TokenObjectEnd);
			}
		}

		protected override Expression VisitMemberInit(MemberInitExpression expression, List<Token<ModelTokenType>> tokens)
		{
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			tokens.Add(ModelGrammar.TokenProperty(expression.NodeType));
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			try
			{
				tokens.Add(ModelGrammar.TokenProperty("NewExpression"));
				VisitNew(expression.NewExpression, tokens);
				tokens.Add(ModelGrammar.TokenProperty("Bindings"));
				VisitBindingList(expression.Bindings, tokens);
				return expression;
			}
			finally
			{
				tokens.Add(ModelGrammar.TokenObjectEnd);
				tokens.Add(ModelGrammar.TokenObjectEnd);
			}
		}

		protected override Expression VisitListInit(ListInitExpression expression, List<Token<ModelTokenType>> tokens)
		{
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			tokens.Add(ModelGrammar.TokenProperty(expression.NodeType));
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			try
			{
				tokens.Add(ModelGrammar.TokenProperty("NewExpression"));
				VisitNew(expression.NewExpression, tokens);
				tokens.Add(ModelGrammar.TokenProperty("Initializers"));
				VisitElementInitializerList(expression.Initializers, tokens);
				return expression;
			}
			finally
			{
				tokens.Add(ModelGrammar.TokenObjectEnd);
				tokens.Add(ModelGrammar.TokenObjectEnd);
			}
		}

		protected override Expression VisitNewArray(NewArrayExpression expression, List<Token<ModelTokenType>> tokens)
		{
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			tokens.Add(ModelGrammar.TokenProperty(expression.NodeType));
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			try
			{
				tokens.Add(ModelGrammar.TokenProperty("ElementType"));
				tokens.Add(ModelGrammar.TokenPrimitive(expression.Type.GetElementType()));
				tokens.Add(ModelGrammar.TokenProperty("Expressions"));
				VisitExpressionList(expression.Expressions, tokens);
				return expression;
			}
			finally
			{
				tokens.Add(ModelGrammar.TokenObjectEnd);
				tokens.Add(ModelGrammar.TokenObjectEnd);
			}
		}

		protected override Expression VisitInvocation(InvocationExpression expression, List<Token<ModelTokenType>> tokens)
		{
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			tokens.Add(ModelGrammar.TokenProperty(expression.NodeType));
			tokens.Add(ModelGrammar.TokenObjectBeginUnnamed);
			try
			{
				tokens.Add(ModelGrammar.TokenProperty("Arguments"));
				VisitExpressionList(expression.Arguments, tokens);
				tokens.Add(ModelGrammar.TokenProperty("Expression"));
				Visit(expression.Expression, tokens);
				return expression;
			}
			finally
			{
				tokens.Add(ModelGrammar.TokenObjectEnd);
				tokens.Add(ModelGrammar.TokenObjectEnd);
			}
		}

		public string GetQueryText(Expression expression)
		{
			try
			{
				IEnumerable<Token<ModelTokenType>> tokens = GetTokens(expression);
				return Formatter.Format(tokens);
			}
			catch (Exception ex)
			{
				return "Error: [ " + ex.Message + " ]";
			}
		}

		public IEnumerable<Token<ModelTokenType>> GetTokens(Expression expression)
		{
			if (expression == null)
			{
				throw new InvalidOperationException("ExpressionWalker only walks expressions.");
			}
			List<Token<ModelTokenType>> list = new List<Token<ModelTokenType>>();
			Visit(expression, list);
			return list;
		}

		private static string GetMemberName(MemberInfo member)
		{
			if (member == null)
			{
				return null;
			}
			MethodInfo methodInfo = member as MethodInfo;
			if (methodInfo != null && methodInfo.IsGenericMethod)
			{
				Type[] genericArguments = methodInfo.GetGenericArguments();
				int num = genericArguments.Length;
				string[] array = new string[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = GetTypeName(genericArguments[i]);
				}
				return GetTypeName(methodInfo.ReturnType) + " " + GetTypeName(methodInfo.DeclaringType) + '.' + methodInfo.Name + '<' + string.Join(", ", array) + '>';
			}
			return GetTypeName(member.DeclaringType) + '.' + member.Name;
		}

		private static string GetTypeName(Type type)
		{
			if (type == null)
			{
				return null;
			}
			if (!type.IsGenericType)
			{
				return type.Name;
			}
			StringBuilder stringBuilder = new StringBuilder(type.GetGenericTypeDefinition().Name.Split('`')[0]);
			Type[] genericArguments = type.GetGenericArguments();
			stringBuilder.Append('<');
			int i = 0;
			for (int num = genericArguments.Length; i < num; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(GetTypeName(genericArguments[i]));
			}
			stringBuilder.Append('>');
			return stringBuilder.ToString();
		}
	}
}
