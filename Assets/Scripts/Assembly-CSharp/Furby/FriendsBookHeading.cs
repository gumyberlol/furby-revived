using Relentless;
using UnityEngine;

namespace Furby
{
	public class FriendsBookHeading : MonoBehaviour
	{
		[SerializeField]
		private UILabel m_label;

		private void Start()
		{
			string slotName = GetSlotName();
			string text = Singleton<Localisation>.Instance.GetText("VIRTUALFRIENDS_HEADER");
			string text2 = string.Format(text, slotName);
			m_label.text = text2;
		}

		private string GetSlotName()
		{
			GameData data = Singleton<GameDataStoreObject>.Instance.Data;
			if (!FurbyGlobals.Player.NoFurbyOnSaveGame())
			{
				string format = "{0}-{1}";
				return string.Format(format, data.FurbyNameLeft, data.FurbyNameRight);
			}
			string text = Singleton<Localisation>.Instance.GetText("DASHBOARD_NOFURBY_NAME");
			int num = Singleton<GameDataStoreObject>.Instance.GetCurrentSlotIndex() + 1;
			return string.Format(text, num);
		}
	}
}
