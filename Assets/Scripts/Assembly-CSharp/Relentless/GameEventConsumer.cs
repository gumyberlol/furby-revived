using System;
using System.Collections.Generic;
using UnityEngine;

namespace Relentless
{
	public abstract class GameEventConsumer<EnumType> : GameEventReceiver where EnumType : struct, IConvertible
	{
		private Queue<KeyValuePair<Enum, int>> m_EventQueue = new Queue<KeyValuePair<Enum, int>>();

		public EnumType? LastEvent;

		public override Type EventType
		{
			get
			{
				return typeof(EnumType);
			}
		}

		protected IEnumerable<EnumType?> Consume()
		{
			while (m_EventQueue.Count > 0)
			{
				yield return (EnumType)(ValueType)m_EventQueue.Dequeue().Key;
			}
		}

		public EnumType? PumpEvents(params EnumType[] filter)
		{
			foreach (EnumType? item in Consume())
			{
				if (item.HasValue && (filter.Length == 0 || Array.IndexOf(filter, item.Value) >= 0))
				{
					return LastEvent = item.Value;
				}
			}
			return LastEvent = null;
		}

		protected IEnumerable<EnumType?> Await(bool flushQueue, params EnumType[] filter)
		{
			if (flushQueue)
			{
				FlushPendingMessageQueue(-1);
			}
			while (!PumpEvents(filter).HasValue)
			{
				yield return null;
			}
		}

		protected IEnumerable<EnumType?> AwaitAny(bool flushQueue)
		{
			return Await(flushQueue, (EnumType[])Enum.GetValues(EventType));
		}

		protected void FlushPendingMessageQueue(int frameDelta)
		{
			while (m_EventQueue.Count > 0)
			{
				KeyValuePair<Enum, int> keyValuePair = m_EventQueue.Peek();
				if (Time.frameCount - keyValuePair.Value < frameDelta)
				{
					break;
				}
				m_EventQueue.Dequeue();
			}
		}

		protected override void OnEvent(Enum type, GameObject sender, object[] arguments)
		{
			FlushPendingMessageQueue(2);
			m_EventQueue.Enqueue(new KeyValuePair<Enum, int>(type, Time.frameCount));
		}
	}
}
