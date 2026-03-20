using System.IO;

namespace JsonFx.Serialization
{
	public interface ITextFormattable<T>
	{
		void Format(ITextFormatter<T> formatter, TextWriter writer);
	}
}
