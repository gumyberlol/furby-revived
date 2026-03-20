namespace Relentless
{
	public class SimplePlayerPrefsDataModel : PersistedDataStore<SimpleNameValueStore>, IDataModel
	{
		protected override int Version
		{
			get
			{
				return 1;
			}
		}

		public SimplePlayerPrefsDataModel()
		{
			base.Name = "SimplePlayerPrefsData";
		}

		void IDataModel.Load()
		{
			Load();
		}

		void IDataModel.Save()
		{
			Save();
		}
	}
}
