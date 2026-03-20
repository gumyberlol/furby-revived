using UnityEngine;

namespace Relentless
{
	public class SingletonInstance<T> : RelentlessMonoBehaviour where T : SingletonInstance<T>
	{
		protected static T s_instance;

		public static T Instance
		{
			get
			{
				if (s_instance == null || s_instance.gameObject == null)
				{
					s_instance = (T)Object.FindObjectOfType(typeof(T));
					if (s_instance == null)
					{
						Logging.LogError("SingletonInstance used before it is loaded : " + typeof(T).FullName);
					}
				}
				return s_instance;
			}
		}

		public static bool Exists
		{
			get
			{
				return s_instance != null;
			}
		}

		public virtual void Awake()
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}

		public virtual void OnDestroy()
		{
			s_instance = (T)null;
		}

		public void DestroyMe()
		{
			Object.DestroyImmediate(base.gameObject);
		}
	}
}
