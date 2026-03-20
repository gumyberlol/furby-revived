using System;
using System.Reflection;
using JsonFx.CodeGen;
using Relentless;

namespace JsonFx.Serialization.Resolvers
{
	public sealed class MemberMapRS
	{
		public readonly MemberInfo MemberInfo;

		public readonly string Name;

		public readonly DataName DataName;

		public readonly Type Type;

		public readonly IGetterHandler Getter;

		public readonly ISetterHandler Setter;

		public readonly ValueIgnoredDelegate IsIgnored;

		public readonly bool IsAlternate;

		public MemberMapRS(MemberMapRS map, DataName dataName)
		{
			MemberInfo = map.MemberInfo;
			Type = map.Type;
			Getter = map.Getter;
			Setter = map.Setter;
			IsIgnored = map.IsIgnored;
			DataName = dataName;
			IsAlternate = true;
		}

		public MemberMapRS(PropertyInfo propertyInfo, DataName dataName, ValueIgnoredDelegate isIgnored)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException("propertyInfo");
			}
			DataName = dataName;
			MemberInfo = propertyInfo;
			Name = propertyInfo.Name;
			Type = propertyInfo.PropertyType;
			Getter = DynamicMethodGeneratorRS.GetPropertyGetter(propertyInfo);
			Setter = DynamicMethodGeneratorRS.GetPropertySetter(propertyInfo);
			IsIgnored = isIgnored;
		}

		public MemberMapRS(FieldInfo fieldInfo, DataName dataName, ValueIgnoredDelegate isIgnored)
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			DataName = dataName;
			MemberInfo = fieldInfo;
			Name = fieldInfo.Name;
			Type = fieldInfo.FieldType;
			Getter = DynamicMethodGeneratorRS.GetFieldGetter(fieldInfo);
			Setter = DynamicMethodGeneratorRS.GetFieldSetter(fieldInfo);
			IsIgnored = isIgnored;
		}
	}
}
