using System;
using Furby;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class MovieTextureReaction : GameEventReaction
	{
		public string m_videoFilename;

		public string m_fabricEventName;

		public string m_CaptionText;

		private MobileMovieTexturePlayer m_moviePlayer;

		private UILabel m_CaptionLabel;

		private GameObject m_CloseButton;

		public void Setup(MobileMovieTexturePlayer player, UILabel captionLabel, GameObject closeButton)
		{
			m_moviePlayer = player;
			m_CaptionLabel = captionLabel;
			m_CloseButton = closeButton;
		}

		public override void React(GameObject gameObject, params object[] paramlist)
		{
			if (m_moviePlayer == null)
			{
				MobileMovieTexturePlayer[] array = UnityEngine.Object.FindObjectsOfType(typeof(MobileMovieTexturePlayer)) as MobileMovieTexturePlayer[];
				MobileMovieTexturePlayer[] array2 = array;
				foreach (MobileMovieTexturePlayer mobileMovieTexturePlayer in array2)
				{
					if (mobileMovieTexturePlayer.gameObject.name == "VideoPlayer")
					{
						m_moviePlayer = mobileMovieTexturePlayer;
					}
				}
			}
			if ((bool)m_moviePlayer)
			{
				if ((bool)m_CaptionLabel)
				{
					m_CaptionLabel.text = Singleton<Localisation>.Instance.GetText(m_CaptionText);
				}
				bool active = FurbyGlobals.VideoDecider.HavePlayedVideo(m_videoFilename);
				if ((bool)m_CloseButton)
				{
					m_CloseButton.SetActive(active);
				}
				m_moviePlayer.PlayVideo(m_videoFilename, m_fabricEventName);
				GameEventRouter.AddDelegateForEnums(VideoFinished, VideoPlayerGameEvents.VideoHasFinished);
			}
		}

		private void VideoFinished(Enum enumValue, GameObject gameObject, params object[] list)
		{
			GameEventRouter.RemoveDelegateForEnums(VideoFinished, VideoPlayerGameEvents.VideoHasFinished);
		}
	}
}
