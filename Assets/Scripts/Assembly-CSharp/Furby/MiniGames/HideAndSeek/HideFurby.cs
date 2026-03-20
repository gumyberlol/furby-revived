using Relentless;
using UnityEngine;

namespace Furby.MiniGames.HideAndSeek
{
	public class HideFurby : MonoBehaviour
	{
		public Camera m_MainCamera;

		public Camera m_HideObjectsCamera;

		public GameObject m_HideObjectsParent;

		private GameObject m_CurrentHideObject;

		public GameObject CurrentHideObject
		{
			get
			{
				return m_CurrentHideObject;
			}
		}

		public void ReHideBaby()
		{
			GameObject hideObjectsParent = m_HideObjectsParent;
			ObjectHitEvent[] componentsInChildren = hideObjectsParent.GetComponentsInChildren<ObjectHitEvent>();
			int num;
			do
			{
				num = Random.Range(0, componentsInChildren.Length - 1);
			}
			while (componentsInChildren[num].gameObject.tag == "special");
			m_CurrentHideObject = componentsInChildren[num].gameObject;
			base.transform.position = m_CurrentHideObject.transform.position;
			base.collider.enabled = true;
			float num2 = m_CurrentHideObject.collider.bounds.extents.magnitude / base.collider.bounds.extents.magnitude;
			base.transform.localScale *= num2;
			base.transform.position = m_CurrentHideObject.collider.bounds.center + (base.transform.position - base.collider.bounds.center);
			base.collider.enabled = false;
			Vector3 vector = m_MainCamera.transform.position - base.transform.position;
			Logging.Log(vector);
			base.transform.rotation = Quaternion.LookRotation(vector);
			HexGridPosition hexGridPosition = GetComponent<HexGridPosition>();
			if (hexGridPosition == null)
			{
				hexGridPosition = base.gameObject.AddComponent<HexGridPosition>();
			}
			hexGridPosition.HexPosition = m_CurrentHideObject.GetComponent<HexGridPosition>().HexPosition;
			Logging.Log(string.Format("Baby pos: {0:0.00},{1:0.00}", hexGridPosition.HexPosition.x, hexGridPosition.HexPosition.y));
		}

		private void Update()
		{
		}
	}
}
