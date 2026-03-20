using Relentless;
using UnityEngine;

namespace Furby.MiniGames.Singalong
{
	public class SongSelect : MonoBehaviour
	{
		[SerializeField]
		private MusicGameSongData m_songToSelect;

		private void OnClick()
		{
			GameEventRouter.SendEvent(MusicGameEvent.SongSelected, base.gameObject, m_songToSelect);
		}
	}
}
