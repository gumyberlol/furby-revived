using System.Collections;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby.Incubator
{
	public class IncubatorPersonalityDialog : GameEventConsumer<IncubatorGameEvent>
	{
		[SerializeField]
		private GameObject m_DialogPanel;

		[SerializeField]
		private IncubatorPersonalityButton[] m_ButtonArray;

		public IEnumerator ShowModal()
		{
			SetButtonPersonalities();
			m_DialogPanel.SetActive(true);
			foreach (IncubatorGameEvent? i in Await(true, IncubatorGameEvent.Imprint_Personality))
			{
				yield return i;
			}
			m_DialogPanel.SetActive(false);
		}

		private void SetButtonPersonalities()
		{
			int num = 1;
			FurbyPersonality personality = Singleton<FurbyDataChannel>.Instance.FurbyStatus.Personality;
			foreach (FurbyPersonality item in EnumExtensions.Values<FurbyPersonality>().Distinct())
			{
				if (item != personality)
				{
					m_ButtonArray[num++].SetPersonality(item);
				}
			}
			m_ButtonArray[0].SetPersonality(personality);
		}
	}
}
