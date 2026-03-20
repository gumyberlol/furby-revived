using JsonFx.Serialization;

namespace JsonFx.Markup
{
	internal class MarkupGrammar
	{
		public const char OperatorElementBegin = '<';

		public const char OperatorElementEnd = '>';

		public const char OperatorElementClose = '/';

		public const char OperatorValueDelim = ' ';

		public const char OperatorPairDelim = '=';

		public const char OperatorPrefixDelim = ':';

		public const char OperatorStringDelim = '"';

		public const char OperatorStringDelimAlt = '\'';

		public const char OperatorEntityBegin = '&';

		public const char OperatorEntityNum = '#';

		public const char OperatorEntityHex = 'x';

		public const char OperatorEntityHexAlt = 'X';

		public const char OperatorEntityEnd = ';';

		public const char OperatorComment = '!';

		public const char OperatorCommentDelim = '-';

		public const string OperatorCommentBegin = "--";

		public const string OperatorCommentEnd = "--";

		public const string OperatorCDataBegin = "[CDATA[";

		public const string OperatorCDataEnd = "]]";

		public const char OperatorProcessingInstruction = '?';

		public const string OperatorPhpExpressionBegin = "?=";

		public const string OperatorProcessingInstructionBegin = "?";

		public const string OperatorProcessingInstructionEnd = "?";

		public const char OperatorCode = '%';

		public const char OperatorCodeDirective = '@';

		public const char OperatorCodeExpression = '=';

		public const char OperatorCodeDeclaration = '!';

		public const char OperatorCodeEncoded = ':';

		public const char OperatorCodeDataBind = '#';

		public const char OperatorCodeExtension = '$';

		public const string OperatorCodeBlockBegin = "%";

		public const string OperatorCodeEnd = "%";

		public const char OperatorT4 = '#';

		public const char OperatorT4Directive = '@';

		public const char OperatorT4Expression = '=';

		public const char OperatorT4ClassFeature = '+';

		public const string OperatorT4BlockBegin = "#";

		public const string OperatorT4End = "#";

		public static readonly Token<MarkupTokenType> TokenNone = new Token<MarkupTokenType>(MarkupTokenType.None);

		public static readonly Token<MarkupTokenType> TokenElementEnd = new Token<MarkupTokenType>(MarkupTokenType.ElementEnd);

		public static Token<MarkupTokenType> TokenUnparsed(string begin, string end, string value)
		{
			return new Token<MarkupTokenType>(MarkupTokenType.Primitive, new UnparsedBlock(begin, end, value));
		}

		public static Token<MarkupTokenType> TokenElementBegin(DataName name)
		{
			return new Token<MarkupTokenType>(MarkupTokenType.ElementBegin, name);
		}

		public static Token<MarkupTokenType> TokenElementVoid(DataName name)
		{
			return new Token<MarkupTokenType>(MarkupTokenType.ElementVoid, name);
		}

		public static Token<MarkupTokenType> TokenAttribute(DataName name)
		{
			return new Token<MarkupTokenType>(MarkupTokenType.Attribute, name);
		}

		public static Token<MarkupTokenType> TokenPrimitive(object value)
		{
			return new Token<MarkupTokenType>(MarkupTokenType.Primitive, value);
		}
	}
}
