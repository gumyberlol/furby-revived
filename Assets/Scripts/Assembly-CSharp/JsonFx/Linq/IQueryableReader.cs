using System;
using System.IO;
using System.Linq;
using JsonFx.Serialization;

namespace JsonFx.Linq
{
	public interface IQueryableReader : IDataReader
	{
		IQueryable<TResult> Query<TResult>(TextReader input, TResult ignored);

		IQueryable<TResult> Query<TResult>(TextReader input);

		IQueryable Query(TextReader input);

		IQueryable Query(TextReader input, Type targetType);

		IQueryable<TResult> Query<TResult>(string input, TResult ignored);

		IQueryable<TResult> Query<TResult>(string input);

		IQueryable Query(string input);

		IQueryable Query(string input, Type targetType);
	}
}
