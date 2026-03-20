using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace JsonFx.Serialization
{
	public sealed class Token<T>
	{
		private const string FullDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'FFFFFFFK";

		public readonly T TokenType;

		public readonly DataName Name;

		public readonly object Value;

		public Token(T tokenType)
		{
			TokenType = tokenType;
			Name = DataName.Empty;
		}

		public Token(T tokenType, object value)
		{
			TokenType = tokenType;
			Value = value;
			Name = DataName.Empty;
		}

		public Token(T tokenType, DataName name)
		{
			TokenType = tokenType;
			Name = name;
		}

		public Token(T tokenType, DataName name, object value)
		{
			TokenType = tokenType;
			Name = name;
			Value = value;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{ ");
			stringBuilder.Append(TokenType);
			if (!Name.IsEmpty)
			{
				stringBuilder.Append("=");
				stringBuilder.Append(Name);
			}
			if (Value != null)
			{
				stringBuilder.Append("=");
				stringBuilder.Append(ValueAsString());
			}
			stringBuilder.Append(" }");
			return stringBuilder.ToString();
		}

		public override bool Equals(object obj)
		{
			Token<T> token = obj as Token<T>;
			if (token == null)
			{
				return false;
			}
			return Equals(token);
		}

		public bool Equals(Token<T> that)
		{
			if (that == null)
			{
				return false;
			}
			return EqualityComparer<T>.Default.Equals(TokenType, that.TokenType) && EqualityComparer<DataName>.Default.Equals(Name, that.Name) && EqualityComparer<object>.Default.Equals(Value, that.Value);
		}

		public override int GetHashCode()
		{
			int num = 1139864702;
			num = -1521134295 * num + EqualityComparer<T>.Default.GetHashCode(TokenType);
			num = -1521134295 * num + EqualityComparer<DataName>.Default.GetHashCode(Name);
			return -1521134295 * num + EqualityComparer<object>.Default.GetHashCode(Value);
		}

		public string ValueAsString()
		{
			return ToString(Value);
		}

		public static string ToString(object value)
		{
			Type type = ((value != null) ? value.GetType() : null);
			if (type != null && type.IsEnum)
			{
				return ((Enum)value).ToString("F");
			}
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Boolean:
				return (!true.Equals(value)) ? "false" : "true";
			case TypeCode.Byte:
				return ((byte)value).ToString("g", CultureInfo.InvariantCulture);
			case TypeCode.Char:
				return char.ToString((char)value);
			case TypeCode.DateTime:
				return ((DateTime)value).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'FFFFFFFK");
			case TypeCode.Empty:
			case TypeCode.DBNull:
				return string.Empty;
			case TypeCode.Decimal:
				return ((decimal)value).ToString("g", CultureInfo.InvariantCulture);
			case TypeCode.Double:
				return ((double)value).ToString("g", CultureInfo.InvariantCulture);
			case TypeCode.Int16:
				return ((short)value).ToString("g", CultureInfo.InvariantCulture);
			case TypeCode.Int32:
				return ((int)value).ToString("g", CultureInfo.InvariantCulture);
			case TypeCode.Int64:
				return ((long)value).ToString("g", CultureInfo.InvariantCulture);
			case TypeCode.SByte:
				return ((sbyte)value).ToString("g", CultureInfo.InvariantCulture);
			case TypeCode.Single:
				return ((float)value).ToString("g", CultureInfo.InvariantCulture);
			case TypeCode.String:
				return (string)value;
			case TypeCode.UInt16:
				return ((ushort)value).ToString("g", CultureInfo.InvariantCulture);
			case TypeCode.UInt32:
				return ((uint)value).ToString("g", CultureInfo.InvariantCulture);
			case TypeCode.UInt64:
				return ((ulong)value).ToString("g", CultureInfo.InvariantCulture);
			default:
			{
				IConvertible convertible = value as IConvertible;
				if (convertible != null)
				{
					return convertible.ToString(CultureInfo.InvariantCulture);
				}
				IFormattable formattable = value as IFormattable;
				if (formattable != null)
				{
					return formattable.ToString(null, CultureInfo.InvariantCulture);
				}
				return (value as string) ?? value.ToString();
			}
			}
		}

		public Token<TOther> ChangeType<TOther>(TOther tokenType)
		{
			return new Token<TOther>(tokenType, Name, Value);
		}

		public static bool operator ==(Token<T> a, Token<T> b)
		{
			if (object.ReferenceEquals(a, null))
			{
				return object.ReferenceEquals(b, null);
			}
			return a.Equals(b);
		}

		public static bool operator !=(Token<T> a, Token<T> b)
		{
			return !(a == b);
		}
	}
}
