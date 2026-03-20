using Relentless;
using UnityEngine;

namespace Furby.SaveSlotSelect
{
	public class SaveGameSelectButton : MonoBehaviour
	{
		[SerializeField]
		private int m_saveGameIndex;

		[SerializeField]
		private UILabel m_nameLabel;

		[SerializeField]
		private string m_nameFormat = "{0}-{1}";

		[SerializeField]
		private NamedTextReference m_noFurbyNamedText;

		private void Awake()
		{
			GameData slot = Singleton<GameDataStoreObject>.Instance.GetSlot(m_saveGameIndex);
			if (slot != null)
			{
				if (!slot.NoFurbyMode)
				{
					m_nameLabel.text = string.Format(m_nameFormat, slot.FurbyNameLeft, slot.FurbyNameRight);
				}
				else
				{
					m_nameLabel.text = string.Format(m_noFurbyNamedText.NamedText, m_saveGameIndex + 1);
				}
			}
		}
	}
}
