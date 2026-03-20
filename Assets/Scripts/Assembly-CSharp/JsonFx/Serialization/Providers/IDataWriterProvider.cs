namespace JsonFx.Serialization.Providers
{
	public interface IDataWriterProvider
	{
		IDataWriter DefaultDataWriter { get; }

		IDataWriter Find(string extension);

		IDataWriter Find(string acceptHeader, string contentTypeHeader);
	}
}
