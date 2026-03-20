using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Relentless
{
	public static class GameObjectExtensions
	{
		public static Type[] FindObjectsOfType<Type>() where Type : Component
		{
			return UnityEngine.Object.FindObjectsOfType(typeof(Type)) as Type[];
		}

		public static Type FindSceneComponent<Type>(this GameObject rootObject, string objectName) where Type : Component
		{
			GameObject gameObject = GameObject.Find(objectName);
			if (gameObject != null)
			{
				return gameObject.GetComponent<Type>();
			}
			return (Type)null;
		}

		public static Type FindComponent<Type>(this GameObject rootObject, string objectName) where Type : Component
		{
			rootObject = rootObject.transform.Find(objectName).gameObject;
			if (rootObject != null)
			{
				return rootObject.GetComponent<Type>();
			}
			return (Type)null;
		}

		public static T GetComponentInChildrenIncludeInactive<T>(this GameObject o) where T : Component
		{
			if (o == null)
			{
				return (T)null;
			}
			T component = o.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			foreach (Transform item in o.transform)
			{
				T componentInChildrenIncludeInactive = item.gameObject.GetComponentInChildrenIncludeInactive<T>();
				if (componentInChildrenIncludeInactive != null)
				{
					return componentInChildrenIncludeInactive;
				}
			}
			return (T)null;
		}

		public static void GetComponentsInChildrenIncludeInactive<T>(this GameObject o, List<T> outList) where T : Component
		{
			if (o == null)
			{
				return;
			}
			outList.AddRange(o.GetComponents<T>());
			foreach (Transform item in o.transform)
			{
				item.gameObject.GetComponentsInChildrenIncludeInactive(outList);
			}
		}

		public static bool GetComponentsInParentIncludeInactive<T>(this GameObject o, List<T> outList, bool stopAfterFirstObjectFound) where T : Component
		{
			if (o == null)
			{
				return false;
			}
			T[] components = o.GetComponents<T>();
			if (components.Length > 0)
			{
				outList.AddRange(components);
				return true;
			}
			Transform parent = o.transform.parent;
			if (parent != null)
			{
				return parent.gameObject.GetComponentsInParentIncludeInactive(outList, stopAfterFirstObjectFound);
			}
			return false;
		}

		public static void SetParentTransformIdentityLocalTransforms(this GameObject o, Transform parent)
		{
			o.transform.parent = parent;
			o.transform.localPosition = Vector3.zero;
			o.transform.localRotation = Quaternion.identity;
			o.transform.localScale = Vector3.one;
		}

		public static void SetParentTransformIdentityLocalTransforms(this GameObject o, GameObject parent)
		{
			o.SetParentTransformIdentityLocalTransforms(parent.transform);
		}

		public static void SetLayerInChildren(this GameObject o, int layer)
		{
			o.layer = layer;
			foreach (Transform item in o.transform)
			{
				item.gameObject.SetLayerInChildren(layer);
			}
		}

		public static void DoDestroyOnLoad(this GameObject o)
		{
			o.AddComponent<DoDestroyOnLoad>();
		}

		public static GameObject GetChildGameObject(this GameObject o, string name)
		{
			if (o == null)
			{
				return null;
			}
			if (string.Compare(o.name, name, true) == 0)
			{
				return o;
			}
			foreach (Transform item in o.transform)
			{
				GameObject childGameObject = item.gameObject.GetChildGameObject(name);
				if (childGameObject != null)
				{
					return childGameObject;
				}
			}
			return null;
		}

		public static T GetInterfaceComponent<T>(this GameObject o) where T : class
		{
			return o.GetComponent(typeof(T)) as T;
		}

		public static List<T> FindObjectsOfInterface<T>(this GameObject o) where T : class
		{
			UnityEngine.Object[] source = UnityEngine.Object.FindObjectsOfType(typeof(MonoBehaviour));
			return (from MonoBehaviour behaviour in source
				select behaviour.GetComponent(typeof(T))).OfType<T>().ToList();
		}

		public static void SendGameEvent(this GameObject gameObject, Enum eventType, params object[] parameters)
		{
			GameEventRouter.SendEvent(eventType, gameObject, parameters);
		}

		public static T GetComponentInParents<T>(this GameObject gObject) where T : Component
		{
			Transform transform = gObject.transform;
			while (transform != null)
			{
				T component = transform.GetComponent<T>();
				if (component != null)
				{
					return component;
				}
				transform = transform.parent;
			}
			return (T)null;
		}

		public static T GetOrAddComponent<T>(this GameObject gameObj) where T : Component
		{
			T val = gameObj.GetComponent<T>();
			if (val == null)
			{
				val = gameObj.AddComponent<T>();
			}
			return val;
		}

		public static GameObject FindWithInactive(string name)
		{
			bool flag = name.StartsWith("/");
			string[] array = name.Split('/');
			GameObject gameObject = null;
			for (int i = (flag ? 1 : 0); i < array.Length; i++)
			{
				string part = array[i];
				if (!gameObject)
				{
					gameObject = GameObject.Find("/" + part);
					continue;
				}
				List<Transform> source = gameObject.transform.Cast<Transform>().ToList();
				IEnumerable<Transform> enumerable = source.Where((Transform t) => t.gameObject.name == part);
				IList<Transform> source2 = (enumerable as IList<Transform>) ?? enumerable.ToList();
				if (!source2.Any())
				{
					return null;
				}
				gameObject = source2.Select((Transform t) => t.gameObject).First();
			}
			return gameObject;
		}
	}
}
