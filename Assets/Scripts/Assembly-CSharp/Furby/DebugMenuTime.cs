using Relentless;
using UnityEngine;

namespace Furby
{
	public class DebugMenuTime : RelentlessMonoBehaviour
	{
		private void Update()
		{
			if (!(SingletonInstance<SystemTimer>.Instance == null))
			{
				UILabel componentInChildren = GetComponentInChildren<UILabel>();
				if (componentInChildren != null)
				{
					float timePlayedEver = SingletonInstance<SystemTimer>.Instance.GetTimePlayedEver();
					int num = (int)Mathf.Floor(SingletonInstance<SystemTimer>.Instance.GetTimePlayedEver() / 60f);
					int num2 = (int)Mathf.Floor(timePlayedEver - (float)num * 60f);
					int num3 = (int)Mathf.Floor((float)num / 60f);
					int num4 = num - num3 * 60;
					componentInChildren.text = string.Format("Time played: {0}:{1:00}:{2:00}", num3, num4, num2);
				}
			}
		}
	}
}
