namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout Box.")]
	public class GUILayoutBox : GUILayoutAction
	{
		[Tooltip("Image to display in the Box.")]
		public FsmTexture image;

		[Tooltip("Text to display in the Box.")]
		public FsmString text;

		[Tooltip("Optional Tooltip string.")]
		public FsmString tooltip;

		[Tooltip("Optional GUIStyle in the active GUISkin.")]
		public FsmString style;

		public override void Reset()
		{
			base.Reset();
			text = string.Empty;
			image = null;
			tooltip = string.Empty;
			style = string.Empty;
		}
	}
}
