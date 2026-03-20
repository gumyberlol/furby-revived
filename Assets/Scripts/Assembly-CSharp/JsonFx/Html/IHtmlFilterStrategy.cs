using JsonFx.Markup;
using JsonFx.Serialization;

namespace JsonFx.Html
{
	public interface IHtmlFilterStrategy
	{
		bool FilterTag(DataName tag, MarkupTokenType type, HtmlTaxonomy taxonomy);

		bool FilterAttribute(DataName tag, DataName attribute, ref object value);

		bool FilterStyle(DataName tag, string style, ref object value);

		bool FilterLiteral(ref object value);
	}
}
