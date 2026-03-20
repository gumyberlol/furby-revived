using System;
using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class HintEventReaction : GameEventReaction
	{
		public HintExpiryCausation[] m_ExpirationList;

		public GameObject m_Prefab;

		public GameObject m_PositioningObject;

		public bool m_MatchRotation;

		private GameObject m_Instance;

		private HintEventTranslator m_translator;

		public override void React(GameObject gameObject, params object[] paramlist)
		{
			m_translator = gameObject.GetComponent<HintEventTranslator>();
			if (m_translator != null)
			{
				Enum value = GameEvent.Value;
				HintEvents hintEvent = (HintEvents)(object)value;
				m_translator.StartCoroutine(InvokeHint(hintEvent, m_ExpirationList));
			}
		}

		private IEnumerator InvokeHint(HintEvents hintEvent, HintExpiryCausation[] expirationList)
		{
			if (!(m_Instance != null) && SingletonInstance<ModalityMediator>.Instance.IsAvailable() && m_Prefab != null)
			{
				InstanceHintPrefab(hintEvent);
				HintHandler handler = m_Instance.GetComponent<HintHandler>();
				handler.RegisterExpiry(expirationList);
			}
			yield break;
		}

		private void InstanceHintPrefab(HintEvents hintEvent)
		{
			if (!(m_Prefab != null))
			{
				return;
			}
			m_Instance = (GameObject)UnityEngine.Object.Instantiate(m_Prefab);
			m_Instance.name = m_Prefab.name + "@" + hintEvent;
			GameEventRouter.SendEvent(HintEvents.Hints_PrefabInstantiated);
			if (m_PositioningObject != null)
			{
				m_Instance.layer = m_PositioningObject.layer;
				m_Instance.transform.localPosition = m_PositioningObject.transform.position;
				m_Instance.transform.localScale = m_PositioningObject.transform.localScale;
				if (m_MatchRotation)
				{
					m_Instance.transform.rotation = m_PositioningObject.transform.rotation;
				}
			}
		}

		public void TerminateHint()
		{
			m_translator.StopCoroutine("InvokeHint");
			if (m_Instance != null)
			{
				UnityEngine.Object.DestroyObject(m_Instance);
				m_Instance = null;
			}
		}
	}
}
