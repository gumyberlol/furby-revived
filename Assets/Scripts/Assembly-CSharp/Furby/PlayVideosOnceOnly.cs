using Relentless;

namespace Furby
{
	public class PlayVideosOnceOnly : VideoDecision
	{
		public override bool ShouldPlayVideo(string videoName)
		{
			return !Singleton<GameDataStoreObject>.Instance.Data.m_videosPlayed.Contains(videoName);
		}

		public override bool HavePlayedVideo(string videoName)
		{
			return Singleton<GameDataStoreObject>.Instance.Data.m_videosPlayed.Contains(videoName);
		}

		public override void MarkVideoAsPlayed(string videoName)
		{
			if (!Singleton<GameDataStoreObject>.Instance.Data.m_videosPlayed.Contains(videoName))
			{
				Singleton<GameDataStoreObject>.Instance.Data.m_videosPlayed.Add(videoName);
				Singleton<GameDataStoreObject>.Instance.Save();
			}
		}
	}
}
