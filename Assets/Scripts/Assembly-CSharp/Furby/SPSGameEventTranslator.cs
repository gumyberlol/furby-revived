using Relentless;

namespace Furby
{
	public class SPSGameEventTranslator : GameEventTranslator<SPSReaction>
	{
		public SPSReaction[] m_eventTable;

		protected override SPSReaction[] EventTable
		{
			get
			{
				return m_eventTable;
			}
		}
	}
}
