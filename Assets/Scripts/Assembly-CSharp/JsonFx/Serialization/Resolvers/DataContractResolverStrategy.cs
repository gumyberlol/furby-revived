using System;
using System.Collections.Generic;
using System.Reflection;
using JsonFx.CodeGen;

namespace JsonFx.Serialization.Resolvers
{
	public class DataContractResolverStrategy : PocoResolverStrategy
	{
		private const string DataContractAssemblyName = "System.Runtime.Serialization";

		private const string DataContractTypeName = "System.Runtime.Serialization.DataContractAttribute";

		private const string DataMemberTypeName = "System.Runtime.Serialization.DataMemberAttribute";

		private const string IgnoreDataMemberTypeName = "System.Runtime.Serialization.IgnoreDataMemberAttribute";

		private static readonly Type DataContractType;

		private static readonly Type DataMemberType;

		private static readonly Type IgnoreDataMemberType;

		private static readonly GetterDelegate DataContractNameGetter;

		private static readonly GetterDelegate DataContractNamespaceGetter;

		private static readonly GetterDelegate DataMemberNameGetter;

		static DataContractResolverStrategy()
		{
			string[] array = typeof(object).Assembly.FullName.Split(',');
			array[0] = "System.Runtime.Serialization";
			Assembly assembly = Assembly.Load(string.Join(",", array));
			DataContractType = assembly.GetType("System.Runtime.Serialization.DataContractAttribute");
			DataMemberType = assembly.GetType("System.Runtime.Serialization.DataMemberAttribute");
			IgnoreDataMemberType = assembly.GetType("System.Runtime.Serialization.IgnoreDataMemberAttribute");
			if (DataContractType != null)
			{
				PropertyInfo property = DataContractType.GetProperty("Name", BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
				DataContractNameGetter = DynamicMethodGenerator.GetPropertyGetter(property);
				property = DataContractType.GetProperty("Namespace", BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
				DataContractNamespaceGetter = DynamicMethodGenerator.GetPropertyGetter(property);
			}
			if (DataMemberType != null)
			{
				PropertyInfo property2 = DataMemberType.GetProperty("Name", BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
				DataMemberNameGetter = DynamicMethodGenerator.GetPropertyGetter(property2);
			}
		}

		public override bool IsPropertyIgnored(PropertyInfo member, bool isImmutableType)
		{
			Type info = member.ReflectedType ?? member.DeclaringType;
			if (TypeCoercionUtility.HasAttribute(info, DataContractType))
			{
				return !TypeCoercionUtility.HasAttribute(member, DataMemberType) || TypeCoercionUtility.HasAttribute(member, IgnoreDataMemberType);
			}
			return base.IsPropertyIgnored(member, isImmutableType);
		}

		public override bool IsFieldIgnored(FieldInfo member)
		{
			Type info = member.ReflectedType ?? member.DeclaringType;
			if (TypeCoercionUtility.HasAttribute(info, DataContractType))
			{
				return !TypeCoercionUtility.HasAttribute(member, DataMemberType) || TypeCoercionUtility.HasAttribute(member, IgnoreDataMemberType);
			}
			return base.IsFieldIgnored(member);
		}

		public override ValueIgnoredDelegate GetValueIgnoredCallback(MemberInfo member)
		{
			return null;
		}

		public override IEnumerable<DataName> GetName(MemberInfo member)
		{
			Attribute typeAttr;
			string ns;
			if (member is Type)
			{
				typeAttr = TypeCoercionUtility.GetAttribute(member, DataContractType);
				if (typeAttr != null)
				{
					string localName = (string)DataContractNameGetter(typeAttr);
					ns = (string)DataContractNamespaceGetter(typeAttr);
					if (!string.IsNullOrEmpty(localName))
					{
						yield return new DataName(localName, null, ns);
					}
				}
				yield break;
			}
			typeAttr = TypeCoercionUtility.GetAttribute(member.DeclaringType, DataContractType);
			if (typeAttr == null)
			{
				yield break;
			}
			ns = (string)DataContractNamespaceGetter(typeAttr);
			Attribute memberAttr = TypeCoercionUtility.GetAttribute(member, DataMemberType);
			if (memberAttr != null)
			{
				string localName = (string)DataMemberNameGetter(memberAttr);
				if (!string.IsNullOrEmpty(localName))
				{
					yield return new DataName(localName, null, ns);
				}
			}
		}
	}
}
