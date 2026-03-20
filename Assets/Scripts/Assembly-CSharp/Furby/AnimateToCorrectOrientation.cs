using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class AnimateToCorrectOrientation : MonoBehaviour
	{
		[SerializeField]
		public AnimationClip m_RotationAnimationClip;

		[HideInInspector]
		private GameEventSubscription m_DebugPanelSub;

		[SerializeField]
		public float m_DurationSecsToWait_BeforeSuspending = 0.1f;

		[SerializeField]
		public GameObject[] m_GameObjectsWithAnimationsToSuspend = new GameObject[0];

		private IEnumerator Start()
		{
			return RunSequence();
		}

		private IEnumerator RunSequence()
		{
			if (RequiresRotation())
			{
				GameEventRouter.SendEvent(AnimateToCorrectOrientationEvent.RotationStarted);
				yield return new WaitForSeconds(m_DurationSecsToWait_BeforeSuspending);
				SuspendWidgets();
				_InvokeAnimation(m_RotationAnimationClip);
				yield return null;
				do
				{
					yield return null;
				}
				while (base.animation.isPlaying);
				base.gameObject.transform.localEulerAngles = Vector3.zero;
				Screen.orientation = ScreenOrientationHelper.GetCorrectOrientationForDevice_OrCustomizedOverride();
				ResumeWidgets();
				GameEventRouter.SendEvent(AnimateToCorrectOrientationEvent.RotationEnded);
			}
			else
			{
				GameEventRouter.SendEvent(AnimateToCorrectOrientationEvent.NoRotationRequired);
			}
		}

		private void SuspendWidgets()
		{
			SetAnimationSpeedOnGameObjects(0f);
		}

		private void ResumeWidgets()
		{
			SetAnimationSpeedOnGameObjects(1f);
		}

		private void SetAnimationSpeedOnGameObjects(float speed)
		{
			GameObject[] gameObjectsWithAnimationsToSuspend = m_GameObjectsWithAnimationsToSuspend;
			foreach (GameObject gameObject in gameObjectsWithAnimationsToSuspend)
			{
				Animation component = gameObject.GetComponent<Animation>();
				if (!component)
				{
					continue;
				}
				foreach (AnimationState item in component)
				{
					item.speed = speed;
				}
			}
		}

		private void _InvokeAnimation(AnimationClip currentGimbalAnimationClip)
		{
			Animation animation = base.gameObject.GetComponent<Animation>();
			if (animation == null)
			{
				animation = base.gameObject.AddComponent<Animation>();
			}
			animation.AddClip(currentGimbalAnimationClip, currentGimbalAnimationClip.name);
			animation.wrapMode = WrapMode.Once;
			animation.Play(currentGimbalAnimationClip.name, PlayMode.StopSameLayer);
			Logging.Log("ORIENT. Started Animation: " + currentGimbalAnimationClip.name);
		}

		private bool RequiresRotation()
		{
			ScreenOrientation currentOrientation = ScreenOrientationHelper.GetCurrentOrientation();
			Logging.Log("ORIENT. Current: " + currentOrientation);
			ScreenOrientation correctOrientationForDevice_OrCustomizedOverride = ScreenOrientationHelper.GetCorrectOrientationForDevice_OrCustomizedOverride();
			Logging.Log("ORIENT. Desired: " + correctOrientationForDevice_OrCustomizedOverride);
			if (currentOrientation != correctOrientationForDevice_OrCustomizedOverride)
			{
				return true;
			}
			Logging.Log("ORIENT. Current and Desired match, no need to animate");
			return false;
		}
	}
}
