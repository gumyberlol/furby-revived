using System.Collections;
using Furby.Scripts.FurMail;
using Relentless;
using UnityEngine;

public class FurMailDashboardLogic : MonoBehaviour
{
	[SerializeField]
	private UISlicedSprite m_Background;

	private void Start()
	{
		SingletonInstance<FurMailManager>.Instance.OnDashboardStart();
		StartCoroutine(CheckMessageStatus());
	}

	private IEnumerator CheckMessageStatus()
	{
		while (true)
		{
			if (SingletonInstance<FurMailManager>.Instance.MessageCount == 0)
			{
				m_Background.color = Color.grey;
				base.collider.enabled = false;
			}
			else
			{
				m_Background.color = Color.white;
				base.collider.enabled = true;
			}
			yield return new WaitForSeconds(2f);
		}
	}
}
