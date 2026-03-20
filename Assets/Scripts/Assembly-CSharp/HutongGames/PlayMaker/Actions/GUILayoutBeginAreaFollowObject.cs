using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Begin a GUILayout area that follows the specified game object. Useful for overlays (e.g., playerName). NOTE: Block must end with a corresponding GUILayoutEndArea.")]
	[ActionCategory(ActionCategory.GUILayout)]
	public class GUILayoutBeginAreaFollowObject : FsmStateAction
	{
		[Tooltip("The GameObject to follow.")]
		[RequiredField]
		public FsmGameObject gameObject;

		[RequiredField]
		public FsmFloat offsetLeft;

		[RequiredField]
		public FsmFloat offsetTop;

		[RequiredField]
		public FsmFloat width;

		[RequiredField]
		public FsmFloat height;

		[Tooltip("Use normalized screen coordinates (0-1).")]
		public FsmBool normalized;

		[Tooltip("Optional named style in the current GUISkin")]
		public FsmString style;

		public override void Reset()
		{
			gameObject = null;
			offsetLeft = 0f;
			offsetTop = 0f;
			width = 1f;
			height = 1f;
			normalized = true;
			style = string.Empty;
		}

		private static void DummyBeginArea()
		{
			GUILayout.BeginArea(default(Rect));
		}
	}
}
