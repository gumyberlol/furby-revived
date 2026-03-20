namespace JsonFx.Serialization.Providers
{
	public interface IDataReaderProvider
	{
		IDataReader Find(string contentTypeHeader);
	}
}
