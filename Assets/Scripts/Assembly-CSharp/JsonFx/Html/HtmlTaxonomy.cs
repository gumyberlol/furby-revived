using System;

namespace JsonFx.Html
{
	[Flags]
	public enum HtmlTaxonomy
	{
		Text = 0,
		Inline = 1,
		Style = 2,
		List = 4,
		Block = 8,
		Media = 0x10,
		Table = 0x20,
		Form = 0x40,
		Script = 0x80,
		Document = 0x100,
		Plugin = 0x200,
		Unknown = 0x8000
	}
}
