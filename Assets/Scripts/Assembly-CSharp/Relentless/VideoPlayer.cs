using UnityEngine;

namespace Relentless
{
	public abstract class VideoPlayer : MonoBehaviour
	{
		public abstract void PlayVideo(string videoPath, string additionalAudio);

		public abstract void StopVideo();
	}
}
