using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class FurbyNameLabel : RelentlessMonoBehaviour
	{
		public bool AdultFurby = true;

		[SerializeField]
		private string m_NameFormat = "{0}-{1}";

		[NamedText]
		[SerializeField]
		private string m_postFixNamedText = string.Empty;

		private string m_noFurbyNamedText = "DASHBOARD_NOFURBY_NAME";

		private void OnEnable()
		{
			GameEventRouter.AddDelegateForType(typeof(PlayerFurbyEvent), OnScanEvent);
			Refresh();
		}

		private void OnDisable()
		{
			if (GameEventRouter.Exists)
			{
				GameEventRouter.RemoveDelegateForType(typeof(PlayerFurbyEvent), OnScanEvent);
			}
		}

		private void Refresh()
		{
			UILabel componentInChildren = GetComponentInChildren<UILabel>();
			if (!(componentInChildren != null))
			{
				return;
			}
			if (AdultFurby)
			{
				if (FurbyGlobals.Player.NoFurbyOnSaveGame())
				{
					if (Singleton<GameDataStoreObject>.Instance.Data.HasCompletedFirstTimeFlow)
					{
						componentInChildren.text = string.Format(Singleton<Localisation>.Instance.GetText(m_noFurbyNamedText), Singleton<GameDataStoreObject>.Instance.GetCurrentSlotIndex() + 1) + ((!string.IsNullOrEmpty(m_postFixNamedText)) ? Singleton<Localisation>.Instance.GetText(m_postFixNamedText) : string.Empty);
					}
					else
					{
						componentInChildren.text = string.Empty;
					}
				}
				else
				{
					componentInChildren.text = string.Format(m_NameFormat, Singleton<GameDataStoreObject>.Instance.Data.FurbyNameLeft, Singleton<GameDataStoreObject>.Instance.Data.FurbyNameRight) + ((!string.IsNullOrEmpty(m_postFixNamedText)) ? Singleton<Localisation>.Instance.GetText(m_postFixNamedText) : string.Empty);
				}
			}
			else
			{
				FurbyBaby selectedFurbyBaby = FurbyGlobals.Player.SelectedFurbyBaby;
				if (selectedFurbyBaby != null)
				{
					componentInChildren.text = string.Format(m_NameFormat, selectedFurbyBaby.NameLeft, selectedFurbyBaby.NameRight);
				}
			}
		}

		private void OnScanEvent(Enum sysEvent, GameObject origin, params object[] paramList)
		{
			if ((PlayerFurbyEvent)(object)sysEvent == PlayerFurbyEvent.StatusUpdated)
			{
				Refresh();
			}
		}
	}
}
