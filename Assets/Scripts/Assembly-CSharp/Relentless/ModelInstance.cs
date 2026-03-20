using UnityEngine;

namespace Relentless
{
	public class ModelInstance : MonoBehaviour
	{
		public GameObject ModelPrefab;

		public int Layer;

		public string StartAnimation;

		public WrapMode WrapMode;

		public bool m_destoryOld;

		private GameObject m_instance;

		public GameObject Instance
		{
			get
			{
				return m_instance;
			}
		}

		public virtual void InstantiateObject()
		{
			if (m_destoryOld)
			{
				Object.Destroy(m_instance);
				m_instance = null;
			}
			if (m_instance != null)
			{
				return;
			}
			m_instance = (GameObject)Object.Instantiate(ModelPrefab, base.transform.position, base.transform.rotation);
			m_instance.transform.parent = base.transform;
			m_instance.transform.localScale = Vector3.one;
			base.gameObject.SetLayerInChildren(Layer);
			if (!(m_instance.animation != null))
			{
				return;
			}
			if (!string.IsNullOrEmpty(StartAnimation))
			{
				m_instance.animation.Play(StartAnimation);
				if (WrapMode != WrapMode.Default)
				{
					m_instance.animation[StartAnimation].wrapMode = WrapMode;
				}
			}
			else
			{
				m_instance.animation.Stop();
			}
		}

		protected void Start()
		{
			InstantiateObject();
		}

		protected void OnDrawGizmosSelected()
		{
			OnDrawGizmos();
		}

		protected void OnDrawGizmos()
		{
			if (!Application.isPlaying && base.enabled && ModelPrefab != null)
			{
				GhostPrefab.RenderGhostPrefab(ModelPrefab, base.transform.localToWorldMatrix);
			}
		}
	}
}
