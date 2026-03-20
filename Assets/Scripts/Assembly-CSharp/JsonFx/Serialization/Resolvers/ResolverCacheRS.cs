using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace JsonFx.Serialization.Resolvers
{
	public sealed class ResolverCacheRS
	{
		private readonly IDictionary<Type, IDictionary<string, MemberMapRS>> MemberCache = new Dictionary<Type, IDictionary<string, MemberMapRS>>();

		private readonly IDictionary<Type, IDictionary<Enum, string>> EnumCache = new Dictionary<Type, IDictionary<Enum, string>>();

		private readonly IDictionary<Type, FactoryMapRS> Factories = new Dictionary<Type, FactoryMapRS>();

		private readonly IDictionary<Type, IEnumerable<DataName>> NameCache = new Dictionary<Type, IEnumerable<DataName>>();

		private readonly IResolverStrategyRS Strategy;

		public ResolverCacheRS(IResolverStrategyRS strategy)
		{
			if (strategy == null)
			{
				throw new ArgumentNullException("strategy");
			}
			Strategy = strategy;
		}

		public IEnumerable<MemberMapRS> SortMembers(IEnumerable<MemberMapRS> members)
		{
			return Strategy.SortMembers(members) ?? members;
		}

		public IEnumerable<DataName> LoadTypeName(Type type)
		{
			if (type == null)
			{
				return new DataName[1]
				{
					new DataName(type)
				};
			}
			try
			{
				IEnumerable<DataName> value;
				if (NameCache.TryGetValue(type, out value))
				{
					return value;
				}
			}
			finally
			{
			}
			IDictionary<string, MemberMapRS> maps;
			return BuildMap(type, out maps);
		}

		public IDictionary<string, MemberMapRS> LoadMaps(Type type)
		{
			if (type == null || type == typeof(object))
			{
				return null;
			}
			IDictionary<string, MemberMapRS> value;
			try
			{
				if (MemberCache.TryGetValue(type, out value))
				{
					return value;
				}
			}
			finally
			{
			}
			BuildMap(type, out value);
			return value;
		}

		public MemberMapRS LoadMemberMap(MemberInfo member)
		{
			IDictionary<string, MemberMapRS> dictionary = LoadMaps(member.DeclaringType);
			foreach (MemberMapRS value in dictionary.Values)
			{
				if (object.Equals(value.MemberInfo, member))
				{
					return value;
				}
			}
			return null;
		}

		public IDictionary<Enum, string> LoadEnumMaps(Type type)
		{
			if (type == null || !type.IsEnum)
			{
				return null;
			}
			IDictionary<Enum, string> value;
			try
			{
				if (EnumCache.TryGetValue(type, out value))
				{
					return value;
				}
			}
			finally
			{
			}
			BuildEnumMap(type, out value);
			return value;
		}

		public void Clear()
		{
			try
			{
				NameCache.Clear();
				MemberCache.Clear();
				EnumCache.Clear();
				Factories.Clear();
			}
			finally
			{
			}
		}

		private IEnumerable<DataName> BuildMap(Type objectType, out IDictionary<string, MemberMapRS> maps)
		{
			bool flag = false;
			IEnumerable<DataName> enumerable = Strategy.GetName(objectType);
			if (enumerable != null)
			{
				foreach (DataName item in enumerable)
				{
					if (!item.IsEmpty)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				enumerable = new DataName[1]
				{
					new DataName(objectType)
				};
			}
			if (objectType.IsEnum)
			{
				IDictionary<Enum, string> enumMaps;
				maps = BuildEnumMap(objectType, out enumMaps);
				return enumerable;
			}
			if (typeof(IDictionary<string, object>).IsAssignableFrom(objectType) || typeof(IDictionary).IsAssignableFrom(objectType))
			{
				try
				{
					IDictionary<string, MemberMapRS> dictionary = null;
					MemberCache[objectType] = dictionary;
					maps = dictionary;
					IEnumerable<DataName> enumerable2 = enumerable;
					NameCache[objectType] = enumerable2;
					return enumerable2;
				}
				finally
				{
				}
			}
			maps = new Dictionary<string, MemberMapRS>();
			bool isImmutableType = FactoryMapRS.IsImmutableType(objectType);
			PropertyInfo[] properties = objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo.GetIndexParameters().Length != 0 || Strategy.IsPropertyIgnored(propertyInfo, isImmutableType))
				{
					continue;
				}
				IEnumerable<DataName> enumerable3 = Strategy.GetName(propertyInfo);
				flag = false;
				if (enumerable3 != null)
				{
					foreach (DataName item2 in enumerable3)
					{
						if (!item2.IsEmpty)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					enumerable3 = new DataName[1]
					{
						new DataName(propertyInfo.Name)
					};
				}
				ValueIgnoredDelegate valueIgnoredCallback = Strategy.GetValueIgnoredCallback(propertyInfo);
				MemberMapRS memberMapRS = null;
				foreach (DataName item3 in enumerable3)
				{
					if (!item3.IsEmpty && !maps.ContainsKey(item3.LocalName))
					{
						if (memberMapRS == null)
						{
							memberMapRS = (maps[item3.LocalName] = new MemberMapRS(propertyInfo, item3, valueIgnoredCallback));
						}
						else
						{
							maps[item3.LocalName] = new MemberMapRS(memberMapRS, item3);
						}
					}
				}
			}
			FieldInfo[] fields = objectType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (Strategy.IsFieldIgnored(fieldInfo))
				{
					continue;
				}
				IEnumerable<DataName> enumerable4 = Strategy.GetName(fieldInfo);
				flag = false;
				if (enumerable4 != null)
				{
					foreach (DataName item4 in enumerable4)
					{
						if (!item4.IsEmpty)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					enumerable4 = new DataName[1]
					{
						new DataName(fieldInfo.Name)
					};
				}
				ValueIgnoredDelegate valueIgnoredCallback2 = Strategy.GetValueIgnoredCallback(fieldInfo);
				MemberMapRS memberMapRS3 = null;
				foreach (DataName item5 in enumerable4)
				{
					if (!item5.IsEmpty && !maps.ContainsKey(item5.LocalName))
					{
						if (memberMapRS3 == null)
						{
							memberMapRS3 = (maps[item5.LocalName] = new MemberMapRS(fieldInfo, item5, valueIgnoredCallback2));
						}
						else
						{
							maps[item5.LocalName] = new MemberMapRS(memberMapRS3, item5);
						}
					}
				}
			}
			try
			{
				MemberCache[objectType] = maps;
				IEnumerable<DataName> enumerable2 = enumerable;
				NameCache[objectType] = enumerable2;
				return enumerable2;
			}
			finally
			{
			}
		}

		private IDictionary<string, MemberMapRS> BuildEnumMap(Type enumType, out IDictionary<Enum, string> enumMaps)
		{
			if (enumType == null || !enumType.IsEnum)
			{
				enumMaps = null;
				return null;
			}
			bool flag = false;
			IEnumerable<DataName> enumerable = Strategy.GetName(enumType);
			if (enumerable != null)
			{
				foreach (DataName item in enumerable)
				{
					if (!item.IsEmpty)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				enumerable = new DataName[1]
				{
					new DataName(enumType)
				};
			}
			IDictionary<string, MemberMapRS> dictionary = new Dictionary<string, MemberMapRS>();
			enumMaps = new Dictionary<Enum, string>(new EnumEqualityComparer());
			FieldInfo[] fields = enumType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			foreach (FieldInfo fieldInfo in fields)
			{
				flag = false;
				IEnumerable<DataName> enumerable2 = Strategy.GetName(fieldInfo);
				if (enumerable2 != null)
				{
					foreach (DataName item2 in enumerable2)
					{
						if (!item2.IsEmpty)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					enumerable2 = new DataName[1]
					{
						new DataName(fieldInfo.Name)
					};
				}
				MemberMapRS memberMapRS = null;
				foreach (DataName item3 in enumerable2)
				{
					if (!item3.IsEmpty && !dictionary.ContainsKey(item3.LocalName))
					{
						if (memberMapRS == null)
						{
							memberMapRS = (dictionary[item3.LocalName] = new MemberMapRS(fieldInfo, item3, null));
							enumMaps[(Enum)memberMapRS.Getter.Call(null)] = item3.LocalName;
						}
						else
						{
							dictionary[item3.LocalName] = new MemberMapRS(memberMapRS, item3);
						}
					}
				}
			}
			try
			{
				NameCache[enumType] = enumerable;
				EnumCache[enumType] = enumMaps;
				IDictionary<string, MemberMapRS> dictionary2 = dictionary;
				MemberCache[enumType] = dictionary2;
				return dictionary2;
			}
			finally
			{
			}
		}

		public FactoryMapRS LoadFactory(Type type)
		{
			if (type == null || type == typeof(object))
			{
				return null;
			}
			FactoryMapRS value;
			try
			{
				if (Factories.TryGetValue(type, out value))
				{
					return value;
				}
			}
			finally
			{
			}
			value = new FactoryMapRS(type);
			try
			{
				FactoryMapRS factoryMapRS = value;
				Factories[type] = factoryMapRS;
				return factoryMapRS;
			}
			finally
			{
			}
		}
	}
}
