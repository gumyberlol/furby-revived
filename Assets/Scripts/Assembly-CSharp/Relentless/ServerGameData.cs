using System;
using System.Collections.Generic;

namespace Relentless
{
	[Serializable]
	public class ServerGameData
	{
		public List<PayWallEpisode> PayWallEpisodes;

		public static ServerGameData CreateTestData()
		{
			ServerGameData serverGameData = new ServerGameData();
			serverGameData.PayWallEpisodes = new List<PayWallEpisode>
			{
				new PayWallEpisode
				{
					EpisodeId = "Episode1",
					Scenes = new List<PayWallScene>
					{
						new PayWallScene
						{
							AtStart = true,
							SceneName = "BT01_EP01_PZ01"
						}
					}
				},
				new PayWallEpisode
				{
					EpisodeId = "Episode2",
					Scenes = new List<PayWallScene>
					{
						new PayWallScene
						{
							AtStart = true,
							SceneName = "BT01_EP02_INTRO"
						}
					}
				},
				new PayWallEpisode
				{
					EpisodeId = "Episode3",
					Scenes = new List<PayWallScene>
					{
						new PayWallScene
						{
							AtStart = true,
							SceneName = "BT01_EP03_INTRO"
						}
					}
				},
				new PayWallEpisode
				{
					EpisodeId = "Episode4",
					Scenes = new List<PayWallScene>
					{
						new PayWallScene
						{
							AtStart = true,
							SceneName = "BT01_EP04_INTRO"
						}
					}
				},
				new PayWallEpisode
				{
					EpisodeId = "Episode5",
					Scenes = new List<PayWallScene>
					{
						new PayWallScene
						{
							AtStart = true,
							SceneName = "BT01_EP05_INTRO"
						}
					}
				},
				new PayWallEpisode
				{
					EpisodeId = "Episode6",
					Scenes = new List<PayWallScene>
					{
						new PayWallScene
						{
							AtStart = true,
							SceneName = "BT01_EP06_INTRO"
						}
					}
				}
			};
			return serverGameData;
		}

		public override string ToString()
		{
			return JSONSerialiser.AsString(this);
		}
	}
}
