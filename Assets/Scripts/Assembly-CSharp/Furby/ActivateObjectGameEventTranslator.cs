using Relentless;

namespace Furby
{
	public class ActivateObjectGameEventTranslator : GameEventTranslator<ActivateObjectReaction>
	{
		public ActivateObjectReaction[] m_eventTable;

		protected override ActivateObjectReaction[] EventTable
		{
			get
			{
				return m_eventTable;
			}
		}
	}
}
