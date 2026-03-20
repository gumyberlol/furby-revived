using UnityEngine;

namespace Relentless
{
	public abstract class VideoDecision : MonoBehaviour
	{
		public abstract bool ShouldPlayVideo(string videoName);

		public abstract void MarkVideoAsPlayed(string videoName);

		public abstract bool HavePlayedVideo(string videoName);
	}
}
