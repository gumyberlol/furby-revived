using Fabric;
using Relentless;

namespace Furby.MiniGames.Singalong
{
	public class MusicGameFabricQuery : Singleton<MusicGameFabricQuery>
	{
		private ResourcesAudioComponent[] m_audioComponents;

		private void Start()
		{
			m_audioComponents = GetComponentsInChildren<ResourcesAudioComponent>();
		}

		public int GetCurrentSongProgressSamples()
		{
			int result = -1;
			ResourcesAudioComponent[] audioComponents = m_audioComponents;
			foreach (ResourcesAudioComponent resourcesAudioComponent in audioComponents)
			{
				if (resourcesAudioComponent._isComponentActive)
				{
					result = resourcesAudioComponent.GetTimeSamplesFromStart();
					break;
				}
			}
			return result;
		}

		public int GetSampleRate()
		{
			int result = 44100;
			ResourcesAudioComponent[] audioComponents = m_audioComponents;
			foreach (ResourcesAudioComponent resourcesAudioComponent in audioComponents)
			{
				if (resourcesAudioComponent._isComponentActive)
				{
					result = resourcesAudioComponent.GetSampleRate();
					break;
				}
			}
			return result;
		}

		public void SetFilter(bool shouldEnable, float freq)
		{
			ResourcesAudioComponent[] audioComponents = m_audioComponents;
			foreach (ResourcesAudioComponent resourcesAudioComponent in audioComponents)
			{
				if (resourcesAudioComponent._isComponentActive)
				{
					resourcesAudioComponent.SetFilterEnabled(shouldEnable, freq);
				}
			}
		}
	}
}
