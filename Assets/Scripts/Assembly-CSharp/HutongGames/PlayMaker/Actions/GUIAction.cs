using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("GUI base action - don't use!")]
	public abstract class GUIAction : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmRect screenRect;

		public FsmFloat left;

		public FsmFloat top;

		public FsmFloat width;

		public FsmFloat height;

		[RequiredField]
		public FsmBool normalized;

		internal Rect rect;

		public override void Reset()
		{
			screenRect = null;
			left = 0f;
			top = 0f;
			width = 1f;
			height = 1f;
			normalized = true;
		}
	}
}
