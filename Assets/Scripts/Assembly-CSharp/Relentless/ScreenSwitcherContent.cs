using System.Collections.Generic;
using UnityEngine;

namespace Relentless
{
	public class ScreenSwitcherContent : RelentlessMonoBehaviour
	{
		public ScreenSwitcherSelectionAlgorithm m_Algorithm;

		public List<ScreenSwitcherContentInstance> m_ContentInstances = new List<ScreenSwitcherContentInstance>();

		public UILabel m_TargetLabel;

		[HideInInspector]
		private int m_LastIndex = -1;

		public void OnDisable()
		{
			DeactivateContent();
		}

		public void OnEnable()
		{
			UpdateUI();
		}

		public int GetContentIndex()
		{
			if (m_ContentInstances.Count > 0)
			{
				switch (m_Algorithm)
				{
				case ScreenSwitcherSelectionAlgorithm.TrueRandom:
					return Random.Range(0, m_ContentInstances.Count);
				case ScreenSwitcherSelectionAlgorithm.JustDifferentToLastTime:
				{
					int num = Random.Range(0, m_ContentInstances.Count);
					if (m_ContentInstances.Count > 1)
					{
						while (m_LastIndex == num)
						{
							num = Random.Range(0, m_ContentInstances.Count);
						}
					}
					return num;
				}
				}
			}
			return -1;
		}

		public void UpdateUI()
		{
			int contentIndex = GetContentIndex();
			ActivateContent(contentIndex);
			m_LastIndex = contentIndex;
		}

		private void ActivateContent(int index)
		{
			ScreenSwitcherContentInstance screenSwitcherContentInstance = m_ContentInstances[index];
			if (screenSwitcherContentInstance != null)
			{
				screenSwitcherContentInstance.ApplyTranslationToComponentLabels(m_TargetLabel);
			}
			screenSwitcherContentInstance.m_HintObject.SetActive(true);
		}

		private void DeactivateContent()
		{
			ScreenSwitcherContentInstance screenSwitcherContentInstance = m_ContentInstances[m_LastIndex];
			if (screenSwitcherContentInstance != null)
			{
				screenSwitcherContentInstance.m_HintObject.SetActive(false);
			}
		}
	}
}
