using Relentless;
using UnityEngine;

namespace Furby.Neighbourhood
{
	public class FabricateFurblings : RelentlessMonoBehaviour
	{
		private void OnClick()
		{
			FurbyGlobals.BabyRepositoryHelpers.TEST_MakeOneOfEachBaby(FurbyBabyProgresss.N, true);
			Application.LoadLevel(Application.loadedLevelName);
		}
	}
}
