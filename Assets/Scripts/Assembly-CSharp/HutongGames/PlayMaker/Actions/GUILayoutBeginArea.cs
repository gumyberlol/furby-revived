using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Begin a GUILayout block of GUI controls in a fixed screen area. NOTE: Block must end with a corresponding GUILayoutEndArea.")]
	[ActionCategory(ActionCategory.GUILayout)]
	public class GUILayoutBeginArea : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmRect screenRect;

		public FsmFloat left;

		public FsmFloat top;

		public FsmFloat width;

		public FsmFloat height;

		public FsmBool normalized;

		public FsmString style;

		private Rect rect;

		public override void Reset()
		{
			screenRect = null;
			left = 0f;
			top = 0f;
			width = 1f;
			height = 1f;
			normalized = true;
			style = string.Empty;
		}
	}
}
