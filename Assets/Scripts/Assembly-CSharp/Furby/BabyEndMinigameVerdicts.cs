using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class BabyEndMinigameVerdicts : MonoBehaviour
	{
		[SerializeField]
		private SerialisableEnum[] m_verdictEvents;

		[SerializeField]
		private GameObject[] m_verdictLabels;

		private void Start()
		{
			if (m_verdictEvents.Length != m_verdictLabels.Length)
			{
				Logging.LogError("BabyEndMinigameVerdict: More events than verdict labels - this will be bad!");
			}
			SerialisableEnum[] verdictEvents = m_verdictEvents;
			foreach (SerialisableEnum serialisableEnum in verdictEvents)
			{
				GameEventRouter.AddDelegateForEnums(OnVerdictEvent, serialisableEnum.Value);
			}
		}

		private void OnDestroy()
		{
			SerialisableEnum[] verdictEvents = m_verdictEvents;
			foreach (SerialisableEnum serialisableEnum in verdictEvents)
			{
				GameEventRouter.RemoveDelegateForEnums(OnVerdictEvent, serialisableEnum.Value);
			}
		}

		private void OnVerdictEvent(Enum eventType, GameObject gameObject, params object[] parameters)
		{
			for (int i = 0; i < m_verdictEvents.Length; i++)
			{
				if (m_verdictEvents[i] != null && m_verdictLabels[i] != null && m_verdictEvents[i].Value != null)
				{
					if (eventType.Equals(m_verdictEvents[i].Value))
					{
						m_verdictLabels[i].SetActive(true);
					}
					else
					{
						m_verdictLabels[i].SetActive(false);
					}
				}
				else
				{
					m_verdictLabels[i].SetActive(false);
				}
			}
		}
	}
}
