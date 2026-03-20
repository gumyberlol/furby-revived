using UnityEngine;

namespace Relentless
{
	public class LookAtTouch : RelentlessMonoBehaviour
	{
		private Transform m_EyeBoneL;

		private Transform m_EyeBoneR;

		public GameObject m_ModelInstanceObj;

		public string m_EyeBoneR_Name;

		public string m_EyeBoneL_Name;

		public Transform m_LookAtTransform_Root;

		public Transform m_LookAtTransform_EyeL;

		public Transform m_LookAtTransform_EyeR;

		public Camera m_Camera;

		public bool m_StartEnabled = true;

		public EyeMovementLevers m_DesignerLevers;

		private Vector3 m_LookAtDefaultPos = Vector3.zero;

		private Vector3 m_Velocity = Vector3.zero;

		private bool m_AmActive = true;

		private bool m_GotEyeBones;

		private void Awake()
		{
			Singleton<LookAtTouch_Enabler>.Instance.Enable = m_StartEnabled;
			m_LookAtDefaultPos = m_LookAtTransform_Root.position;
			m_DesignerLevers.m_FocusPosition = m_LookAtDefaultPos;
			GetEyeBones();
		}

		private void GetEyeBones()
		{
			if (m_GotEyeBones)
			{
				return;
			}
			ModelInstance component = m_ModelInstanceObj.GetComponent<ModelInstance>();
			if (!component)
			{
				return;
			}
			GameObject instance = component.Instance;
			if (!instance)
			{
				return;
			}
			Transform[] componentsInChildren = instance.GetComponentsInChildren<Transform>(true);
			foreach (Transform transform in componentsInChildren)
			{
				if (m_EyeBoneL_Name.Equals(transform.name))
				{
					m_EyeBoneL = transform;
				}
				if (m_EyeBoneR_Name.Equals(transform.name))
				{
					m_EyeBoneR = transform;
				}
			}
			if (m_EyeBoneL != null && m_EyeBoneR != null)
			{
				m_GotEyeBones = true;
			}
		}

		private void LateUpdate()
		{
			if (m_AmActive)
			{
				if (Singleton<LookAtTouch_Enabler>.Instance.Enable)
				{
					GetEyeBones();
					if (Input.GetMouseButton(0))
					{
						SmoothLookToInputPoint(m_DesignerLevers.m_TimeSecsForEyeToFocus);
					}
					else
					{
						SmoothLookToDefaultPosition(m_DesignerLevers.m_TimeSecsForEyeToDefocus);
					}
				}
			}
			else
			{
				SmoothLookToDefaultPosition(m_DesignerLevers.m_TimeSecsForEyeToDefocus);
			}
		}

		private void SmoothLookToDefaultPosition(float smoothTime)
		{
			SmoothLookAtPosition(ref m_DesignerLevers.m_FocusPosition, smoothTime);
			AlignEyes();
		}

		private void SmoothLookToInputPoint(float smoothTime)
		{
			Vector3 touchPos = m_Camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5f));
			SmoothLookAtPosition(ref touchPos, smoothTime);
			AlignEyes();
		}

		private void SmoothLookAtPosition(ref Vector3 touchPos, float smoothTime)
		{
			m_LookAtTransform_Root.position = Vector3.SmoothDamp(m_LookAtTransform_Root.position, new Vector3(touchPos.x, touchPos.y, m_DesignerLevers.m_FocusPosition.z), ref m_Velocity, smoothTime);
			m_LookAtTransform_Root.localPosition = Vector3.ClampMagnitude(m_LookAtTransform_Root.localPosition, m_DesignerLevers.m_MaxEyeMovementRange);
		}

		private void SnapLookAtDefaultPosition()
		{
			m_LookAtTransform_Root.position = m_DesignerLevers.m_FocusPosition;
			AlignEyes();
		}

		private void AlignEyes()
		{
			if (m_DesignerLevers.m_EyesShouldCross)
			{
				if (m_EyeBoneL != null)
				{
					m_EyeBoneL.LookAt(m_LookAtTransform_EyeL.position);
				}
				if (m_EyeBoneR != null)
				{
					m_EyeBoneR.LookAt(m_LookAtTransform_EyeR.position);
				}
			}
			else
			{
				if (m_EyeBoneL != null)
				{
					m_EyeBoneL.LookAt(m_LookAtTransform_EyeR.position);
				}
				if (m_EyeBoneR != null)
				{
					m_EyeBoneR.LookAt(m_LookAtTransform_EyeL.position);
				}
			}
		}

		public void SetActiveTrue()
		{
			m_AmActive = true;
		}

		public void SetActiveFalse()
		{
			m_AmActive = false;
		}
	}
}
