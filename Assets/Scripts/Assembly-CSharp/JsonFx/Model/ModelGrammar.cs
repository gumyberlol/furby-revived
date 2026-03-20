using JsonFx.Serialization;

namespace JsonFx.Model
{
	public static class ModelGrammar
	{
		public static readonly Token<ModelTokenType> TokenNone = new Token<ModelTokenType>(ModelTokenType.None);

		public static readonly Token<ModelTokenType> TokenArrayEnd = new Token<ModelTokenType>(ModelTokenType.ArrayEnd);

		public static readonly Token<ModelTokenType> TokenObjectEnd = new Token<ModelTokenType>(ModelTokenType.ObjectEnd);

		public static readonly Token<ModelTokenType> TokenNull = new Token<ModelTokenType>(ModelTokenType.Primitive);

		public static readonly Token<ModelTokenType> TokenFalse = new Token<ModelTokenType>(ModelTokenType.Primitive, false);

		public static readonly Token<ModelTokenType> TokenTrue = new Token<ModelTokenType>(ModelTokenType.Primitive, true);

		public static readonly Token<ModelTokenType> TokenArrayBeginUnnamed = new Token<ModelTokenType>(ModelTokenType.ArrayBegin);

		public static readonly Token<ModelTokenType> TokenObjectBeginUnnamed = new Token<ModelTokenType>(ModelTokenType.ObjectBegin);

		internal static readonly Token<ModelTokenType> TokenNaN = new Token<ModelTokenType>(ModelTokenType.Primitive, double.NaN);

		internal static readonly Token<ModelTokenType> TokenPositiveInfinity = new Token<ModelTokenType>(ModelTokenType.Primitive, double.PositiveInfinity);

		internal static readonly Token<ModelTokenType> TokenNegativeInfinity = new Token<ModelTokenType>(ModelTokenType.Primitive, double.NegativeInfinity);

		public static Token<ModelTokenType> TokenArrayBegin(string name)
		{
			return new Token<ModelTokenType>(ModelTokenType.ArrayBegin, new DataName(name));
		}

		public static Token<ModelTokenType> TokenArrayBegin(string name, string prefix, string namespaceUri)
		{
			return new Token<ModelTokenType>(ModelTokenType.ArrayBegin, new DataName(name, prefix, namespaceUri));
		}

		public static Token<ModelTokenType> TokenArrayBegin(DataName name)
		{
			return new Token<ModelTokenType>(ModelTokenType.ArrayBegin, name);
		}

		public static Token<ModelTokenType> TokenObjectBegin(string name)
		{
			return new Token<ModelTokenType>(ModelTokenType.ObjectBegin, new DataName(name));
		}

		public static Token<ModelTokenType> TokenObjectBegin(DataName name)
		{
			return new Token<ModelTokenType>(ModelTokenType.ObjectBegin, name);
		}

		internal static Token<ModelTokenType> TokenProperty(object localName)
		{
			string localName2 = Token<ModelTokenType>.ToString(localName);
			return new Token<ModelTokenType>(ModelTokenType.Property, new DataName(localName2));
		}

		public static Token<ModelTokenType> TokenProperty(string localName)
		{
			return new Token<ModelTokenType>(ModelTokenType.Property, new DataName(localName));
		}

		public static Token<ModelTokenType> TokenProperty(DataName name)
		{
			return new Token<ModelTokenType>(ModelTokenType.Property, name);
		}

		public static Token<ModelTokenType> TokenPrimitive(object value)
		{
			return new Token<ModelTokenType>(ModelTokenType.Primitive, value);
		}
	}
}
