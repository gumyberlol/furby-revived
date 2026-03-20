using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class LevelProcessReplaceAudioWithGameEvent : LevelProcess
	{
		public override bool Process()
		{
			bool flag = false;
			HashSet<GameObject> hashSet = new HashSet<GameObject>();
			FindAndAdd<UIButton>(hashSet);
			FindAndAdd<UIButtonScale>(hashSet);
			FindAndAdd<UIButtonOffset>(hashSet);
			FindAndAdd<UIButtonSound>(hashSet);
			foreach (GameObject item in hashSet)
			{
				flag |= UpgradeButton(item);
			}
			return flag;
		}

		private bool UpgradeButton(GameObject goIn)
		{
			bool flag = false;
			flag |= FindAndUpgrade<UIButton>(goIn);
			flag |= FindAndUpgrade<UIButtonScale>(goIn);
			flag |= FindAndUpgrade<UIButtonOffset>(goIn);
			return flag | FindAndUpgrade<UIButtonSound>(goIn);
		}

		private bool FindAndUpgrade<T>(GameObject go) where T : MonoBehaviour
		{
			bool flag = false;
			T[] componentsInChildren = go.GetComponentsInChildren<T>(true);
			T[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				T val = array[i];
				flag |= UpgradeSub(val.gameObject);
			}
			return flag;
		}

		private bool UpgradeSub(GameObject go)
		{
			return false;
		}

		private void FindAndAdd<T>(HashSet<GameObject> hashSet) where T : MonoBehaviour
		{
			Object[] array = Object.FindObjectsOfType(typeof(T));
			Object[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				T val = (T)array2[i];
				hashSet.Add(val.gameObject);
			}
			Object[] array3 = Object.FindObjectsOfType(typeof(UICamera));
			Object[] array4 = array3;
			for (int j = 0; j < array4.Length; j++)
			{
				UICamera uICamera = (UICamera)array4[j];
				T[] componentsInChildren = uICamera.gameObject.GetComponentsInChildren<T>(true);
				T[] array5 = componentsInChildren;
				for (int k = 0; k < array5.Length; k++)
				{
					T val2 = array5[k];
					hashSet.Add(val2.gameObject);
				}
			}
		}
	}
}
