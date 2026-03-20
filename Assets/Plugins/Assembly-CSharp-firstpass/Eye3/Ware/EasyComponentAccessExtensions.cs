using UnityEngine;

namespace Eye3.Ware
{
	public static class EasyComponentAccessExtensions
	{
		public static T Cmp<T>(this object obj) where T : Component
		{
			GameObject gameObject = obj as GameObject;
			if (gameObject == null)
			{
				Component component = obj as Component;
				if (component == null)
				{
					return (T)null;
				}
				return component.GetComponent<T>();
			}
			return gameObject.GetComponent<T>();
		}

		public static T[] Cmps<T>(this object obj) where T : Component
		{
			GameObject gameObject = obj as GameObject;
			if (gameObject == null)
			{
				Component component = obj as Component;
				if (component == null)
				{
					return null;
				}
				return component.GetComponents<T>();
			}
			return gameObject.GetComponents<T>();
		}

		public static T CmpKid<T>(this object obj) where T : Component
		{
			GameObject gameObject = obj as GameObject;
			if (gameObject == null)
			{
				Component component = obj as Component;
				if (component == null)
				{
					return (T)null;
				}
				return component.GetComponentInChildren<T>();
			}
			return gameObject.GetComponentInChildren<T>();
		}

		public static T[] CmpKids<T>(this object obj) where T : Component
		{
			GameObject gameObject = obj as GameObject;
			if (gameObject == null)
			{
				Component component = obj as Component;
				if (component == null)
				{
					return null;
				}
				return component.GetComponentsInChildren<T>();
			}
			return gameObject.GetComponentsInChildren<T>();
		}
	}
}
