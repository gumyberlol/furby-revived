using System;
using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class TutorialInstructionBox : CommonPanel
	{
		public delegate void Handler();

		[SerializeField]
		private FlowDialog m_dialog;

		[SerializeField]
		private Collider m_targetCollider;

		[SerializeField]
		private bool m_exitOnClick;

		[SerializeField]
		private bool m_exitOnPress;

		[SerializeField]
		private SerialisableEnum m_exitMessage;

		[SerializeField]
		private float m_enablePause;

		[SerializeField]
		private float m_displayPause;

		private bool isDisabling;

		public GameObject m_VFX;

		[SerializeField]
		public GameObjectType m_objectType;

		public Collider TargetCollider
		{
			get
			{
				return m_targetCollider;
			}
			set
			{
				m_targetCollider = value;
			}
		}

		public float DisplayPause
		{
			get
			{
				return m_displayPause;
			}
			set
			{
				m_displayPause = value;
			}
		}

		public override Type EventType
		{
			get
			{
				return typeof(FlowDialog);
			}
		}

		public event Handler Exiting;

		protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			if (enumValue.Equals(m_dialog))
			{
				StartCoroutine(WaitAndEnable());
			}
			else if (enumValue.Equals(FlowDialog.Hide_Dialog) && GetComponent<UIPanel>().enabled && !isDisabling)
			{
				OnExclusivityLost();
			}
		}

		private void OnExclusivityLost()
		{
			isDisabling = true;
			DoDisable();
			isDisabling = false;
		}

		public IEnumerator WaitAndEnable()
		{
			if ((bool)m_VFX)
			{
				m_VFX.SetActive(false);
			}
			yield return StartCoroutine(SingletonInstance<ModalityMediator>.Instance.WaitAndAcquire(this, OnExclusivityLost));
			if (m_enablePause != 0f)
			{
				yield return new WaitForSeconds(m_enablePause);
			}
			InternalSetEnabled(true);
			if (m_displayPause != 0f)
			{
				m_WidgetPanel.enabled = false;
				yield return new WaitForSeconds(m_displayPause);
				m_WidgetPanel.enabled = true;
			}
			if (!m_targetCollider)
			{
				m_targetCollider = GameObjectMarker.GetGameObjectByMarker(m_objectType).collider;
			}
			bool targetSpecified = m_targetCollider != null;
			bool haveOwnCollider = base.collider != null;
			if (targetSpecified && !haveOwnCollider)
			{
				GameObject dummyObj = new GameObject("Collider Positioner");
				dummyObj.transform.parent = m_targetCollider.transform;
				dummyObj.transform.localPosition = Vector3.zero;
				dummyObj.transform.localScale = Vector3.one;
				dummyObj.transform.localRotation = Quaternion.identity;
				dummyObj.transform.parent = base.transform;
				if (m_targetCollider.GetType() == typeof(BoxCollider))
				{
					BoxCollider box = base.gameObject.AddComponent<BoxCollider>();
					BoxCollider otherBox = m_targetCollider as BoxCollider;
					box.center = Vector3.Scale(otherBox.center, dummyObj.transform.localScale) + dummyObj.transform.localPosition;
					box.size = Vector3.Scale(otherBox.size, dummyObj.transform.localScale);
				}
				else if (m_targetCollider.GetType() == typeof(SphereCollider))
				{
					SphereCollider sphere = base.gameObject.AddComponent<SphereCollider>();
					SphereCollider otherSphere = m_targetCollider as SphereCollider;
					sphere.center = base.transform.InverseTransformPoint(otherSphere.transform.TransformPoint(otherSphere.center));
					sphere.radius = otherSphere.radius * dummyObj.transform.localScale.x;
				}
				UnityEngine.Object.Destroy(dummyObj);
			}
			if ((bool)m_VFX)
			{
				m_VFX.SetActive(true);
			}
		}

		private void DoDisable()
		{
			SingletonInstance<ModalityMediator>.Instance.Release(this);
			if (m_exitMessage.IsTypeSet())
			{
				GameEventRouter.SendEvent(m_exitMessage);
			}
			if (this.Exiting != null)
			{
				this.Exiting();
			}
			SetDisabled(0f);
			if ((bool)m_VFX)
			{
				m_VFX.SetActive(false);
			}
			if (base.collider != null)
			{
				UnityEngine.Object.Destroy(base.collider);
			}
		}

		private void OnClick()
		{
			if (m_targetCollider != null)
			{
				m_targetCollider.gameObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			}
			if (m_exitOnClick)
			{
				DoDisable();
			}
		}

		private void OnPress(bool isDown)
		{
			if (m_targetCollider != null)
			{
				m_targetCollider.gameObject.SendMessage("OnPress", isDown, SendMessageOptions.DontRequireReceiver);
			}
			if (m_exitOnPress)
			{
				DoDisable();
			}
		}

		private void OnRelease()
		{
			if (m_targetCollider != null)
			{
				m_targetCollider.gameObject.SendMessage("OnRelease", SendMessageOptions.DontRequireReceiver);
			}
		}

		private void OnHover(bool isOver)
		{
			if (m_targetCollider != null)
			{
				m_targetCollider.gameObject.SendMessage("OnHover", isOver, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
