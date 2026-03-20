using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class PhysicalInteractionMessaging : RelentlessMonoBehaviour
	{
		public float m_IntervalSecsBetweenDisplays = 450f;

		private float m_LastTimestamp;

		[SerializeField]
		public List<Locale> m_LocalesToShowPhysicalInteraction = new List<Locale>();

		public void Start()
		{
			ResetTimestamp();
		}

		public bool IsARegionWeWantToShowInteractionMessagesFor()
		{
			return m_LocalesToShowPhysicalInteraction.Contains(Singleton<Localisation>.Instance.CurrentLocale);
		}

		public bool ShouldShowPhysicalInteractionMessage()
		{
			if (Singleton<GameDataStoreObject>.Instance.HasAGameLoaded() && !FurbyGlobals.Player.NoFurbyOnSaveGame() && IsARegionWeWantToShowInteractionMessagesFor())
			{
				float num = Time.time - m_LastTimestamp;
				return num >= m_IntervalSecsBetweenDisplays;
			}
			return false;
		}

		public void ResetTimestamp()
		{
			m_LastTimestamp = Time.time;
		}
	}
}
