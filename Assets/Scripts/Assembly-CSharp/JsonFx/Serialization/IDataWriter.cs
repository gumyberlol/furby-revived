using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JsonFx.Serialization
{
	public interface IDataWriter
	{
		Encoding ContentEncoding { get; }

		IEnumerable<string> ContentType { get; }

		IEnumerable<string> FileExtension { get; }

		DataWriterSettings Settings { get; }

		void Write(object data, TextWriter output);

		string Write(object data);
	}
}
