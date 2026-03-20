using Relentless;
using UnityEngine;

namespace Furby.Neighbourhood
{
	public class ClearFurblings : RelentlessMonoBehaviour
	{
		private void OnClick()
		{
			FurbyGlobals.BabyRepositoryHelpers.TEST_MakeBabies(6);
			Application.LoadLevel(Application.loadedLevelName);
		}
	}
}
