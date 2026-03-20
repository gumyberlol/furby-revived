using System;
using System.Collections;
using System.Collections.Generic;

namespace JsonFx.Serialization
{
	public interface ITokenAnalyzerRS<T>
	{
		DataReaderSettingsRS Settings { get; }

		IEnumerable Analyze(IEnumerable<Token<T>> tokens);

		IEnumerable Analyze(IEnumerable<Token<T>> tokens, Type targetType);

		IEnumerable<TResult> Analyze<TResult>(IEnumerable<Token<T>> tokens);

		IEnumerable<TResult> Analyze<TResult>(IEnumerable<Token<T>> tokens, TResult ignored);
	}
}
