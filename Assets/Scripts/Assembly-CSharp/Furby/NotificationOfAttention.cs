using System;
using Furby.Scanner;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class NotificationOfAttention : MonoBehaviour
	{
		[SerializeField]
		private float m_pauseTime = 12f;

		private DateTime? m_notificationTime;

		private ScannerBehaviour m_scanner;

		private bool m_VideoIsPlaying;

		private GameEventSubscription m_VideoEventSubscription;

		private void Start()
		{
			FurbyBaby inProgressFurbyBaby = FurbyGlobals.Player.InProgressFurbyBaby;
			if (inProgressFurbyBaby != null)
			{
				inProgressFurbyBaby.PlayerNotifiedOfAttentionPoint = false;
			}
			m_scanner = (ScannerBehaviour)UnityEngine.Object.FindObjectOfType(typeof(ScannerBehaviour));
			m_VideoEventSubscription = new GameEventSubscription(typeof(VideoPlayerGameEvents), OnHandleVideoEvent);
		}

		private void OnDestroy()
		{
			m_VideoEventSubscription.Dispose();
		}

		private void OnHandleVideoEvent(Enum evtType, GameObject gObj, params object[] parameters)
		{
			switch ((VideoPlayerGameEvents)(object)evtType)
			{
			case VideoPlayerGameEvents.VideoHasStarted:
				m_VideoIsPlaying = true;
				break;
			case VideoPlayerGameEvents.VideoHasFinished:
				m_VideoIsPlaying = false;
				break;
			}
		}

		private void Update()
		{
			if ((m_scanner != null && m_scanner.IsBusy()) || m_VideoIsPlaying)
			{
				return;
			}
			if (!m_notificationTime.HasValue)
			{
				m_notificationTime = DateTime.Now.AddSeconds(m_pauseTime);
			}
			FurbyBaby inProgressFurbyBaby = FurbyGlobals.Player.InProgressFurbyBaby;
			if (inProgressFurbyBaby != null && inProgressFurbyBaby.Progress == FurbyBabyProgresss.E && inProgressFurbyBaby.IncubationProgress > 0f && inProgressFurbyBaby.NextAttentionPointTime <= DateTime.Now)
			{
				DateTime? notificationTime = m_notificationTime;
				if (notificationTime.HasValue && notificationTime.Value <= DateTime.Now && !inProgressFurbyBaby.PlayerNotifiedOfAttentionPoint)
				{
					GameEventRouter.SendEvent(EggNotificationEvent.EggNeedsAttention);
					inProgressFurbyBaby.PlayerNotifiedOfAttentionPoint = true;
				}
			}
		}
	}
}
