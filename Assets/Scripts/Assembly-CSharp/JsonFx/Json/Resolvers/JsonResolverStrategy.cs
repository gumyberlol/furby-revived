using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using JsonFx.CodeGen;
using JsonFx.Serialization;
using JsonFx.Serialization.Resolvers;

namespace JsonFx.Json.Resolvers
{
	public class JsonResolverStrategy : PocoResolverStrategy
	{
		public override bool IsPropertyIgnored(PropertyInfo member, bool isImmutableType)
		{
			return base.IsPropertyIgnored(member, isImmutableType) || TypeCoercionUtility.HasAttribute<JsonIgnoreAttribute>(member);
		}

		public override bool IsFieldIgnored(FieldInfo member)
		{
			return base.IsFieldIgnored(member) || TypeCoercionUtility.HasAttribute<JsonIgnoreAttribute>(member);
		}

		public override ValueIgnoredDelegate GetValueIgnoredCallback(MemberInfo member)
		{
			Type type = member.ReflectedType ?? member.DeclaringType;
			JsonSpecifiedPropertyAttribute attribute = TypeCoercionUtility.GetAttribute<JsonSpecifiedPropertyAttribute>(member);
			GetterDelegate specifiedPropertyGetter = null;
			if (attribute != null && !string.IsNullOrEmpty(attribute.SpecifiedProperty))
			{
				PropertyInfo property = type.GetProperty(attribute.SpecifiedProperty, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				if (property != null && property.PropertyType == typeof(bool))
				{
					specifiedPropertyGetter = DynamicMethodGenerator.GetPropertyGetter(property);
				}
			}
			DefaultValueAttribute attribute2 = TypeCoercionUtility.GetAttribute<DefaultValueAttribute>(member);
			if (attribute2 == null)
			{
				if (specifiedPropertyGetter == null)
				{
					return null;
				}
				return (object target, object value) => object.Equals(false, specifiedPropertyGetter(target));
			}
			object defaultValue = attribute2.Value;
			if (specifiedPropertyGetter == null)
			{
				return (object target, object value) => object.Equals(defaultValue, value);
			}
			return (object target, object value) => object.Equals(defaultValue, value) || object.Equals(false, specifiedPropertyGetter(target));
		}

		public override IEnumerable<DataName> GetName(MemberInfo member)
		{
			JsonNameAttribute attr = TypeCoercionUtility.GetAttribute<JsonNameAttribute>(member);
			if (attr != null && attr.Name != null)
			{
				yield return new DataName(attr.Name);
			}
		}
	}
}
