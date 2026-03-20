using System.IO;

namespace JsonFx.Serialization
{
	public interface IBinaryFormattable<T>
	{
		int Format(IBinaryFormatter<T> formatter, BinaryWriter writer);
	}
}
