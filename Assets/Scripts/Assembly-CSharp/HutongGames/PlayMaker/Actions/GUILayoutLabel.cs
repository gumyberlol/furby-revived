namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout Label.")]
	public class GUILayoutLabel : GUILayoutAction
	{
		public FsmTexture image;

		public FsmString text;

		public FsmString tooltip;

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
