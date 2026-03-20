using System;
using System.Collections;
using System.Collections.Generic;
using Fabric;
using Relentless;
using UnityEngine;

namespace Furby.Neighbourhood
{
	public class HoodTransitionAudioTimer : RelentlessMonoBehaviour
	{
		[Serializable]
		public class HoodFullnessSwitch
		{
			[SerializeField]
			public int m_threshold;

			[SerializeField]
			public string m_switch;
		}

		[SerializeField]
		private HoodFullnessSwitch[] m_fullnessLevels;

		[SerializeField]
		private string m_hoodFullnessSwitchName;

		public Tribeset m_Tribeset;

		private void Start()
		{
			List<FurbyBaby> babiesInHoodOfTribeSet = FurbyGlobals.BabyRepositoryHelpers.GetBabiesInHoodOfTribeSet(m_Tribeset);
			int count = babiesInHoodOfTribeSet.Count;
			int num = -1;
			string text = null;
			HoodFullnessSwitch[] fullnessLevels = m_fullnessLevels;
			foreach (HoodFullnessSwitch hoodFullnessSwitch in fullnessLevels)
			{
				if (count >= hoodFullnessSwitch.m_threshold && hoodFullnessSwitch.m_threshold > num)
				{
					num = hoodFullnessSwitch.m_threshold;
					text = hoodFullnessSwitch.m_switch;
				}
			}
			if (text != null)
			{
				EventManager.Instance.PostEvent(m_hoodFullnessSwitchName, EventAction.SetSwitch, text);
			}
			StartCoroutine(StartTimingSequence());
		}

		private IEnumerator StartTimingSequence()
		{
			yield return StartCoroutine(TimingSequence());
		}

		private IEnumerator TimingSequence()
		{
			float gatingCounter = 0f;
			float counterMax = 10f;
			float timeWaited = 0f;
			while (timeWaited <= 1f)
			{
				timeWaited += Time.deltaTime / 10f;
				if (gatingCounter > counterMax)
				{
					GameEventRouter.SendEvent(HoodEvents.Hood_TransitionSequencer, null, Mathf.Clamp01(timeWaited));
					gatingCounter = 0f;
				}
				gatingCounter += 1f;
				yield return null;
			}
		}
	}
}
