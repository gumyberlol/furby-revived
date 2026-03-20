using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("Fills the screen with a Color. NOTE: Uses OnGUI so you need a PlayMakerGUI component in the scene.")]
	public class DrawFullscreenColor : FsmStateAction
	{
		[RequiredField]
		public FsmColor color;

		public override void Reset()
		{
			color = Color.white;
		}
	}
}
