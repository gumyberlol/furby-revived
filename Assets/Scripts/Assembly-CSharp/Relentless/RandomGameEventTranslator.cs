namespace Relentless
{
	public class RandomGameEventTranslator : GameEventTranslator<RandomReaction>
	{
		public RandomReaction[] m_eventTable;

		protected override RandomReaction[] EventTable
		{
			get
			{
				return m_eventTable;
			}
		}
	}
}
