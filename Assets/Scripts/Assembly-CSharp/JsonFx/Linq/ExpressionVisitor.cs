using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace JsonFx.Linq
{
	internal abstract class ExpressionVisitor<T>
	{
		private const string ErrorUnexpectedExpression = "Unexpected expression ({0})";

		private const string ErrorUnexpectedMemberBinding = "Unexpected member binding ({0})";

		protected virtual Expression Visit(Expression expression, T context)
		{
			if (expression == null)
			{
				return null;
			}
			switch (expression.NodeType)
			{
			case ExpressionType.ArrayLength:
			case ExpressionType.Convert:
			case ExpressionType.ConvertChecked:
			case ExpressionType.Negate:
			case ExpressionType.UnaryPlus:
			case ExpressionType.NegateChecked:
			case ExpressionType.Not:
			case ExpressionType.Quote:
			case ExpressionType.TypeAs:
				return VisitUnary((UnaryExpression)expression, context);
			case ExpressionType.Add:
			case ExpressionType.AddChecked:
			case ExpressionType.And:
			case ExpressionType.AndAlso:
			case ExpressionType.ArrayIndex:
			case ExpressionType.Coalesce:
			case ExpressionType.Divide:
			case ExpressionType.Equal:
			case ExpressionType.ExclusiveOr:
			case ExpressionType.GreaterThan:
			case ExpressionType.GreaterThanOrEqual:
			case ExpressionType.LeftShift:
			case ExpressionType.LessThan:
			case ExpressionType.LessThanOrEqual:
			case ExpressionType.Modulo:
			case ExpressionType.Multiply:
			case ExpressionType.MultiplyChecked:
			case ExpressionType.NotEqual:
			case ExpressionType.Or:
			case ExpressionType.OrElse:
			case ExpressionType.Power:
			case ExpressionType.RightShift:
			case ExpressionType.Subtract:
			case ExpressionType.SubtractChecked:
				return VisitBinary((BinaryExpression)expression, context);
			case ExpressionType.TypeIs:
				return VisitTypeIs((TypeBinaryExpression)expression, context);
			case ExpressionType.Conditional:
				return VisitConditional((ConditionalExpression)expression, context);
			case ExpressionType.Constant:
				return VisitConstant((ConstantExpression)expression, context);
			case ExpressionType.Parameter:
				return VisitParameter((ParameterExpression)expression, context);
			case ExpressionType.MemberAccess:
				return VisitMemberAccess((MemberExpression)expression, context);
			case ExpressionType.Call:
				return VisitMethodCall((MethodCallExpression)expression, context);
			case ExpressionType.Lambda:
				return VisitLambda((LambdaExpression)expression, context);
			case ExpressionType.New:
				return VisitNew((NewExpression)expression, context);
			case ExpressionType.NewArrayInit:
			case ExpressionType.NewArrayBounds:
				return VisitNewArray((NewArrayExpression)expression, context);
			case ExpressionType.Invoke:
				return VisitInvocation((InvocationExpression)expression, context);
			case ExpressionType.MemberInit:
				return VisitMemberInit((MemberInitExpression)expression, context);
			case ExpressionType.ListInit:
				return VisitListInit((ListInitExpression)expression, context);
			default:
				return VisitUnknown(expression, context);
			}
		}

		protected virtual Expression VisitUnknown(Expression exp, T context)
		{
			throw new NotSupportedException(string.Format("Unexpected expression ({0})", exp.NodeType));
		}

		protected virtual MemberBinding VisitBinding(MemberBinding binding, T context)
		{
			switch (binding.BindingType)
			{
			case MemberBindingType.Assignment:
				return VisitMemberAssignment((MemberAssignment)binding, context);
			case MemberBindingType.MemberBinding:
				return VisitMemberMemberBinding((MemberMemberBinding)binding, context);
			case MemberBindingType.ListBinding:
				return VisitMemberListBinding((MemberListBinding)binding, context);
			default:
				throw new NotSupportedException(string.Format("Unexpected member binding ({0})", binding.BindingType));
			}
		}

		protected virtual ElementInit VisitElementInitializer(ElementInit initializer, T context)
		{
			IEnumerable<Expression> enumerable = VisitExpressionList(initializer.Arguments, context);
			if (enumerable == initializer.Arguments)
			{
				return initializer;
			}
			return Expression.ElementInit(initializer.AddMethod, enumerable);
		}

		protected virtual Expression VisitUnary(UnaryExpression unary, T context)
		{
			Expression expression = Visit(unary.Operand, context);
			if (expression == unary.Operand)
			{
				return unary;
			}
			return Expression.MakeUnary(unary.NodeType, expression, expression.Type, unary.Method);
		}

		protected virtual Expression VisitBinary(BinaryExpression binary, T context)
		{
			Expression expression = Visit(binary.Left, context);
			Expression expression2 = Visit(binary.Right, context);
			Expression expression3 = Visit(binary.Conversion, context);
			if (expression == binary.Left && expression2 == binary.Right && expression3 == binary.Conversion)
			{
				return binary;
			}
			if (binary.NodeType == ExpressionType.Coalesce && binary.Conversion != null)
			{
				return Expression.Coalesce(expression, expression2, expression3 as LambdaExpression);
			}
			return Expression.MakeBinary(binary.NodeType, expression, expression2, binary.IsLiftedToNull, binary.Method);
		}

		protected virtual Expression VisitTypeIs(TypeBinaryExpression binary, T context)
		{
			Expression expression = Visit(binary.Expression, context);
			if (expression == binary.Expression)
			{
				return binary;
			}
			return Expression.TypeIs(expression, binary.TypeOperand);
		}

		protected virtual Expression VisitConstant(ConstantExpression constant, T context)
		{
			return constant;
		}

		protected virtual Expression VisitConditional(ConditionalExpression conditional, T context)
		{
			Expression expression = Visit(conditional.Test, context);
			Expression expression2 = Visit(conditional.IfTrue, context);
			Expression expression3 = Visit(conditional.IfFalse, context);
			if (expression == conditional.Test && expression2 == conditional.IfTrue && expression3 == conditional.IfFalse)
			{
				return conditional;
			}
			return Expression.Condition(expression, expression2, expression3);
		}

		protected virtual ParameterExpression VisitParameter(ParameterExpression p, T context)
		{
			return p;
		}

		protected virtual IEnumerable<ParameterExpression> VisitParameterList(IList<ParameterExpression> original, T context)
		{
			List<ParameterExpression> list = null;
			int i = 0;
			for (int count = original.Count; i < count; i++)
			{
				ParameterExpression parameterExpression = VisitParameter(original[i], context);
				if (list != null)
				{
					list.Add(parameterExpression);
				}
				else if (parameterExpression != original[i])
				{
					list = new List<ParameterExpression>(count);
					for (int j = 0; j < i; j++)
					{
						list.Add(original[j]);
					}
					list.Add(parameterExpression);
				}
			}
			if (list == null)
			{
				return original;
			}
			return list.AsReadOnly();
		}

		protected virtual Expression VisitMemberAccess(MemberExpression m, T context)
		{
			Expression expression = Visit(m.Expression, context);
			if (expression == m.Expression)
			{
				return m;
			}
			return Expression.MakeMemberAccess(expression, m.Member);
		}

		protected virtual Expression VisitMethodCall(MethodCallExpression m, T context)
		{
			Expression expression = Visit(m.Object, context);
			IEnumerable<Expression> enumerable = VisitExpressionList(m.Arguments, context);
			if (expression == m.Object && enumerable == m.Arguments)
			{
				return m;
			}
			return Expression.Call(expression, m.Method, enumerable);
		}

		protected virtual IEnumerable<Expression> VisitExpressionList(IList<Expression> original, T context)
		{
			List<Expression> list = null;
			int i = 0;
			for (int count = original.Count; i < count; i++)
			{
				Expression expression = Visit(original[i], context);
				if (list != null)
				{
					list.Add(expression);
				}
				else if (expression != original[i])
				{
					list = new List<Expression>(count);
					for (int j = 0; j < i; j++)
					{
						list.Add(original[j]);
					}
					list.Add(expression);
				}
			}
			if (list == null)
			{
				return original;
			}
			return list.AsReadOnly();
		}

		protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment, T context)
		{
			Expression expression = Visit(assignment.Expression, context);
			if (expression == assignment.Expression)
			{
				return assignment;
			}
			return Expression.Bind(assignment.Member, expression);
		}

		protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding, T context)
		{
			IEnumerable<MemberBinding> enumerable = VisitBindingList(binding.Bindings, context);
			if (enumerable == binding.Bindings)
			{
				return binding;
			}
			return Expression.MemberBind(binding.Member, enumerable);
		}

		protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding, T context)
		{
			IEnumerable<ElementInit> enumerable = VisitElementInitializerList(binding.Initializers, context);
			if (enumerable == binding.Initializers)
			{
				return binding;
			}
			return Expression.ListBind(binding.Member, enumerable);
		}

		protected virtual IEnumerable<MemberBinding> VisitBindingList(IList<MemberBinding> original, T context)
		{
			List<MemberBinding> list = null;
			int i = 0;
			for (int count = original.Count; i < count; i++)
			{
				MemberBinding memberBinding = VisitBinding(original[i], context);
				if (list != null)
				{
					list.Add(memberBinding);
				}
				else if (memberBinding != original[i])
				{
					list = new List<MemberBinding>(count);
					for (int j = 0; j < i; j++)
					{
						list.Add(original[j]);
					}
					list.Add(memberBinding);
				}
			}
			if (list == null)
			{
				return original;
			}
			return list;
		}

		protected virtual IEnumerable<ElementInit> VisitElementInitializerList(IList<ElementInit> original, T context)
		{
			List<ElementInit> list = null;
			int i = 0;
			for (int count = original.Count; i < count; i++)
			{
				ElementInit elementInit = VisitElementInitializer(original[i], context);
				if (list != null)
				{
					list.Add(elementInit);
				}
				else if (elementInit != original[i])
				{
					list = new List<ElementInit>(count);
					for (int j = 0; j < i; j++)
					{
						list.Add(original[j]);
					}
					list.Add(elementInit);
				}
			}
			if (list == null)
			{
				return original;
			}
			return list;
		}

		protected virtual Expression VisitLambda(LambdaExpression lambda, T context)
		{
			Expression expression = Visit(lambda.Body, context);
			if (expression == lambda.Body)
			{
				return lambda;
			}
			return Expression.Lambda(lambda.Type, expression, lambda.Parameters);
		}

		protected virtual NewExpression VisitNew(NewExpression ctor, T context)
		{
			IEnumerable<Expression> enumerable = VisitExpressionList(ctor.Arguments, context);
			if (enumerable == ctor.Arguments)
			{
				return ctor;
			}
			if (ctor.Members == null)
			{
				return Expression.New(ctor.Constructor, enumerable);
			}
			return Expression.New(ctor.Constructor, enumerable, ctor.Members);
		}

		protected virtual Expression VisitMemberInit(MemberInitExpression init, T context)
		{
			NewExpression newExpression = VisitNew(init.NewExpression, context);
			IEnumerable<MemberBinding> enumerable = VisitBindingList(init.Bindings, context);
			if (newExpression == init.NewExpression && enumerable == init.Bindings)
			{
				return init;
			}
			return Expression.MemberInit(newExpression, enumerable);
		}

		protected virtual Expression VisitListInit(ListInitExpression init, T context)
		{
			NewExpression newExpression = VisitNew(init.NewExpression, context);
			IEnumerable<ElementInit> enumerable = VisitElementInitializerList(init.Initializers, context);
			if (newExpression == init.NewExpression && enumerable == init.Initializers)
			{
				return init;
			}
			return Expression.ListInit(newExpression, enumerable);
		}

		protected virtual Expression VisitNewArray(NewArrayExpression ctor, T context)
		{
			IEnumerable<Expression> enumerable = VisitExpressionList(ctor.Expressions, context);
			if (enumerable == ctor.Expressions)
			{
				return ctor;
			}
			if (ctor.NodeType == ExpressionType.NewArrayInit)
			{
				return Expression.NewArrayInit(ctor.Type.GetElementType(), enumerable);
			}
			return Expression.NewArrayBounds(ctor.Type.GetElementType(), enumerable);
		}

		protected virtual Expression VisitInvocation(InvocationExpression invoke, T context)
		{
			IEnumerable<Expression> enumerable = VisitExpressionList(invoke.Arguments, context);
			Expression expression = Visit(invoke.Expression, context);
			if (enumerable == invoke.Arguments && expression == invoke.Expression)
			{
				return invoke;
			}
			return Expression.Invoke(expression, enumerable);
		}

		protected static LambdaExpression GetLambda(Expression expression)
		{
			while (expression.NodeType == ExpressionType.Quote)
			{
				expression = ((UnaryExpression)expression).Operand;
			}
			if (expression.NodeType == ExpressionType.Constant)
			{
				return ((ConstantExpression)expression).Value as LambdaExpression;
			}
			return expression as LambdaExpression;
		}
	}
}
