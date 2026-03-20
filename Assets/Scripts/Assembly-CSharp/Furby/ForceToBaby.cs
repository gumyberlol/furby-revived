using UnityEngine;

namespace Furby
{
	public class ForceToBaby : MonoBehaviour
	{
		private void Awake()
		{
			if (FurbyGlobals.Player.InProgressFurbyBaby != null && FurbyGlobals.Player.InProgressFurbyBaby.Progress == FurbyBabyProgresss.E)
			{
				FurbyGlobals.Player.InProgressFurbyBaby.Progress = FurbyBabyProgresss.P;
			}
		}
	}
}
