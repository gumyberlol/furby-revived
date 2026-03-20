using UnityEngine;

namespace Relentless
{
	public class MovieTextureGameEventTranslator : GameEventTranslator<MovieTextureReaction>
	{
		public MovieTextureReaction[] m_eventTable;

		public MobileMovieTexturePlayer m_moviePlayer;

		public UILabel m_CaptionLabel;

		public GameObject m_CloseButton;

		protected override MovieTextureReaction[] EventTable
		{
			get
			{
				return m_eventTable;
			}
		}

		private void Start()
		{
			MovieTextureReaction[] eventTable = m_eventTable;
			foreach (MovieTextureReaction movieTextureReaction in eventTable)
			{
				movieTextureReaction.Setup(m_moviePlayer, m_CaptionLabel, m_CloseButton);
			}
		}
	}
}
