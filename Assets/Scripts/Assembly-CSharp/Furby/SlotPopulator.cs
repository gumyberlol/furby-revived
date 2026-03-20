using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class SlotPopulator : MonoBehaviour
	{
		[SerializeField]
		private string m_QRCode = string.Empty;

		[SerializeField]
		private string m_VariantCode = string.Empty;

		[SerializeField]
		public SlotReference[] m_SlotReferencesX2;

		[SerializeField]
		public SlotReference[] m_SlotReferencesX3;

		private SlotMode m_SlotMode;

		public string QRCode
		{
			get
			{
				return m_QRCode;
			}
			set
			{
				m_QRCode = value;
			}
		}

		public string VariantCode
		{
			get
			{
				return m_VariantCode;
			}
			set
			{
				m_VariantCode = value;
			}
		}

		public SlotReference[] SlotReferencesX2
		{
			get
			{
				return m_SlotReferencesX2;
			}
			set
			{
				m_SlotReferencesX2 = value;
			}
		}

		public SlotReference[] SlotReferencesX3
		{
			get
			{
				return m_SlotReferencesX3;
			}
			set
			{
				m_SlotReferencesX3 = value;
			}
		}

		public void SetInSlotMode(SlotMode mode)
		{
			m_SlotMode = mode;
			switch (m_SlotMode)
			{
			case SlotMode.TwoSlots:
			{
				SlotReference[] slotReferencesX3 = SlotReferencesX2;
				foreach (SlotReference slotReference3 in slotReferencesX3)
				{
					slotReference3.Collider.enabled = true;
					slotReference3.Handler.enabled = true;
				}
				SlotReference[] slotReferencesX4 = SlotReferencesX3;
				foreach (SlotReference slotReference4 in slotReferencesX4)
				{
					slotReference4.Collider.enabled = false;
					slotReference4.Handler.enabled = false;
					slotReference4.Handler.gameObject.SetActive(false);
				}
				break;
			}
			case SlotMode.ThreeSlots:
			{
				SlotReference[] slotReferencesX = SlotReferencesX2;
				foreach (SlotReference slotReference in slotReferencesX)
				{
					slotReference.Collider.enabled = false;
					slotReference.Handler.enabled = false;
					slotReference.Handler.gameObject.SetActive(false);
				}
				SlotReference[] slotReferencesX2 = SlotReferencesX3;
				foreach (SlotReference slotReference2 in slotReferencesX2)
				{
					slotReference2.Collider.enabled = true;
					slotReference2.Handler.enabled = true;
				}
				break;
			}
			}
		}

		private void Activate()
		{
			base.gameObject.SetActive(true);
		}

		private void Deactivate()
		{
			base.gameObject.SetActive(false);
		}

		public IEnumerator WaitForActivationCodeToChange()
		{
			Activate();
			SetColliderEnabledState(true);
			m_QRCode = string.Empty;
			while (m_QRCode == string.Empty)
			{
				yield return null;
			}
			SetColliderEnabledState(false);
			Deactivate();
		}

		private void SetColliderEnabledState(bool newState)
		{
			switch (m_SlotMode)
			{
			case SlotMode.TwoSlots:
			{
				SlotReference[] slotReferencesX2 = SlotReferencesX2;
				foreach (SlotReference slotReference2 in slotReferencesX2)
				{
					slotReference2.m_Collider.enabled = newState;
				}
				break;
			}
			case SlotMode.ThreeSlots:
			{
				SlotReference[] slotReferencesX = SlotReferencesX3;
				foreach (SlotReference slotReference in slotReferencesX)
				{
					slotReference.m_Collider.enabled = newState;
				}
				break;
			}
			}
		}

		public IEnumerator PresentationFeedback(SlotHandler chosen)
		{
			Logging.Log("SlotPopulator.PresentationFeedback, chose: " + chosen.QRCode + "|" + chosen.VariantCode);
			float animationDurationSecs = 1f;
			GameEventRouter.SendEvent(EggProductUnlockingEvents.UnlockChoiceChosen);
			SetColliderEnabledState(false);
			SlotReference chosenSlotRef = null;
			switch (m_SlotMode)
			{
			case SlotMode.TwoSlots:
			{
				SlotReference[] slotReferencesX2 = SlotReferencesX2;
				foreach (SlotReference slotref2 in slotReferencesX2)
				{
					if (chosen.gameObject == slotref2.Handler.gameObject)
					{
						chosenSlotRef = slotref2;
						continue;
					}
					ScaleSlotDown(slotref2, animationDurationSecs);
					slotref2.VFX.SetActive(false);
				}
				break;
			}
			case SlotMode.ThreeSlots:
			{
				SlotReference[] slotReferencesX = SlotReferencesX3;
				foreach (SlotReference slotref in slotReferencesX)
				{
					if (chosen.gameObject == slotref.Handler.gameObject)
					{
						chosenSlotRef = slotref;
						continue;
					}
					ScaleSlotDown(slotref, animationDurationSecs);
					slotref.VFX.SetActive(false);
				}
				break;
			}
			}
			MoveSlotToCenter(chosenSlotRef, animationDurationSecs);
			yield return new WaitForSeconds(animationDurationSecs * 1.5f);
			ScaleSlotDown(chosenSlotRef, animationDurationSecs);
			yield return new WaitForSeconds(animationDurationSecs * 1.5f);
			QRCode = chosen.QRCode;
			VariantCode = chosen.VariantCode;
		}

		private void ScaleSlotDown(SlotReference slotref, float animationDurationSecs)
		{
			TweenScale.Begin(slotref.Handler.gameObject, animationDurationSecs, new Vector3(0f, 0f)).method = UITweener.Method.EaseOut;
		}

		private void MoveSlotToCenter(SlotReference slotref, float animationDurationSecs)
		{
			TweenTransform.Begin(slotref.Handler.gameObject, animationDurationSecs, base.gameObject.transform).method = UITweener.Method.EaseInOut;
		}
	}
}
