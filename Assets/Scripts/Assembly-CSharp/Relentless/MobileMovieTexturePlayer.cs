using System;
using Fabric;
using Furby;
using MMT;
using UnityEngine;

namespace Relentless
{
	public class MobileMovieTexturePlayer : VideoPlayer
	{
		[SerializeField]
		private MobileMovieTexture m_player;

		[SerializeField]
		private GameObject m_displayObject;

		private string m_fabricEvent;

		private void Start()
		{
			GameEventRouter.AddDelegateForEnums(OnRequestStop, VideoPlayerGameEvents.RequestVideoStop);
			base.gameObject.SetLayerInChildren(base.gameObject.layer);
			DisableInputOnLayer[] componentsInChildren = GetComponentsInChildren<DisableInputOnLayer>(true);
			DisableInputOnLayer[] array = componentsInChildren;
			foreach (DisableInputOnLayer disableInputOnLayer in array)
			{
				disableInputOnLayer.ExcludeLayer(base.gameObject.layer);
			}
		}

		private void OnDestroy()
		{
			GameEventRouter.RemoveDelegateForEnums(OnRequestStop, VideoPlayerGameEvents.RequestVideoStop);
		}

		private void OnRequestStop(Enum eventSent, GameObject go, params object[] objectParams)
		{
			StopVideo();
		}

public override void PlayVideo(
    string videoPath,
    string fabricEvent)
{
    m_player.onFinished += 
        VideoFinishedPlaying;
    m_player.Path = videoPath;
    m_displayObject.SetActive(true);
    GameEventRouter.SendEvent(
        VideoPlayerGameEvents
        .VideoHasStarted);
    m_player.Play();
}

		private void VideoFinishedPlaying(
    MobileMovieTexture sender)
{
    StartCoroutine(DelayedStop());
}

		private System.Collections.IEnumerator 
    DelayedStop()
{
    yield return new WaitForSeconds(0.5f);
    StopVideo();
}

		public override void StopVideo()
{
    m_player.Stop();
    m_player.onFinished -= 
        VideoFinishedPlaying;
    GameEventRouter.SendEvent(
        VideoPlayerGameEvents
        .VideoHasFinished);
    m_displayObject.SetActive(false);
} // h

		public bool IsVideoPlaying()
		{
			if (m_player == null)
			{
				return false;
			}
			return m_player.isPlaying;
		}

		public static bool AnyVideosPlaying()
		{
			MobileMovieTexturePlayer[] array = GameObjectExtensions.FindObjectsOfType<MobileMovieTexturePlayer>();
			foreach (MobileMovieTexturePlayer mobileMovieTexturePlayer in array)
			{
				if (mobileMovieTexturePlayer.IsVideoPlaying())
				{
					return true;
				}
			}
			return false;
		}

		public static void StopAllVideos()
		{
			MobileMovieTexturePlayer[] array = GameObjectExtensions.FindObjectsOfType<MobileMovieTexturePlayer>();
			foreach (MobileMovieTexturePlayer mobileMovieTexturePlayer in array)
			{
				if (mobileMovieTexturePlayer.IsVideoPlaying())
				{
					mobileMovieTexturePlayer.StopVideo();
				}
			}
		}
	}
}
