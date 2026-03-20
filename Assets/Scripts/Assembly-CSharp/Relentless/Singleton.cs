using UnityEngine;

namespace Relentless
{
	public class Singleton<T> : RelentlessMonoBehaviour where T : MonoBehaviour
	{
		private static volatile object m_lock = new object();

		private static volatile T s_instance;

		public static T Instance
		{
			get
			{
				lock (m_lock)
				{
					if (s_instance == null || s_instance.gameObject == null)
					{
						s_instance = (T)Object.FindObjectOfType(typeof(T));
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
					return Instance != null;
				}
			}
		}

		public virtual void OnDestroy()
		{
			lock (m_lock)
			{
				s_instance = (T)null;
			}
		}
	}
}
