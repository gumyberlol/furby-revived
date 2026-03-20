namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("GUILayout Label for a Float Variable.")]
	[ActionCategory(ActionCategory.GUILayout)]
	public class GUILayoutFloatLabel : GUILayoutAction
	{
		[Tooltip("Text to put before the float variable.")]
		public FsmString prefix;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Float variable to display.")]
		public FsmFloat floatVariable;

		[Tooltip("Optional GUIStyle in the active GUISKin.")]
		public FsmString style;

		public override void Reset()
		{
			base.Reset();
			prefix = string.Empty;
			style = string.Empty;
			floatVariable = null;
		}
	}
}
