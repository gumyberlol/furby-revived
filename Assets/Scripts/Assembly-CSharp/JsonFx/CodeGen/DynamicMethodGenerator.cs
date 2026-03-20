using System;
using System.Reflection;
using System.Security;

namespace JsonFx.CodeGen
{
	[SecuritySafeCritical]
	internal static class DynamicMethodGenerator
	{
		public static GetterDelegate GetGetter(MemberInfo memberInfo)
		{
			if (memberInfo is PropertyInfo)
			{
				return GetPropertyGetter((PropertyInfo)memberInfo);
			}
			if (memberInfo is FieldInfo)
			{
				return GetFieldGetter((FieldInfo)memberInfo);
			}
			return null;
		}

		public static SetterDelegate GetSetter(MemberInfo memberInfo)
		{
			if (memberInfo is PropertyInfo)
			{
				return GetPropertySetter((PropertyInfo)memberInfo);
			}
			if (memberInfo is FieldInfo)
			{
				return GetFieldSetter((FieldInfo)memberInfo);
			}
			return null;
		}

		public static GetterDelegate GetPropertyGetter(PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException("propertyInfo");
			}
			if (!propertyInfo.CanRead)
			{
				return null;
			}
			MethodInfo getMethod = propertyInfo.GetGetMethod(true);
			if (getMethod == null)
			{
				return null;
			}
			return null;
		}

		public static SetterDelegate GetPropertySetter(PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException("propertyInfo");
			}
			if (!propertyInfo.CanWrite)
			{
				return null;
			}
			MethodInfo setMethod = propertyInfo.GetSetMethod(true);
			if (setMethod == null)
			{
				return null;
			}
			return null;
		}

		public static GetterDelegate GetFieldGetter(FieldInfo fieldInfo)
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			return null;
		}

		public static SetterDelegate GetFieldSetter(FieldInfo fieldInfo)
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			if (fieldInfo.IsInitOnly || fieldInfo.IsLiteral)
			{
				return null;
			}
			return null;
		}

		public static ProxyDelegate GetMethodProxy(Type declaringType, string methodName, params Type[] argTypes)
		{
			if (declaringType == null)
			{
				throw new ArgumentNullException("declaringType");
			}
			if (string.IsNullOrEmpty(methodName))
			{
				throw new ArgumentNullException("methodName");
			}
			MethodInfo methodInfo = ((argTypes.Length <= 0) ? declaringType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy) : declaringType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, null, argTypes, null));
			if (methodInfo == null)
			{
				return null;
			}
			return GetMethodProxy(methodInfo);
		}

		public static ProxyDelegate GetMethodProxy(MethodInfo methodInfo)
		{
			if (methodInfo == null)
			{
				throw new ArgumentNullException("methodInfo");
			}
			return null;
		}

		public static FactoryDelegate GetTypeFactory(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, null, Type.EmptyTypes, null);
			if (constructor == null)
			{
				return null;
			}
			return GetTypeFactory(constructor);
		}

		public static FactoryDelegate GetTypeFactory(Type type, params Type[] args)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, null, args, null);
			if (constructor == null)
			{
				return null;
			}
			return GetTypeFactory(constructor);
		}

		public static FactoryDelegate GetTypeFactory(ConstructorInfo ctor)
		{
			if (ctor == null)
			{
				throw new ArgumentNullException("ctor");
			}
			return null;
		}
	}
}
