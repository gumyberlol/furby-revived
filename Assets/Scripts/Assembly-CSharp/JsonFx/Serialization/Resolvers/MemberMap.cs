using System;
using System.Reflection;
using JsonFx.CodeGen;

namespace JsonFx.Serialization.Resolvers
{
	public sealed class MemberMap
	{
		public readonly MemberInfo MemberInfo;

		public readonly string Name;

		public readonly DataName DataName;

		public readonly Type Type;

		public readonly GetterDelegate Getter;

		public readonly SetterDelegate Setter;

		public readonly ValueIgnoredDelegate IsIgnored;

		public readonly bool IsAlternate;

		public MemberMap(MemberMap map, DataName dataName)
		{
			MemberInfo = map.MemberInfo;
			Type = map.Type;
			Getter = map.Getter;
			Setter = map.Setter;
			IsIgnored = map.IsIgnored;
			DataName = dataName;
			IsAlternate = true;
		}

		public MemberMap(PropertyInfo propertyInfo, DataName dataName, ValueIgnoredDelegate isIgnored)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException("propertyInfo");
			}
			DataName = dataName;
			MemberInfo = propertyInfo;
			Name = propertyInfo.Name;
			Type = propertyInfo.PropertyType;
			Getter = DynamicMethodGenerator.GetPropertyGetter(propertyInfo);
			Setter = DynamicMethodGenerator.GetPropertySetter(propertyInfo);
			IsIgnored = isIgnored;
		}

		public MemberMap(FieldInfo fieldInfo, DataName dataName, ValueIgnoredDelegate isIgnored)
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			DataName = dataName;
			MemberInfo = fieldInfo;
			Name = fieldInfo.Name;
			Type = fieldInfo.FieldType;
			Getter = DynamicMethodGenerator.GetFieldGetter(fieldInfo);
			Setter = DynamicMethodGenerator.GetFieldSetter(fieldInfo);
			IsIgnored = isIgnored;
		}
	}
}
