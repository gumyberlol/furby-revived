using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class DashboardHints : MonoBehaviour
	{
		public delegate bool Tester(DashboardFlow dash);

		[SerializeField]
		private HintState m_eggHint;

		[SerializeField]
		private HintState m_hungryHint;

		[SerializeField]
		private HintState m_happyHint;

		[SerializeField]
		private HintState m_showerHint;

		[SerializeField]
		private HintState m_toiletHint;

		private List<HintState> m_allHints = new List<HintState>();

		private Dictionary<HintState, Tester> m_testers = new Dictionary<HintState, Tester>();

		private HintState m_currentHint;

		[SerializeField]
		private DashboardFlow m_dash;

		public void Start()
		{
			m_testers[m_happyHint] = WrapTest((DashboardFlow dash) => dash.IsUnhappy());
			m_testers[m_toiletHint] = WrapTest((DashboardFlow dash) => dash.NeedsToilet());
			m_testers[m_showerHint] = WrapTest((DashboardFlow dash) => dash.IsDirty());
			m_testers[m_hungryHint] = WrapTest((DashboardFlow dash) => dash.IsHungry());
			m_testers[m_eggHint] = WrapTest((DashboardFlow dash) => dash.HasEgg());
			m_allHints.Add(m_eggHint);
			m_allHints.Add(m_hungryHint);
			m_allHints.Add(m_happyHint);
			m_allHints.Add(m_showerHint);
			m_allHints.Add(m_toiletHint);
		}

		private Tester WrapTest(Tester t)
		{
			bool isFurbyMode = !FurbyGlobals.Player.NoFurbyOnSaveGame();
			return (DashboardFlow dash) => isFurbyMode && t(dash);
		}

		public void Update()
		{
			EnableBestHint();
			foreach (HintState allHint in m_allHints)
			{
				allHint.TestAndBroadcastState();
			}
		}

		private void EnableBestHint()
		{
			HintState hintState = m_allHints.Find((HintState h) => m_testers[h](m_dash));
			if (hintState != m_currentHint)
			{
				if (m_currentHint != null)
				{
					m_currentHint.SoftDisable();
				}
				if (hintState != null)
				{
					hintState.SoftEnable();
				}
				m_currentHint = hintState;
				base.gameObject.SendGameEvent(HintEvents.DeactivateAll);
			}
		}
	}
}
