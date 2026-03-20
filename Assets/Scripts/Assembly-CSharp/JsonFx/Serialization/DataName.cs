using System;
using System.Collections;
using System.Globalization;

namespace JsonFx.Serialization
{
	public struct DataName : IComparable<DataName>
	{
		public static readonly DataName Empty = default(DataName);

		public readonly string LocalName;

		public readonly string Prefix;

		public readonly string NamespaceUri;

		public readonly bool IsAttribute;

		public bool IsEmpty
		{
			get
			{
				return LocalName == null;
			}
		}

		public DataName(Type type)
			: this(GetTypeName(type))
		{
		}

		public DataName(string localName)
			: this(localName, null, null, false)
		{
		}

		public DataName(string localName, string prefix, string namespaceUri)
			: this(localName, prefix, namespaceUri, false)
		{
		}

		public DataName(string localName, string prefix, string namespaceUri, bool isAttribute)
		{
			if (localName == null)
			{
				throw new ArgumentNullException("localName");
			}
			if (prefix == null)
			{
				prefix = string.Empty;
			}
			if (string.IsNullOrEmpty(namespaceUri))
			{
				namespaceUri = string.Empty;
			}
			else if (!Uri.IsWellFormedUriString(namespaceUri, UriKind.Absolute))
			{
				throw new ArgumentException("Namespace must be an absolute URI or empty", "namespaceUri");
			}
			LocalName = localName;
			Prefix = prefix;
			NamespaceUri = namespaceUri;
			IsAttribute = isAttribute;
		}

		private static string GetTypeName(Type type)
		{
			if (type == typeof(object))
			{
				type = null;
			}
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Empty:
			case TypeCode.DBNull:
				return "object";
			case TypeCode.Boolean:
				return "boolean";
			case TypeCode.Byte:
				return "unsignedByte";
			case TypeCode.Char:
				return "Char";
			case TypeCode.Decimal:
				return "decimal";
			case TypeCode.Double:
				return "double";
			case TypeCode.Int16:
				return "short";
			case TypeCode.Int32:
				return "int";
			case TypeCode.Int64:
				return "long";
			case TypeCode.SByte:
				return "byte";
			case TypeCode.Single:
				return "float";
			case TypeCode.UInt16:
				return "unsignedShort";
			case TypeCode.UInt32:
				return "unsignedInt";
			case TypeCode.UInt64:
				return "unsignedLong";
			case TypeCode.String:
				return "string";
			case TypeCode.DateTime:
				return "dateTime";
			default:
			{
				if (typeof(IDictionary).IsAssignableFrom(type) || type.GetInterface(TypeCoercionUtility.TypeGenericIDictionary, false) != null)
				{
					return "object";
				}
				if (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type))
				{
					return "array";
				}
				bool isGenericType = type.IsGenericType;
				if (isGenericType && type.Name.StartsWith("<>f__AnonymousType", false, CultureInfo.InvariantCulture))
				{
					return "object";
				}
				string text = type.Name;
				int num = ((!isGenericType) ? (-1) : text.IndexOf('`'));
				if (num >= 0)
				{
					text = text.Substring(0, num);
				}
				return text;
			}
			}
		}

		internal static string GetStandardPrefix(string namespaceUri)
		{
			switch (namespaceUri)
			{
			case "http://www.w3.org/XML/1998/namespace":
				return "xml";
			case "http://www.w3.org/2001/XMLSchema":
				return "xs";
			case "http://www.w3.org/2001/XMLSchema-instance":
				return "xsi";
			case "http://www.w3.org/2000/xmlns/":
				return "xmlns";
			case "http://www.w3.org/1999/xhtml":
				return "html";
			case "http://www.w3.org/2005/Atom":
				return "atom";
			case "http://purl.org/dc/elements/1.1/":
				return "dc";
			case "http://purl.org/syndication/thread/1.0":
				return "thr";
			default:
				return null;
			}
		}

		public string ToPrefixedName()
		{
			if (string.IsNullOrEmpty(Prefix))
			{
				return LocalName;
			}
			string text = Prefix;
			if (string.IsNullOrEmpty(text))
			{
				text = GetStandardPrefix(NamespaceUri) ?? "pfx";
			}
			return text + ":" + LocalName;
		}

		public string ToQualifiedName()
		{
			if (string.IsNullOrEmpty(NamespaceUri))
			{
				return LocalName;
			}
			return "{" + NamespaceUri + "}" + LocalName;
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(Prefix))
			{
				return ToQualifiedName();
			}
			return ToPrefixedName();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is DataName))
			{
				return false;
			}
			return Equals((DataName)obj);
		}

		public bool Equals(DataName that)
		{
			return StringComparer.Ordinal.Equals(NamespaceUri, that.NamespaceUri) && StringComparer.Ordinal.Equals(Prefix, that.Prefix) && StringComparer.Ordinal.Equals(LocalName, that.LocalName);
		}

		public override int GetHashCode()
		{
			int num = 908675878;
			num = -1521134295 * num + StringComparer.Ordinal.GetHashCode(LocalName ?? string.Empty);
			num = -1521134295 * num + StringComparer.Ordinal.GetHashCode(Prefix ?? string.Empty);
			return -1521134295 * num + StringComparer.Ordinal.GetHashCode(NamespaceUri ?? string.Empty);
		}

		public int CompareTo(DataName that)
		{
			int num = StringComparer.Ordinal.Compare(NamespaceUri, that.NamespaceUri);
			if (num != 0)
			{
				return num;
			}
			return StringComparer.Ordinal.Compare(LocalName, that.LocalName);
		}

		public static bool operator ==(DataName a, DataName b)
		{
			if (object.ReferenceEquals(a, null))
			{
				return object.ReferenceEquals(b, null);
			}
			return a.Equals(b);
		}

		public static bool operator !=(DataName a, DataName b)
		{
			return !(a == b);
		}
	}
}
