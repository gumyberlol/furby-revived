using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JsonFx.Serialization
{
	public interface IDataWriterRS
	{
		Encoding ContentEncoding { get; }

		IEnumerable<string> ContentType { get; }

		IEnumerable<string> FileExtension { get; }

		DataWriterSettingsRS Settings { get; }

		void Write(object data, TextWriter output);

		string Write(object data);
	}
}
