using Relentless;

namespace Furby.Scripts.FurMail
{
	public class FurMailPersistedStore : PersistedDataStore<FurMailData>
	{
		protected override int Version
		{
			get
			{
				return 3;
			}
		}

		public FurMailPersistedStore(int slotIndex)
		{
			base.Name = "FurMailData" + slotIndex;
			AddVersionUpdater(2, (string s) => s);
			AddVersionUpdater(3, (string s) => Encrypt(s));
		}
	}
}
