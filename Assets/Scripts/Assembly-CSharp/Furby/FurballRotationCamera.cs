using System;
using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class FurballRotationCamera : GameEventReceiver
	{
		[SerializeField]
		private GameObject m_rotatePrompt;

		[SerializeField]
		private GameObject m_returnPrompt;

		[SerializeField]
		private float m_showPromptDuration = 3f;

		public override Type EventType
		{
			get
			{
				return typeof(FurBallGameEvent);
			}
		}

		private IEnumerator SendFurballTone()
		{
			yield break;
		}

		private IEnumerator ShowRotatePrompt()
		{
			base.transform.localRotation = Quaternion.AngleAxis(180f, Vector3.up);
			ScreenOrientation currentOrientation = ScreenOrientationHelper.GetCurrentOrientation();
			ScreenOrientation desiredOrientation = ScreenOrientationHelper.GetReversedOrientation();
			if (currentOrientation != desiredOrientation && m_returnPrompt != null)
			{
				m_rotatePrompt.SetActive(true);
				Screen.orientation = desiredOrientation;
				yield return new WaitForSeconds(m_showPromptDuration);
				m_rotatePrompt.SetActive(false);
			}
			yield return StartCoroutine(SendFurballTone());
			GameEventRouter.SendEvent(FurBallGameEvent.FurballStartPlayingFurbyMode);
		}

		private IEnumerator ShowReturnPrompt()
		{
			base.transform.localRotation = Quaternion.AngleAxis(0f, Vector3.up);
			ScreenOrientation currentOrientation = ScreenOrientationHelper.GetCurrentOrientation();
			ScreenOrientation desiredOrientation = ScreenOrientationHelper.GetCorrectOrientationForDevice_OrCustomizedOverride();
			if (currentOrientation != desiredOrientation && m_returnPrompt != null)
			{
				m_returnPrompt.SetActive(true);
				Screen.orientation = desiredOrientation;
				yield return new WaitForSeconds(m_showPromptDuration);
				m_returnPrompt.SetActive(false);
			}
			yield return StartCoroutine(SendFurballTone());
			GameEventRouter.SendEvent(FurBallGameEvent.FurballStartPlayingFurblingMode);
		}

		protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			switch ((FurBallGameEvent)(object)enumValue)
			{
			case FurBallGameEvent.FurballSelectFurblingGameMode:
				StartCoroutine(ShowReturnPrompt());
				break;
			case FurBallGameEvent.FurballSelectFurbyGameMode:
				StartCoroutine(ShowRotatePrompt());
				break;
			}
		}
	}
}
