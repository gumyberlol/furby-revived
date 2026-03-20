using UnityEngine;

namespace Furby
{
	public class SetSelectedBabyToInProgress : MonoBehaviour
	{
		private void Awake()
		{
			FurbyGlobals.Player.SelectedFurbyBaby = FurbyGlobals.Player.InProgressFurbyBaby;
		}
	}
}
