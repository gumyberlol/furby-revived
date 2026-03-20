namespace Relentless
{
	public class GlobalGameDataStore : PersistedDataStore<GlobalGameData>
	{
		protected override int Version
		{
			get
			{
				return 18;
			}
		}

		public GlobalGameDataStore()
		{
			base.Name = "GlobalFurbyGameData";
			AddVersionUpdater(17, (string s) => s);
			AddVersionUpdater(18, (string s) => Encrypt(s));
		}
	}
}
