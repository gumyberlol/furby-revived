using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;
using JsonFx.CodeGen;
using JsonFx.Serialization;
using JsonFx.Serialization.Resolvers;

namespace JsonFx.Xml.Resolvers
{
	public class XmlResolverStrategy : PocoResolverStrategy
	{
		private const string SpecifiedSuffix = "Specified";

		private const string ShouldSerializePrefix = "ShouldSerialize";

		public override bool IsPropertyIgnored(PropertyInfo member, bool isImmutableType)
		{
			return base.IsPropertyIgnored(member, isImmutableType) || TypeCoercionUtility.HasAttribute<XmlIgnoreAttribute>(member);
		}

		public override bool IsFieldIgnored(FieldInfo member)
		{
			return base.IsFieldIgnored(member) || TypeCoercionUtility.HasAttribute<XmlIgnoreAttribute>(member);
		}

		public override ValueIgnoredDelegate GetValueIgnoredCallback(MemberInfo member)
		{
			Type type = member.ReflectedType ?? member.DeclaringType;
			GetterDelegate specifiedPropertyGetter = null;
			PropertyInfo property = type.GetProperty(member.Name + "Specified", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (property != null && property.PropertyType == typeof(bool))
			{
				specifiedPropertyGetter = DynamicMethodGenerator.GetPropertyGetter(property);
			}
			ProxyDelegate shouldSerializeProxy = null;
			MethodInfo method = type.GetMethod("ShouldSerialize" + member.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (method != null && method.ReturnType == typeof(bool) && method.GetParameters().Length == 0)
			{
				shouldSerializeProxy = DynamicMethodGenerator.GetMethodProxy(method);
			}
			DefaultValueAttribute attribute = TypeCoercionUtility.GetAttribute<DefaultValueAttribute>(member);
			if (attribute == null)
			{
				if (specifiedPropertyGetter == null)
				{
					if (shouldSerializeProxy == null)
					{
						return (object target, object value) => value == null;
					}
					return (object target, object value) => value == null || object.Equals(false, shouldSerializeProxy(target));
				}
				if (shouldSerializeProxy == null)
				{
					return (object target, object value) => value == null || object.Equals(false, specifiedPropertyGetter(target));
				}
				return (object target, object value) => value == null || object.Equals(false, shouldSerializeProxy(target)) || object.Equals(false, specifiedPropertyGetter(target));
			}
			object defaultValue = attribute.Value;
			if (specifiedPropertyGetter == null)
			{
				if (shouldSerializeProxy == null)
				{
					return (object target, object value) => value == null || object.Equals(defaultValue, value);
				}
				return (object target, object value) => value == null || object.Equals(defaultValue, value) || object.Equals(false, shouldSerializeProxy(target));
			}
			if (shouldSerializeProxy == null)
			{
				return (object target, object value) => value == null || object.Equals(defaultValue, value) || object.Equals(false, specifiedPropertyGetter(target));
			}
			return (object target, object value) => value == null || object.Equals(defaultValue, value) || object.Equals(false, shouldSerializeProxy(target)) || object.Equals(false, specifiedPropertyGetter(target));
		}

		public override IEnumerable<DataName> GetName(MemberInfo member)
		{
			if (member is Type)
			{
				XmlRootAttribute rootAttr = TypeCoercionUtility.GetAttribute<XmlRootAttribute>(member);
				if (rootAttr != null && !string.IsNullOrEmpty(rootAttr.ElementName))
				{
					yield return new DataName(rootAttr.ElementName, null, rootAttr.Namespace);
				}
				XmlTypeAttribute typeAttr = TypeCoercionUtility.GetAttribute<XmlTypeAttribute>(member);
				if (typeAttr != null && !string.IsNullOrEmpty(typeAttr.TypeName))
				{
					yield return new DataName(typeAttr.TypeName, null, typeAttr.Namespace);
				}
				yield break;
			}
			XmlElementAttribute elemAttr = TypeCoercionUtility.GetAttribute<XmlElementAttribute>(member);
			if (elemAttr != null && !string.IsNullOrEmpty(elemAttr.ElementName))
			{
				yield return new DataName(elemAttr.ElementName, null, elemAttr.Namespace);
			}
			XmlAttributeAttribute attrAttr = TypeCoercionUtility.GetAttribute<XmlAttributeAttribute>(member);
			if (attrAttr != null && !string.IsNullOrEmpty(attrAttr.AttributeName))
			{
				yield return new DataName(attrAttr.AttributeName, null, attrAttr.Namespace, true);
			}
			XmlArrayAttribute arrayAttr = TypeCoercionUtility.GetAttribute<XmlArrayAttribute>(member);
			if (arrayAttr != null && !string.IsNullOrEmpty(arrayAttr.ElementName))
			{
				yield return new DataName(arrayAttr.ElementName, null, arrayAttr.Namespace);
			}
			if (member is FieldInfo && ((FieldInfo)member).DeclaringType.IsEnum)
			{
				XmlEnumAttribute enumAttr = TypeCoercionUtility.GetAttribute<XmlEnumAttribute>(member);
				if (enumAttr != null && !string.IsNullOrEmpty(enumAttr.Name))
				{
					yield return new DataName(enumAttr.Name);
				}
			}
		}

		public override IEnumerable<MemberMap> SortMembers(IEnumerable<MemberMap> members)
		{
			int count;
			if (members is ICollection<MemberMap>)
			{
				count = ((ICollection<MemberMap>)members).Count;
				if (count <= 1)
				{
					foreach (MemberMap member in members)
					{
						yield return member;
					}
					yield break;
				}
			}
			else
			{
				count = 5;
			}
			List<MemberMap> elements = new List<MemberMap>(count);
			foreach (MemberMap map in members)
			{
				DataName dataName = map.DataName;
				if (dataName.IsAttribute)
				{
					yield return map;
				}
				else
				{
					elements.Add(map);
				}
			}
			foreach (MemberMap item in elements)
			{
				yield return item;
			}
		}
	}
}
