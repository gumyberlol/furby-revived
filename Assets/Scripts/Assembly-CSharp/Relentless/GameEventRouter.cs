using System;
using System.Collections.Generic;
using UnityEngine;

namespace Relentless
{
	public class GameEventRouter : RelentlessMonoBehaviour
	{
		public delegate void EventHandler(Enum eventType, GameObject gameObject, params object[] parameters);

		private static volatile object m_lock = new object();

		private static volatile GameEventRouter s_instance;

		private static volatile bool s_alreadyDestroyed = false;

		private Dictionary<Type, EventHandler> m_typeSpecificHandlers = new Dictionary<Type, EventHandler>();

		private Dictionary<Enum, EventHandler> m_enumSpecificHandlers = new Dictionary<Enum, EventHandler>();

		private EventHandler m_onAnyEvent;

		public static GameEventRouter Instance
		{
			get
			{
				lock (m_lock)
				{
					if (s_instance == null || s_instance.gameObject == null)
					{
						s_instance = (GameEventRouter)UnityEngine.Object.FindObjectOfType(typeof(GameEventRouter));
						if (s_instance == null && !s_alreadyDestroyed)
						{
							GameObject gameObject = new GameObject("Game Event Router");
							s_instance = gameObject.AddComponent<GameEventRouter>();
							UnityEngine.Object.DontDestroyOnLoad(gameObject);
						}
					}
					return s_instance;
				}
			}
		}

		public static bool Exists
		{
			get
			{
				lock (m_lock)
				{
					return s_instance != null;
				}
			}
		}

		public static void AddDelegateForType(Type type, EventHandler handler)
		{
			EventHandler value;
			if (!Instance.m_typeSpecificHandlers.TryGetValue(type, out value))
			{
				Instance.m_typeSpecificHandlers.Add(type, handler);
				return;
			}
			value = (EventHandler)Delegate.Combine(value, handler);
			Instance.m_typeSpecificHandlers[type] = value;
		}

		public static void RemoveDelegateForType(Type type, EventHandler handler)
		{
			EventHandler value;
			if (!(s_instance == null) && Instance.m_typeSpecificHandlers.TryGetValue(type, out value))
			{
				value = (EventHandler)Delegate.Remove(value, handler);
				if (value == null)
				{
					Instance.m_typeSpecificHandlers.Remove(type);
				}
				else
				{
					Instance.m_typeSpecificHandlers[type] = value;
				}
			}
		}

		public static void AddDelegateForEnums(EventHandler handler, params Enum[] enumValues)
		{
			foreach (Enum key in enumValues)
			{
				EventHandler value;
				if (!Instance.m_enumSpecificHandlers.TryGetValue(key, out value))
				{
					Instance.m_enumSpecificHandlers.Add(key, handler);
					continue;
				}
				value = (EventHandler)Delegate.Combine(value, handler);
				Instance.m_enumSpecificHandlers[key] = value;
			}
		}

		public static void RemoveDelegateForEnums(EventHandler handler, params Enum[] enumValues)
		{
			if (s_instance == null)
			{
				return;
			}
			foreach (Enum key in enumValues)
			{
				EventHandler value;
				if (Instance.m_enumSpecificHandlers.TryGetValue(key, out value))
				{
					value = (EventHandler)Delegate.Remove(value, handler);
					if (value == null)
					{
						Instance.m_enumSpecificHandlers.Remove(key);
					}
					else
					{
						Instance.m_enumSpecificHandlers[key] = value;
					}
				}
			}
		}

		public static void AddDelegateForAll(EventHandler handler)
		{
			GameEventRouter instance = Instance;
			instance.m_onAnyEvent = (EventHandler)Delegate.Combine(instance.m_onAnyEvent, handler);
		}

		public static void RemoveDelegateForAll(EventHandler handler)
		{
			if (!(s_instance == null))
			{
				GameEventRouter instance = Instance;
				instance.m_onAnyEvent = (EventHandler)Delegate.Remove(instance.m_onAnyEvent, handler);
			}
		}

		public static void SendEvent(Enum eventType, GameObject gameObject, params object[] parameters)
		{
			if (s_instance == null)
			{
				return;
			}
			EventHandler value;
			if (Instance.m_enumSpecificHandlers.TryGetValue(eventType, out value))
			{
				Delegate[] invocationList = value.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					EventHandler eventHandler = (EventHandler)invocationList[i];
					try
					{
						eventHandler(eventType, gameObject, parameters);
					}
					catch (Exception ex)
					{
						Logging.LogError(string.Format("{0}\n{1}", ex.Message, ex.StackTrace));
					}
				}
			}
			if (Instance.m_typeSpecificHandlers.TryGetValue(eventType.GetType(), out value))
			{
				Delegate[] invocationList2 = value.GetInvocationList();
				for (int j = 0; j < invocationList2.Length; j++)
				{
					EventHandler eventHandler2 = (EventHandler)invocationList2[j];
					try
					{
						eventHandler2(eventType, gameObject, parameters);
					}
					catch (Exception ex2)
					{
						Logging.LogError(string.Format("{0}\n{1}", ex2.Message, ex2.StackTrace));
					}
				}
			}
			if (Instance.m_onAnyEvent == null)
			{
				return;
			}
			Delegate[] invocationList3 = Instance.m_onAnyEvent.GetInvocationList();
			for (int k = 0; k < invocationList3.Length; k++)
			{
				EventHandler eventHandler3 = (EventHandler)invocationList3[k];
				try
				{
					eventHandler3(eventType, gameObject, parameters);
				}
				catch (Exception ex3)
				{
					Logging.LogError(string.Format("{0}\n{1}", ex3.Message, ex3.StackTrace));
				}
			}
		}

		public static void SendEvent(Enum eventType)
		{
			SendEvent(eventType, null);
		}

		public void OnDestroy()
		{
			lock (m_lock)
			{
				m_onAnyEvent = null;
				while (m_typeSpecificHandlers.Count != 0)
				{
					Dictionary<Type, EventHandler>.Enumerator enumerator = m_typeSpecificHandlers.GetEnumerator();
					if (enumerator.MoveNext())
					{
						m_typeSpecificHandlers[enumerator.Current.Key] = null;
						m_typeSpecificHandlers.Remove(enumerator.Current.Key);
					}
				}
				m_typeSpecificHandlers = null;
				s_instance = null;
				s_alreadyDestroyed = true;
			}
		}
	}
}
