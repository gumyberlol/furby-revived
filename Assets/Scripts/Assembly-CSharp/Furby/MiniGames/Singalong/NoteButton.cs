using Relentless;
using UnityEngine;

namespace Furby.MiniGames.Singalong
{
	public class NoteButton : MonoBehaviour
	{
		[SerializeField]
		private UISprite m_flash;

		[SerializeField]
		public int m_railIndex;

		[SerializeField]
		public ParticleSystem m_particles;

		[SerializeField]
		public MusicGameSongPlayer m_musicGameLogic;

		private void OnPress(bool isDown)
		{
			if (isDown)
			{
				GameEventRouter.SendEvent(MusicGameEvent.NotePressed, base.gameObject, m_railIndex);
				m_flash.alpha = 1f;
				if (m_musicGameLogic.CurrentPlayerState == MusicGameSongPlayer.PlayerState.DiscoMode)
				{
					m_particles.emissionRate = 32f;
				}
			}
		}

		private void Update()
		{
			if (m_flash.alpha > 0f)
			{
				m_flash.alpha -= Time.deltaTime;
			}
			if (m_particles.emissionRate > 0f)
			{
				m_particles.emissionRate -= Mathf.CeilToInt(Time.deltaTime);
			}
		}
	}
}
