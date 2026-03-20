using System;
using System.Reflection;
using System.Security;
using Relentless;

namespace JsonFx.CodeGen
{
	[SecuritySafeCritical]
	internal static class DynamicMethodGeneratorRS
	{
		public static IGetterHandler GetGetter(MemberInfo memberInfo)
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

		public static ISetterHandler GetSetter(MemberInfo memberInfo)
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

		public static IGetterHandler GetPropertyGetter(PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException("propertyInfo");
			}
			if (!propertyInfo.CanRead)
			{
				return null;
			}
			return new GetterHandler(propertyInfo);
		}

		public static ISetterHandler GetPropertySetter(PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException("propertyInfo");
			}
			if (!propertyInfo.CanWrite)
			{
				return null;
			}
			return new SetterHandler(propertyInfo);
		}

		public static IGetterHandler GetFieldGetter(FieldInfo fieldInfo)
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			return new GetterHandler(fieldInfo);
		}

		public static ISetterHandler GetFieldSetter(FieldInfo fieldInfo)
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			if (fieldInfo.IsInitOnly || fieldInfo.IsLiteral)
			{
				return null;
			}
			return new SetterHandler(fieldInfo);
		}

		public static IProxyHandler GetMethodProxy(Type declaringType, string methodName, params Type[] argTypes)
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

		public static IProxyHandler GetMethodProxy(MethodInfo methodInfo)
		{
			if (methodInfo == null)
			{
				throw new ArgumentNullException("methodInfo");
			}
			return new ProxyHandler(methodInfo);
		}

		public static IFactoryHandler GetTypeFactory(Type type)
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

		public static IFactoryHandler GetTypeFactory(Type type, params Type[] args)
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

		public static IFactoryHandler GetTypeFactory(ConstructorInfo ctor)
		{
			if (ctor == null)
			{
				throw new ArgumentNullException("ctor");
			}
			return new FactoryHandler(ctor);
		}
	}
}
