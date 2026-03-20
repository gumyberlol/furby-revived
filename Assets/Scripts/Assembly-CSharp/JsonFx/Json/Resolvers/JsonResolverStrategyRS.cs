using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using JsonFx.CodeGen;
using JsonFx.Serialization;
using JsonFx.Serialization.Resolvers;
using Relentless;

namespace JsonFx.Json.Resolvers
{
	public class JsonResolverStrategyRS : PocoResolverStrategyRS
	{
		public override bool IsPropertyIgnored(PropertyInfo member, bool isImmutableType)
		{
			return base.IsPropertyIgnored(member, isImmutableType) || TypeCoercionUtilityRS.HasAttribute<JsonIgnoreAttribute>(member);
		}

		public override bool IsFieldIgnored(FieldInfo member)
		{
			return base.IsFieldIgnored(member) || TypeCoercionUtilityRS.HasAttribute<JsonIgnoreAttribute>(member);
		}

		public override ValueIgnoredDelegate GetValueIgnoredCallback(MemberInfo member)
		{
			Type type = member.ReflectedType ?? member.DeclaringType;
			JsonSpecifiedPropertyAttribute attribute = TypeCoercionUtilityRS.GetAttribute<JsonSpecifiedPropertyAttribute>(member);
			IGetterHandler specifiedPropertyGetter = null;
			if (attribute != null && !string.IsNullOrEmpty(attribute.SpecifiedProperty))
			{
				PropertyInfo property = type.GetProperty(attribute.SpecifiedProperty, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				if (property != null && property.PropertyType == typeof(bool))
				{
					specifiedPropertyGetter = DynamicMethodGeneratorRS.GetPropertyGetter(property);
				}
			}
			DefaultValueAttribute attribute2 = TypeCoercionUtilityRS.GetAttribute<DefaultValueAttribute>(member);
			if (attribute2 == null)
			{
				if (specifiedPropertyGetter == null)
				{
					return null;
				}
				return (object target, object value) => object.Equals(false, specifiedPropertyGetter.Call(target));
			}
			object defaultValue = attribute2.Value;
			if (specifiedPropertyGetter == null)
			{
				return (object target, object value) => object.Equals(defaultValue, value);
			}
			return (object target, object value) => object.Equals(defaultValue, value) || object.Equals(false, specifiedPropertyGetter.Call(target));
		}

		public override IEnumerable<DataName> GetName(MemberInfo member)
		{
			JsonNameAttribute attr = TypeCoercionUtilityRS.GetAttribute<JsonNameAttribute>(member);
			if (attr != null && attr.Name != null)
			{
				yield return new DataName(attr.Name);
			}
		}
	}
}
