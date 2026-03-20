namespace Relentless
{
	public interface SaveObject
	{
		void SerializeTo(SaveGameWriter writer);

		void DeserializeFrom(SaveGameReader reader);

		int GetVersion();
	}
}
