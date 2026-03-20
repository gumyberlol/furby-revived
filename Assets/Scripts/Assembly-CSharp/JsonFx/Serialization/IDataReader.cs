using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace JsonFx.Serialization
{
	public interface IDataReader
	{
		IEnumerable<string> ContentType { get; }

		DataReaderSettings Settings { get; }

		TResult Read<TResult>(TextReader input, TResult ignored);

		TResult Read<TResult>(TextReader input);

		object Read(TextReader input);

		object Read(TextReader input, Type targetType);

		TResult Read<TResult>(string input, TResult ignored);

		TResult Read<TResult>(string input);

		object Read(string input);

		object Read(string input, Type targetType);

		IEnumerable ReadMany(TextReader input);
	}
}
