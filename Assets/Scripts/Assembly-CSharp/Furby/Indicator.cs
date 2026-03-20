using Relentless;
using UnityEngine;

namespace Furby
{
	public class Indicator : RelentlessMonoBehaviour
	{
		public GameObject m_indicatorPrefab;

		public float m_ignorePixels;

		public float m_offset = 64f;

		public GameObject m_onScreenPrefab;

		private Vector3 m_lastScreenPosition;

		private GameObject m_indicatorInstance;

		private GameObject m_onScreenInstance;

		private Color m_color = Color.white;

		private void Start()
		{
			if ((bool)m_indicatorPrefab)
			{
				m_indicatorInstance = SingletonInstance<PrefabPool>.Instance.InstantiatePrefab(m_indicatorPrefab, Vector3.zero, Quaternion.identity);
				m_indicatorInstance.transform.parent = Singleton<InGameGUIInterface>.Instance.m_enemyIndicatorRoot;
				SetColor(m_color);
			}
			if ((bool)m_onScreenPrefab)
			{
				m_onScreenInstance = SingletonInstance<PrefabPool>.Instance.InstantiatePrefab(m_onScreenPrefab, Vector3.zero, Quaternion.identity);
				m_onScreenInstance.transform.parent = Singleton<InGameGUIInterface>.Instance.m_enemyIndicatorRoot;
			}
		}

		private void OnEnable()
		{
			if ((bool)m_indicatorInstance)
			{
				m_indicatorInstance.SetActive(true);
				SetColor(m_color);
			}
			if ((bool)m_onScreenInstance)
			{
				m_onScreenInstance.SetActive(true);
			}
		}

		private void OnDisable()
		{
			if ((bool)m_indicatorInstance)
			{
				m_indicatorInstance.SetActive(false);
			}
			if ((bool)m_onScreenInstance)
			{
				m_onScreenInstance.SetActive(true);
			}
		}

		public void SetColor(Color color)
		{
			m_color = color;
			if ((bool)m_indicatorInstance)
			{
				m_indicatorInstance.GetComponent<UISprite>().color = color;
			}
		}

		public Vector3 GetLastScreenPosition()
		{
			return m_lastScreenPosition;
		}

		public bool IsPrettyMuchOnScreen()
		{
			float num = (float)Screen.width * 0.05f;
			if (m_lastScreenPosition.x > 0f - num && m_lastScreenPosition.x < (float)Screen.width + num && m_lastScreenPosition.y > 0f - num && m_lastScreenPosition.y < (float)Screen.height + num)
			{
				return true;
			}
			return false;
		}

		private void Update()
		{
			bool flag = false;
			if (m_indicatorInstance != null)
			{
				Vector3 vector = (m_lastScreenPosition = Camera.main.WorldToScreenPoint(base.transform.position));
				vector.z = 0f;
				if (vector.x > m_ignorePixels && vector.x < (float)Screen.width - m_ignorePixels && vector.y > m_ignorePixels && vector.y < (float)Screen.height - m_ignorePixels)
				{
					m_indicatorInstance.SetActive(false);
				}
				else
				{
					flag = true;
					Vector3 vector2 = Camera.main.transform.position + Camera.main.transform.forward;
					Vector3 position = (base.transform.position - vector2).normalized + vector2;
					vector = Camera.main.WorldToScreenPoint(position);
					m_indicatorInstance.SetActive(true);
					Vector3 normalized = (vector - new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f)).normalized;
					Vector3 vector3 = normalized;
					if (Mathf.Abs(normalized.x) > Mathf.Abs(normalized.y))
					{
						vector3 /= Mathf.Abs(vector3.x);
					}
					else
					{
						vector3 /= Mathf.Abs(vector3.y);
					}
					vector3.x *= (float)Screen.width / 2f - m_offset;
					vector3.y *= (float)Screen.height / 2f - m_offset;
					Vector3 localPosition = new Vector3((float)Screen.width / 2f + vector3.x, (float)Screen.height / 2f + vector3.y, 0f);
					UIRoot component = Singleton<InGameGUIInterface>.Instance.gameObject.GetComponent<UIRoot>();
					if (component != null)
					{
						localPosition.x /= Screen.width;
						localPosition.y /= Screen.height;
						localPosition.x *= (float)component.manualHeight / (float)Screen.height * (float)Screen.width;
						localPosition.y *= component.manualHeight;
					}
					m_indicatorInstance.transform.localPosition = localPosition;
					m_indicatorInstance.transform.localRotation = Quaternion.LookRotation(new Vector3(0f, 0f, 1f), normalized);
				}
			}
			if (!(m_onScreenInstance != null))
			{
				return;
			}
			if (!flag)
			{
				m_onScreenInstance.SetActive(true);
				Vector3 localPosition2 = Camera.main.WorldToScreenPoint(base.transform.position);
				UIRoot component2 = Singleton<InGameGUIInterface>.Instance.gameObject.GetComponent<UIRoot>();
				if (component2 != null)
				{
					localPosition2.x /= Screen.width;
					localPosition2.y /= Screen.height;
					localPosition2.x *= (float)component2.manualHeight / (float)Screen.height * (float)Screen.width;
					localPosition2.y *= component2.manualHeight;
				}
				m_onScreenInstance.transform.localPosition = localPosition2;
			}
			else
			{
				m_onScreenInstance.SetActive(false);
			}
		}
	}
}
