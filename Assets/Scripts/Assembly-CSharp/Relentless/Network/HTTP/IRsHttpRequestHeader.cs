using System;

namespace Relentless.Network.HTTP
{
	public interface IRsHttpRequestHeader
	{
		string Verb { get; }

		Uri Uri { get; }

		void AddHeader(string name, string value);

		string GetHeader(string name);
	}
}
