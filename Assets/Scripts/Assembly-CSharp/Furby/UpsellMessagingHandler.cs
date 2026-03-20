using Relentless;
using UnityEngine;

namespace Furby
{
	public class UpsellMessagingHandler : MonoBehaviour
	{
		[SerializeField]
		public UpsellMessageDefinition[] m_UpsellMessages;

		[SerializeField]
		public GameObject m_ParentRoot;

		private GameObject m_InstancedPrefab;

		public UILabel m_DialogHeaderLabel;

		private int m_IndexOfLastUpsellMessage = -1;

		public void OnEnable()
		{
			ActivateContent();
		}

		public void OnDisable()
		{
			DeactivateContent();
		}

		public void OnDestroy()
		{
			DeactivateContent();
		}

		private UpsellMessageDefinition GetRandomUpsellDefinition()
		{
			int num;
			for (num = m_IndexOfLastUpsellMessage; num == m_IndexOfLastUpsellMessage; num = Random.Range(0, m_UpsellMessages.Length))
			{
			}
			m_IndexOfLastUpsellMessage = num;
			return m_UpsellMessages[num];
		}

		private void ActivateContent()
		{
			UpsellMessageDefinition randomUpsellDefinition = GetRandomUpsellDefinition();
			GameObject prefabToInstance = randomUpsellDefinition.m_PrefabToInstance;
			m_InstancedPrefab = (GameObject)Object.Instantiate(prefabToInstance);
			m_InstancedPrefab.transform.parent = m_ParentRoot.transform;
			m_InstancedPrefab.transform.gameObject.SetLayerInChildren(m_ParentRoot.transform.gameObject.layer);
			m_InstancedPrefab.transform.position = m_ParentRoot.transform.position;
			m_InstancedPrefab.transform.localScale = Vector3.one;
			m_InstancedPrefab.SetActive(true);
			m_DialogHeaderLabel.text = Singleton<Localisation>.Instance.GetText(randomUpsellDefinition.m_DialogHeaderNamedTextKey);
		}

		private void DeactivateContent()
		{
			if (m_InstancedPrefab != null)
			{
				Object.Destroy(m_InstancedPrefab);
				m_InstancedPrefab = null;
			}
		}
	}
}
