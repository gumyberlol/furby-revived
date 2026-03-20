using Relentless;

namespace Furby.MiniGames.HideAndSeek
{
	public class SpecialObject : RelentlessMonoBehaviour
	{
		private SpecialHitObjectType m_SpecialObjectType;

		public SpecialHitObjectType SpecialObjectType
		{
			get
			{
				return m_SpecialObjectType;
			}
			set
			{
				m_SpecialObjectType = value;
			}
		}
	}
}
