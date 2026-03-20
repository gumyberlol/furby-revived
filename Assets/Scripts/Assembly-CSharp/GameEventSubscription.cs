using System;
using Relentless;

public class GameEventSubscription : IDisposable
{
	private enum Mode
	{
		Everything = 0,
		Types = 1,
		Values = 2
	}

	private Mode m_mode;

	private Type m_subscriptionType;

	private GameEventRouter.EventHandler m_handler;

	private Enum[] m_enumValues;

	public GameEventSubscription(GameEventRouter.EventHandler handler)
	{
		m_handler = handler;
		GameEventRouter.AddDelegateForAll(handler);
		m_mode = Mode.Everything;
	}

	public GameEventSubscription(Type eventType, GameEventRouter.EventHandler handler)
	{
		m_handler = handler;
		m_subscriptionType = eventType;
		GameEventRouter.AddDelegateForType(eventType, handler);
		m_mode = Mode.Types;
	}

	public GameEventSubscription(GameEventRouter.EventHandler handler, params Enum[] enumValues)
	{
		m_handler = handler;
		m_enumValues = enumValues;
		GameEventRouter.AddDelegateForEnums(handler, enumValues);
		m_mode = Mode.Values;
	}

	public void Dispose()
	{
		switch (m_mode)
		{
		case Mode.Everything:
			GameEventRouter.RemoveDelegateForAll(m_handler);
			break;
		case Mode.Types:
			GameEventRouter.RemoveDelegateForType(m_subscriptionType, m_handler);
			break;
		case Mode.Values:
			GameEventRouter.RemoveDelegateForEnums(m_handler, m_enumValues);
			break;
		}
	}
}
