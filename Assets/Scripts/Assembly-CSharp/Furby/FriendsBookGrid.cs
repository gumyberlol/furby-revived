using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class FriendsBookGrid : RelentlessMonoBehaviour
	{
		[Serializable]
		public class Entry
		{
			public Color TrimColour = Color.yellow;

			public Color GradientTop = Color.magenta;

			public Color GradientBottom = Color.blue;

			public float Angle;

			public Vector3 Offset = Vector3.zero;
		}

		[SerializeField]
		private GameObject m_furbyPrefab;

		[SerializeField]
		private Entry[] m_furbySettings;

		[SerializeField]
		private string m_messagePattern = "VIRTUALFRIENDS_MESSAGE_FRIEND{0}";

		[SerializeField]
		private string m_lockMessage = "VIRTUALFRIENDS_FRIENDLOCKED";

		private void Start()
		{
			IEnumerator<int> enumerator = FurbyGlobals.AdultLibrary.UnlockLevels.GetEnumerator();
			enumerator.MoveNext();
			int num = 0;
			IEnumerable<AdultFurbyType> unlocksInOrder = FurbyGlobals.Player.Furby.UnlocksInOrder;
			if (FurbyGlobals.Player.NoFurbyOnSaveGame())
			{
				unlocksInOrder = FurbyGlobals.Player.NoFurbyUnlock.UnlocksInOrder;
			}
			foreach (AdultFurbyType item in unlocksInOrder)
			{
				FurbyData furbyData = null;
				foreach (FurbyData furby in FurbyGlobals.AdultLibrary.Furbies)
				{
					if (item.Equals(furby.AdultType))
					{
						furbyData = furby;
					}
				}
				if (furbyData != null && furbyData.GetNextBabyTypeFromVirtualFurby() != null)
				{
					GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(m_furbyPrefab, base.transform.position, base.transform.rotation);
					Vector3 localScale = gameObject.transform.localScale;
					gameObject.transform.parent = base.transform;
					gameObject.transform.localScale = localScale;
					gameObject.SetLayerInChildren(base.gameObject.layer);
					SpriteFurbyDisplay component = gameObject.GetComponent<SpriteFurbyDisplay>();
					component.Furby = FurbyGlobals.AdultLibrary.GetAdultFurby(item);
					component.Unlocked = FurbyGlobals.Player.IsFurbyUnlocked(item);
					Entry entry = ((m_furbySettings.Length <= 0) ? new Entry() : m_furbySettings[num % m_furbySettings.Length]);
					component.EdgeTint = entry.TrimColour;
					component.GradientTop = entry.GradientTop;
					component.GradientBottom = entry.GradientBottom;
					component.Angle = entry.Angle;
					component.Offset = entry.Offset;
					component.Message = Singleton<Localisation>.Instance.GetText(string.Format(m_messagePattern, num + 1));
					component.LockMessage = string.Format(Singleton<Localisation>.Instance.GetText(m_lockMessage), enumerator.Current);
					component.Index = num++;
				}
				enumerator.MoveNext();
			}
			UIGrid component2 = GetComponent<UIGrid>();
			component2.Reposition();
		}
	}
}
