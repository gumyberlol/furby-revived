namespace JsonFx.Serialization.GraphCycles
{
	public interface ICycleDetector
	{
		bool Add(object item);

		void Remove(object item);
	}
}
