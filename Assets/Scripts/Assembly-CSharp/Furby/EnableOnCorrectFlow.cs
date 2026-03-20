using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class EnableOnCorrectFlow : MonoBehaviour
	{
		[SerializeField]
		private FlowStage m_validForStage;

		[SerializeField]
		private SerialisableEnum m_enableEvent;

		[SerializeField]
		private GameObject m_pauseUntilInactive;

		[SerializeField]
		private bool m_invertEffect;

		private void Start()
		{
			if (m_pauseUntilInactive != null)
			{
				StartCoroutine(WaitUntilInactive());
			}
			else
			{
				Initialise();
			}
		}

		private IEnumerator WaitUntilInactive()
		{
			do
			{
				yield return null;
			}
			while (m_pauseUntilInactive.activeInHierarchy);
			Initialise();
		}

		private void Initialise()
		{
			bool flag = FurbyGlobals.Player.FlowStage == m_validForStage;
			if (m_invertEffect)
			{
				flag = !flag;
			}
			base.gameObject.SetActive(flag);
			if (flag && m_enableEvent.IsTypeSet())
			{
				GameEventRouter.SendEvent(m_enableEvent.Value);
			}
		}
	}
}
