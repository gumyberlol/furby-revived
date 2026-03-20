namespace JsonFx.Html
{
	public class HtmlFilter
	{
		private static bool VoidTagRequired(string tag)
		{
			switch (tag)
			{
			case "area":
			case "base":
			case "basefont":
			case "br":
			case "col":
			case "frame":
			case "hr":
			case "img":
			case "input":
			case "isindex":
			case "keygen":
			case "link":
			case "meta":
			case "param":
			case "source":
			case "wbr":
				return true;
			default:
				return false;
			}
		}

		private static bool CloseTagOptional(string tag)
		{
			switch (tag)
			{
			case "body":
			case "colgroup":
			case "dd":
			case "dt":
			case "embed":
			case "head":
			case "html":
			case "li":
			case "option":
			case "p":
			case "tbody":
			case "td":
			case "tfoot":
			case "th":
			case "thead":
			case "tr":
				return true;
			default:
				return false;
			}
		}

		private static HtmlTaxonomy GetTaxonomy(string tag)
		{
			switch (tag)
			{
			case "a":
			case "abbr":
			case "acronym":
			case "area":
			case "bdo":
			case "cite":
			case "code":
			case "dfn":
			case "em":
			case "isindex":
			case "kbd":
			case "map":
			case "q":
			case "samp":
			case "span":
			case "strong":
			case "time":
			case "var":
			case "wbr":
				return HtmlTaxonomy.Inline;
			case "audio":
			case "bgsound":
			case "img":
			case "sound":
			case "source":
				return HtmlTaxonomy.Inline | HtmlTaxonomy.Media;
			case "canvas":
			case "math":
			case "svg":
			case "video":
				return HtmlTaxonomy.Block | HtmlTaxonomy.Media;
			case "b":
			case "big":
			case "blink":
			case "figcaption":
			case "font":
			case "i":
			case "marquee":
			case "mark":
			case "rp":
			case "rt":
			case "ruby":
			case "s":
			case "small":
			case "strike":
			case "sub":
			case "sup":
			case "tt":
			case "u":
				return HtmlTaxonomy.Inline | HtmlTaxonomy.Style;
			case "address":
			case "article":
			case "asside":
			case "blockquote":
			case "bq":
			case "br":
			case "center":
			case "del":
			case "details":
			case "div":
			case "figure":
			case "footer":
			case "h1":
			case "h2":
			case "h3":
			case "h4":
			case "h5":
			case "h6":
			case "header":
			case "hgroup":
			case "hr":
			case "ins":
			case "nav":
			case "nobr":
			case "p":
			case "pre":
			case "section":
			case "summary":
				return HtmlTaxonomy.Block;
			case "command":
			case "dl":
			case "dd":
			case "dir":
			case "dt":
			case "lh":
			case "li":
			case "menu":
			case "ol":
			case "ul":
				return HtmlTaxonomy.List;
			case "caption":
			case "col":
			case "colgroup":
			case "table":
			case "tbody":
			case "td":
			case "th":
			case "thead":
			case "tfoot":
			case "tr":
				return HtmlTaxonomy.Table;
			case "button":
			case "datalist":
			case "fieldset":
			case "form":
			case "keygen":
			case "input":
			case "label":
			case "legend":
			case "meter":
			case "optgroup":
			case "option":
			case "output":
			case "progress":
			case "select":
			case "textarea":
				return HtmlTaxonomy.Form;
			case "applet":
			case "embed":
			case "noembed":
			case "object":
			case "param":
				return HtmlTaxonomy.Plugin;
			case "basefont":
			case "link":
			case "style":
				return HtmlTaxonomy.Style | HtmlTaxonomy.Document;
			case "noscript":
			case "script":
				return HtmlTaxonomy.Script | HtmlTaxonomy.Document;
			case "base":
			case "body":
			case "frame":
			case "frameset":
			case "head":
			case "html":
			case "iframe":
			case "meta":
			case "noframes":
			case "title":
				return HtmlTaxonomy.Document;
			default:
				return HtmlTaxonomy.Unknown;
			}
		}
	}
}
