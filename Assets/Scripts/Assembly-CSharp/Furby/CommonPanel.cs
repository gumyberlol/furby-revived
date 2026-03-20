using System.Collections;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby
{
	public abstract class CommonPanel : GameEventReceiver
	{
		protected UIPanel m_WidgetPanel;

		private BoxCollider[] m_WidgetColliders;

		[SerializeField]
		private bool m_exclusive;

		private UICamera[] m_disabledCameras;

		public void Awake()
		{
			m_WidgetColliders = GetComponentsInChildren<BoxCollider>();
			m_WidgetPanel = GetComponent<UIPanel>();
		}

		protected virtual void OnDestroy()
		{
			if (SingletonInstance<ModalityMediator>.Exists)
			{
				SingletonInstance<ModalityMediator>.Instance.Release(this);
			}
		}

		public void Start()
		{
			BoxCollider[] widgetColliders = m_WidgetColliders;
			foreach (BoxCollider boxCollider in widgetColliders)
			{
				boxCollider.enabled = m_WidgetPanel.enabled;
			}
			OnToggleWidgets(m_WidgetPanel.enabled);
			m_disabledCameras = (from camera in Camera.allCameras
				where camera.gameObject.layer != base.gameObject.layer
				let uicamera = camera.GetComponent<UICamera>()
				where uicamera != null
				where uicamera.enabled
				select uicamera).ToArray();
		}

		protected bool IsPanelEnabled()
		{
			return m_WidgetPanel.enabled;
		}

		public void SetEnabled(bool enabled)
		{
			StartCoroutine(SetEnabledThereCanBeOnlyOne(enabled));
		}

		private IEnumerator SetEnabledThereCanBeOnlyOne(bool enabled)
		{
			if (enabled && m_exclusive)
			{
				yield return StartCoroutine(SingletonInstance<ModalityMediator>.Instance.WaitAndAcquire(this, null));
			}
			InternalSetEnabled(enabled);
		}

		protected virtual void InternalSetEnabled(bool enabled)
		{
			m_WidgetColliders = GetComponentsInChildren<BoxCollider>();
			CancelInvoke("_InternalDisable");
			if (m_WidgetColliders != null)
			{
				BoxCollider[] widgetColliders = m_WidgetColliders;
				foreach (BoxCollider boxCollider in widgetColliders)
				{
					boxCollider.enabled = enabled;
				}
			}
			if (m_exclusive)
			{
				GameEventRouter.SendEvent(SharedGuiEvents.MessageBoxAppear);
				if (m_disabledCameras != null)
				{
					UICamera[] disabledCameras = m_disabledCameras;
					foreach (UICamera uICamera in disabledCameras)
					{
						if (uICamera.gameObject.layer != 31)
						{
							uICamera.enabled = false;
						}
					}
				}
			}
			if (m_WidgetColliders != null)
			{
				OnToggleWidgets(enabled);
			}
		}

		public virtual void SetDisabled(float disableTime)
		{
			BoxCollider[] widgetColliders = m_WidgetColliders;
			foreach (BoxCollider boxCollider in widgetColliders)
			{
				if (boxCollider != null)
				{
					boxCollider.enabled = false;
				}
			}
			GameEventRouter.SendEvent(SharedGuiEvents.MessageBoxDisappear);
			if (m_disabledCameras != null)
			{
				UICamera[] disabledCameras = m_disabledCameras;
				foreach (UICamera uICamera in disabledCameras)
				{
					if (uICamera.gameObject.layer != 31)
					{
						uICamera.enabled = true;
					}
				}
			}
			if (disableTime == 0f)
			{
				_InternalDisable();
			}
			else
			{
				Invoke("_InternalDisable", disableTime);
			}
			SingletonInstance<ModalityMediator>.Instance.Release(this);
		}

		public void _InternalDisable()
		{
			OnToggleWidgets(false);
		}

		protected virtual void OnToggleWidgets(bool state)
		{
			m_WidgetPanel.enabled = state;
		}
	}
}
