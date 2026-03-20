using System.Linq;
using UnityEngine;

namespace Furby
{
	public class ShowBabyDNA : MonoBehaviour
	{
		[SerializeField]
		private bool m_useInProgressBaby;

		private FurbyBaby m_overrideTargetBaby;

		public void SetTargetBaby(FurbyBaby target)
		{
			m_overrideTargetBaby = target;
		}

		private void OnEnable()
		{
			Display();
		}

		private void Display()
		{
			Transform[] array = base.transform.Cast<Transform>().ToArray();
			Transform[] array2 = array;
			foreach (Transform transform in array2)
			{
				transform.gameObject.SetActive(false);
			}
			FurbyBaby furbyBaby = m_overrideTargetBaby;
			if (furbyBaby == null)
			{
				furbyBaby = ((!m_useInProgressBaby) ? FurbyGlobals.Player.SelectedFurbyBaby : FurbyGlobals.Player.InProgressFurbyBaby);
			}
			if (furbyBaby == null || furbyBaby.Progress == FurbyBabyProgresss.E)
			{
				return;
			}
			int num = 0;
			for (int num2 = 1; num2 <= 16; num2 *= 2)
			{
				if (((uint)furbyBaby.Personality & (uint)num2) != 0)
				{
					array[num].gameObject.SetActive(true);
					UISprite component = array[num].GetComponent<UISprite>();
					if (component != null)
					{
						component.spriteName = string.Format("VFX_IconImprint_{0}", (FurbyBabyPersonality)num2);
					}
					num++;
					if (num >= array.Length)
					{
						break;
					}
				}
			}
		}
	}
}
