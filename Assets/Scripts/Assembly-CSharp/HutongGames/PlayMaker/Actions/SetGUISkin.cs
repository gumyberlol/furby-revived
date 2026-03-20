using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("Sets the GUISkin used by GUI elements.")]
	public class SetGUISkin : FsmStateAction
	{
		[RequiredField]
		public GUISkin skin;

		public FsmBool applyGlobally;

		public override void Reset()
		{
			skin = null;
			applyGlobally = true;
		}
	}
}
