using Fabric;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class FurbyBeakSync : MonoBehaviour
	{
		private Transform m_topBeak;

		private Transform m_bottomBeak;

		private bool m_initialised;

		private ModifiedVolumeMeter m_volumeMeter;

		private Quaternion m_initialTopRotation;

		private Quaternion m_initialBottomRotation;

		private FurbyBeakSyncData m_beakSyncData;

		private Animation m_animationComponentToWatch;

		public void AssignBeakControllers(Transform topBeak, Transform bottomBeak, FurbyBeakSyncData beakSyncData, Animation animationComponent)
		{
			if (animationComponent != null && beakSyncData != null && topBeak != null && bottomBeak != null)
			{
				m_beakSyncData = beakSyncData;
				m_topBeak = topBeak;
				m_bottomBeak = bottomBeak;
				m_animationComponentToWatch = animationComponent;
				Object[] array = Object.FindObjectsOfType(typeof(ModifiedVolumeMeter));
				Object[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					ModifiedVolumeMeter modifiedVolumeMeter = (ModifiedVolumeMeter)array2[i];
					if (modifiedVolumeMeter.name == m_beakSyncData.m_volumeMeterName)
					{
						m_volumeMeter = modifiedVolumeMeter;
						m_initialised = true;
						break;
					}
				}
				m_initialTopRotation = topBeak.transform.localRotation;
				m_initialBottomRotation = bottomBeak.transform.localRotation;
			}
			else
			{
				Logging.Log(string.Format("Null parameter passed in: Top Beak {0} Lower Beak {1} Animation {2} BeakSyncData {3}", topBeak, bottomBeak, animationComponent, beakSyncData));
			}
		}

		private void LateUpdate()
		{
			if (m_initialised && !m_beakSyncData.IsAnimationThatPreventsBeakSyncPlaying(ref m_animationComponentToWatch))
			{
				float num = Mathf.Sqrt(m_volumeMeter.volumeMeterState.mRMS);
				m_bottomBeak.localRotation = m_initialBottomRotation * Quaternion.AngleAxis((!(m_beakSyncData.m_lowerBeakMaxRotation > 0f)) ? Mathf.Max(m_beakSyncData.m_lowerBeakFactorRotation * num, m_beakSyncData.m_lowerBeakMaxRotation) : Mathf.Min(m_beakSyncData.m_lowerBeakFactorRotation * num, m_beakSyncData.m_lowerBeakMaxRotation), Vector3.forward);
				m_topBeak.localRotation = m_initialTopRotation * Quaternion.AngleAxis((!(m_beakSyncData.m_topBeakMaxRotation > 0f)) ? Mathf.Max(m_beakSyncData.m_topBeakFactorRotation * num, m_beakSyncData.m_topBeakMaxRotation) : Mathf.Min(m_beakSyncData.m_topBeakFactorRotation * num, m_beakSyncData.m_topBeakMaxRotation), Vector3.forward);
			}
		}
	}
}
