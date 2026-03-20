using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using JsonFx.Model.Filters;
using JsonFx.Serialization.Resolvers;

namespace JsonFx.Serialization
{
	public sealed class TypeCoercionUtility : IResolverCacheContainer
	{
		internal const string AnonymousTypePrefix = "<>f__AnonymousType";

		private const string ErrorNullValueType = "{0} does not accept null as a value";

		private const string ErrorCtor = "Unable to find a suitable constructor for instantiating the target Type. ({0})";

		private const string ErrorCannotInstantiateAsT = "Type {0} is not of Type {1}";

		private const string ErrorGenericIDictionary = "Types which implement Generic IDictionary<TKey, TValue> also need to implement IDictionary to be deserialized. ({0})";

		private const string ErrorGenericIDictionaryKeys = "Types which implement Generic IDictionary<TKey, TValue> need to have string keys to be deserialized. ({0})";

		internal static readonly string TypeGenericIEnumerable = typeof(IEnumerable<>).FullName;

		internal static readonly string TypeGenericICollection = typeof(ICollection<>).FullName;

		internal static readonly string TypeGenericIDictionary = typeof(IDictionary<, >).FullName;

		private readonly bool AllowNullValueTypes;

		private readonly ResolverCache ResolverCache;

		ResolverCache IResolverCacheContainer.ResolverCache
		{
			get
			{
				return ResolverCache;
			}
		}

		public TypeCoercionUtility(IResolverCacheContainer cacheContainer, bool allowNullValueTypes)
			: this(cacheContainer.ResolverCache, allowNullValueTypes)
		{
		}

		public TypeCoercionUtility(ResolverCache resolverCache, bool allowNullValueTypes)
		{
			if (resolverCache == null)
			{
				throw new ArgumentNullException("resolverCache");
			}
			ResolverCache = resolverCache;
			AllowNullValueTypes = allowNullValueTypes;
		}

		internal object InstantiateObjectDefaultCtor(Type targetType)
		{
			if (targetType == null || targetType.IsValueType || targetType.IsAbstract || targetType == typeof(object) || targetType == typeof(string))
			{
				return new Dictionary<string, object>();
			}
			targetType = ResolveInterfaceType(targetType);
			if (targetType.IsInterface)
			{
				return new Dictionary<string, object>();
			}
			FactoryMap factoryMap = ResolverCache.LoadFactory(targetType);
			if (factoryMap == null || factoryMap.Ctor == null || (factoryMap.CtorArgs != null && factoryMap.CtorArgs.Length > 0))
			{
				return new Dictionary<string, object>();
			}
			return factoryMap.Ctor();
		}

		internal object InstantiateObject(Type targetType, object args)
		{
			targetType = ResolveInterfaceType(targetType);
			FactoryMap factoryMap = ResolverCache.LoadFactory(targetType);
			if (factoryMap == null || factoryMap.Ctor == null)
			{
				throw new TypeCoercionException(string.Format("Unable to find a suitable constructor for instantiating the target Type. ({0})", targetType.FullName));
			}
			if (factoryMap.CtorArgs == null || factoryMap.CtorArgs.Length < 1)
			{
				return factoryMap.Ctor();
			}
			object[] array = new object[factoryMap.CtorArgs.Length];
			IDictionary<string, object> dictionary = args as IDictionary<string, object>;
			if (dictionary != null)
			{
				int i = 0;
				for (int num = array.Length; i < num; i++)
				{
					string name = factoryMap.CtorArgs[i].Name;
					Type parameterType = factoryMap.CtorArgs[i].ParameterType;
					foreach (string key in dictionary.Keys)
					{
						try
						{
							if (StringComparer.OrdinalIgnoreCase.Equals(key, name))
							{
								array[i] = CoerceType(parameterType, dictionary[key]);
								break;
							}
						}
						catch
						{
						}
					}
				}
			}
			else
			{
				IDictionary dictionary2 = args as IDictionary;
				if (dictionary2 != null)
				{
					int j = 0;
					for (int num2 = array.Length; j < num2; j++)
					{
						string name2 = factoryMap.CtorArgs[j].Name;
						Type parameterType2 = factoryMap.CtorArgs[j].ParameterType;
						foreach (string key2 in dictionary2.Keys)
						{
							try
							{
								if (StringComparer.OrdinalIgnoreCase.Equals(key2, name2))
								{
									array[j] = CoerceType(parameterType2, dictionary2[key2]);
									break;
								}
							}
							catch
							{
							}
						}
					}
				}
			}
			return factoryMap.Ctor(array);
		}

		internal void SetMemberValue(object target, Type targetType, MemberMap memberMap, string memberName, object memberValue)
		{
			if (target == null)
			{
				return;
			}
			if (target is IDictionary<string, object>)
			{
				((IDictionary<string, object>)target)[memberName] = memberValue;
				return;
			}
			if (target is IDictionary)
			{
				((IDictionary)target)[memberName] = memberValue;
				return;
			}
			if (targetType != null && targetType.GetInterface(TypeGenericIDictionary, false) != null)
			{
				throw new TypeCoercionException(string.Format("Types which implement Generic IDictionary<TKey, TValue> also need to implement IDictionary to be deserialized. ({0})", targetType));
			}
			if (memberMap != null && memberMap.Setter != null)
			{
				if (memberValue == null)
				{
					memberMap.Setter(target, (!memberMap.Type.IsValueType) ? null : Activator.CreateInstance(memberMap.Type, true));
				}
				else
				{
					memberMap.Setter(target, CoerceType(memberMap.Type, memberValue));
				}
			}
		}

		public T CoerceType<T>(object value)
		{
			return (T)CoerceType(typeof(T), value);
		}

		public object CoerceType(Type targetType, object value)
		{
			if (targetType == null || targetType == typeof(object))
			{
				return value;
			}
			bool flag = IsNullable(targetType);
			if (value == null)
			{
				if (!AllowNullValueTypes && targetType.IsValueType && !flag)
				{
					throw new TypeCoercionException(string.Format("{0} does not accept null as a value", targetType.FullName));
				}
				return value;
			}
			if (flag)
			{
				Type[] genericArguments = targetType.GetGenericArguments();
				if (genericArguments.Length == 1)
				{
					targetType = genericArguments[0];
				}
			}
			Type type = value.GetType();
			if (targetType.IsAssignableFrom(type))
			{
				return value;
			}
			if (targetType.IsEnum)
			{
				if (value is string)
				{
					if (!Enum.IsDefined(targetType, value))
					{
						IDictionary<string, MemberMap> dictionary = ResolverCache.LoadMaps(targetType);
						if (dictionary != null && dictionary.ContainsKey((string)value))
						{
							value = dictionary[(string)value].Name;
						}
					}
					return Enum.Parse(targetType, (string)value, false);
				}
				value = CoerceType(Enum.GetUnderlyingType(targetType), value);
				return Enum.ToObject(targetType, value);
			}
			if (value is IDictionary<string, object>)
			{
				return CoerceType(targetType, (IDictionary<string, object>)value);
			}
			if (value is IDictionary)
			{
				return CoerceType(targetType, (IDictionary)value);
			}
			if (typeof(IEnumerable).IsAssignableFrom(targetType) && typeof(IEnumerable).IsAssignableFrom(type))
			{
				return CoerceList(targetType, type, (IEnumerable)value);
			}
			if (value is string)
			{
				if (targetType == typeof(DateTime))
				{
					DateTime value2;
					try
					{
						value2 = DateTime.Parse((string)value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.RoundtripKind);
						if (value2.Kind == DateTimeKind.Local)
						{
							return value2.ToUniversalTime();
						}
						return value2;
					}
					catch (Exception)
					{
					}
					if (MSAjaxDateFilter.TryParseMSAjaxDate((string)value, out value2))
					{
						return value2;
					}
				}
				else
				{
					if (targetType == typeof(Guid))
					{
						return new Guid((string)value);
					}
					if (targetType == typeof(char))
					{
						if (((string)value).Length == 1)
						{
							return ((string)value)[0];
						}
					}
					else if (targetType == typeof(Uri))
					{
						Uri result;
						if (Uri.TryCreate((string)value, UriKind.RelativeOrAbsolute, out result))
						{
							return result;
						}
					}
					else
					{
						if (targetType == typeof(Version))
						{
							return new Version((string)value);
						}
						if (targetType == typeof(TimeSpan))
						{
							try
							{
								return TimeSpan.FromTicks(long.Parse((string)value));
							}
							catch (Exception)
							{
							}
							try
							{
								return TimeSpan.Parse((string)value);
							}
							catch (Exception)
							{
							}
						}
					}
				}
			}
			else if (targetType == typeof(TimeSpan))
			{
				return new TimeSpan((long)CoerceType(typeof(long), value));
			}
			TypeConverter converter = TypeDescriptor.GetConverter(targetType);
			if (converter.CanConvertFrom(type))
			{
				return converter.ConvertFrom(value);
			}
			converter = TypeDescriptor.GetConverter(type);
			if (converter.CanConvertTo(targetType))
			{
				return converter.ConvertTo(value, targetType);
			}
			try
			{
				return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
			}
			catch (Exception innerException)
			{
				throw new TypeCoercionException(string.Format("Error converting {0} to {1}", value.GetType().FullName, targetType.FullName), innerException);
			}
		}

		private object CoerceType(Type targetType, IDictionary<string, object> value)
		{
			object obj = InstantiateObject(targetType, value);
			IDictionary<string, MemberMap> dictionary = ResolverCache.LoadMaps(targetType);
			if (dictionary == null)
			{
				IDictionary<string, object> dictionary2 = obj as IDictionary<string, object>;
				if (dictionary2 != null)
				{
					foreach (string key in value.Keys)
					{
						dictionary2[key] = value[key];
					}
				}
				else
				{
					IDictionary dictionary3 = obj as IDictionary;
					if (dictionary3 != null)
					{
						foreach (string key2 in value.Keys)
						{
							dictionary3[key2] = value[key2];
						}
					}
				}
			}
			else
			{
				foreach (string key3 in value.Keys)
				{
					MemberMap value2;
					if (dictionary.TryGetValue(key3, out value2) && value2 != null && value2.Setter != null)
					{
						value2.Setter(obj, value[key3]);
					}
				}
			}
			return obj;
		}

		private object CoerceType(Type targetType, IDictionary value)
		{
			object obj = InstantiateObject(targetType, value);
			IDictionary<string, MemberMap> dictionary = ResolverCache.LoadMaps(targetType);
			if (dictionary == null)
			{
				IDictionary<string, object> dictionary2 = obj as IDictionary<string, object>;
				if (dictionary2 != null)
				{
					foreach (object key3 in value.Keys)
					{
						string key = Convert.ToString(key3, CultureInfo.InvariantCulture);
						dictionary2[key] = value[key];
					}
				}
				else
				{
					IDictionary dictionary3 = obj as IDictionary;
					if (dictionary3 != null)
					{
						foreach (object key4 in value.Keys)
						{
							dictionary3[key4] = value[key4];
						}
					}
				}
			}
			else
			{
				foreach (object key5 in value.Keys)
				{
					string key2 = Convert.ToString(key5, CultureInfo.InvariantCulture);
					MemberMap value2;
					if (dictionary.TryGetValue(key2, out value2) && value2 != null && value2.Setter != null)
					{
						value2.Setter(obj, value[key5]);
					}
				}
			}
			return obj;
		}

		private object CoerceList(Type targetType, Type valueType, IEnumerable value)
		{
			targetType = ResolveInterfaceType(targetType);
			if (targetType.IsArray)
			{
				return CoerceArray(targetType.GetElementType(), value);
			}
			FactoryMap factoryMap = ResolverCache.LoadFactory(targetType);
			if (factoryMap == null)
			{
				throw new TypeCoercionException(string.Format("Unable to find a suitable constructor for instantiating the target Type. ({0})", targetType.FullName));
			}
			foreach (Type argType in factoryMap.ArgTypes)
			{
				if (argType.IsAssignableFrom(valueType))
				{
					try
					{
						return factoryMap[argType](value);
					}
					catch
					{
					}
				}
			}
			if (factoryMap.Ctor == null)
			{
				throw new TypeCoercionException(string.Format("Unable to find a suitable constructor for instantiating the target Type. ({0})", targetType.FullName));
			}
			if (factoryMap.AddRange != null && factoryMap.AddRangeType != null && factoryMap.AddRangeType.IsAssignableFrom(valueType))
			{
				object obj2 = factoryMap.Ctor();
				factoryMap.AddRange(obj2, value);
				return obj2;
			}
			if (factoryMap.Add != null && factoryMap.AddType != null)
			{
				object obj3 = factoryMap.Ctor();
				Type addType = factoryMap.AddType;
				{
					foreach (object item in value)
					{
						factoryMap.Add(obj3, CoerceType(addType, item));
					}
					return obj3;
				}
			}
			try
			{
				return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
			}
			catch (Exception innerException)
			{
				throw new TypeCoercionException(string.Format("Error converting {0} to {1}", value.GetType().FullName, targetType.FullName), innerException);
			}
		}

		internal object CoerceCollection(Type targetType, Type itemType, ICollection value)
		{
			if (targetType != null && targetType != typeof(object))
			{
				return CoerceList(targetType, value.GetType(), value);
			}
			Array array = Array.CreateInstance(itemType ?? typeof(object), value.Count);
			value.CopyTo(array, 0);
			return array;
		}

		private Array CoerceArray(Type itemType, IEnumerable value)
		{
			ICollection collection = value as ICollection;
			if (collection == null)
			{
				List<object> list = new List<object>();
				foreach (object item in value)
				{
					list.Add(CoerceType(itemType, item));
				}
				collection = list;
			}
			Array array = Array.CreateInstance(itemType ?? typeof(object), collection.Count);
			collection.CopyTo(array, 0);
			return array;
		}

		private static Type ResolveInterfaceType(Type targetType)
		{
			if (targetType.IsInterface)
			{
				if (targetType.IsGenericType)
				{
					Type genericTypeDefinition = targetType.GetGenericTypeDefinition();
					if (genericTypeDefinition == typeof(IList<>) || genericTypeDefinition == typeof(IEnumerable<>) || genericTypeDefinition == typeof(IQueryable<>) || genericTypeDefinition == typeof(IOrderedQueryable<>) || genericTypeDefinition == typeof(ICollection<>))
					{
						Type[] genericArguments = targetType.GetGenericArguments();
						targetType = typeof(List<>).MakeGenericType(genericArguments);
					}
					else if (genericTypeDefinition == typeof(IDictionary<, >))
					{
						Type[] genericArguments2 = targetType.GetGenericArguments();
						targetType = ((genericArguments2.Length != 2 || genericArguments2[0] != typeof(string) || genericArguments2[0] != typeof(object)) ? typeof(Dictionary<, >).MakeGenericType(genericArguments2) : typeof(Dictionary<string, object>));
					}
				}
				else if (targetType == typeof(IList) || targetType == typeof(IEnumerable) || targetType == typeof(IQueryable) || targetType == typeof(IOrderedQueryable) || targetType == typeof(ICollection))
				{
					targetType = typeof(object[]);
				}
				else if (targetType == typeof(IDictionary))
				{
					targetType = typeof(Dictionary<string, object>);
				}
			}
			return targetType;
		}

		internal static Type GetDictionaryItemType(Type targetType)
		{
			if (targetType == null)
			{
				return null;
			}
			Type type = null;
			type = targetType.GetInterface(TypeGenericIDictionary, false);
			if (type == null)
			{
				return null;
			}
			Type[] genericArguments = type.GetGenericArguments();
			if (genericArguments.Length != 2 || !genericArguments[0].IsAssignableFrom(typeof(string)))
			{
				if (typeof(IDictionary).IsAssignableFrom(targetType))
				{
					return null;
				}
				throw new ArgumentException(string.Format("Types which implement Generic IDictionary<TKey, TValue> need to have string keys to be deserialized. ({0})", targetType));
			}
			return genericArguments[1];
		}

		internal static Type GetElementType(Type targetType)
		{
			if (targetType == null || targetType == typeof(string))
			{
				return null;
			}
			if (targetType.HasElementType)
			{
				return targetType.GetElementType();
			}
			Type type = null;
			type = targetType.GetInterface(TypeGenericIEnumerable, false);
			if (type == null)
			{
				return null;
			}
			Type[] genericArguments = type.GetGenericArguments();
			return (genericArguments.Length != 1) ? null : genericArguments[0];
		}

		internal static Type FindCommonType(Type itemType, object value)
		{
			if (value == null)
			{
				if (itemType != null && itemType.IsValueType)
				{
					itemType = typeof(object);
				}
			}
			else if (itemType == null)
			{
				itemType = value.GetType();
			}
			else
			{
				Type type = value.GetType();
				if (!itemType.IsAssignableFrom(type))
				{
					itemType = ((!type.IsAssignableFrom(itemType)) ? typeof(object) : type);
				}
			}
			return itemType;
		}

		private static bool IsNullable(Type type)
		{
			return type.IsGenericType && typeof(Nullable<>) == type.GetGenericTypeDefinition();
		}

		internal static bool HasAttribute<T>(MemberInfo info) where T : Attribute
		{
			return info != null && Attribute.IsDefined(info, typeof(T));
		}

		internal static bool HasAttribute(MemberInfo info, Type type)
		{
			return info != null && type != null && Attribute.IsDefined(info, type);
		}

		internal static T GetAttribute<T>(MemberInfo info) where T : Attribute
		{
			if (info == null || !Attribute.IsDefined(info, typeof(T)))
			{
				return (T)null;
			}
			return (T)Attribute.GetCustomAttribute(info, typeof(T));
		}

		internal static Attribute GetAttribute(MemberInfo info, Type type)
		{
			if (info == null || type == null || !Attribute.IsDefined(info, type))
			{
				return null;
			}
			return Attribute.GetCustomAttribute(info, type);
		}
	}
}
