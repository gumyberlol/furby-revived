using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Help
{
	public class HelpData : ScriptableObject
	{
		public string m_rootDirectory;

		public string m_titleLocalisedKey;

		public Texture m_iconTexture;

		public List<HelpPageData> m_pages;

		public MobileMovieTexturePlayer m_moviePlayer;

		public UILabel m_VideoText_Caption;

		public GameObject m_SubtitleGameObject;
	}
}
