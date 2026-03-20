using Relentless;
using UnityEngine;

namespace Furby.MiniGames.HideAndSeek
{
	public class HideAndSeekUtlity : Singleton<HideAndSeekUtlity>
	{
		public GameObject m_FurbyBaby;

		public GameObject FurbyBaby
		{
			get
			{
				return m_FurbyBaby;
			}
		}

		public GameObject LastHitObject { get; set; }

		public GameObject GetBabyHidingObject()
		{
			return m_FurbyBaby.GetComponent<HideFurby>().CurrentHideObject;
		}

		public GameObject GetRandomHideObject(GameObject objectToAvoid)
		{
			GameObject currentHideObject = m_FurbyBaby.GetComponent<HideFurby>().CurrentHideObject;
			if (currentHideObject == null)
			{
				return null;
			}
			GameObject gameObject = currentHideObject.transform.parent.gameObject;
			ObjectHitEvent[] componentsInChildren = gameObject.GetComponentsInChildren<ObjectHitEvent>();
			int num = componentsInChildren.Length;
			if (num <= 3)
			{
				return null;
			}
			int num2 = ((objectToAvoid == null) ? 1 : 2);
			ObjectHitEvent[] array = new ObjectHitEvent[num - num2];
			int num3 = 0;
			for (int i = 0; i < num; i++)
			{
				if (!(componentsInChildren[i].gameObject == objectToAvoid) && !(componentsInChildren[i].gameObject == currentHideObject))
				{
					array[num3] = componentsInChildren[i];
					num3++;
				}
			}
			int num4 = Random.Range(0, array.Length - 1);
			return array[num4].gameObject;
		}
	}
}
