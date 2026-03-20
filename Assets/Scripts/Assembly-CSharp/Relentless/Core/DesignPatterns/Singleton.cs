using System;
using System.Reflection;

namespace Relentless.Core.DesignPatterns
{
	public sealed class Singleton<T> where T : class
	{
		private static volatile T _instance;

		private static object _lock;

		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_lock)
					{
						if (_instance == null)
						{
							ConstructorInfo constructorInfo = null;
							try
							{
								constructorInfo = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null);
							}
							catch (Exception innerException)
							{
								throw new SingletonException(innerException);
							}
							if (constructorInfo == null || constructorInfo.IsAssembly)
							{
								throw new SingletonException(string.Format("A private or protected constructor is missing for '{0}'.", typeof(T).Name));
							}
							_instance = (T)constructorInfo.Invoke(null);
						}
					}
				}
				return _instance;
			}
		}

		static Singleton()
		{
			_lock = new object();
		}
	}
}
