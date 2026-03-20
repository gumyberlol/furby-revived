namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("GUILayout Label for an Int Variable.")]
	[ActionCategory(ActionCategory.GUILayout)]
	public class GUILayoutIntLabel : GUILayoutAction
	{
		[Tooltip("Text to put before the int variable.")]
		public FsmString prefix;

		[Tooltip("Int variable to display.")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmInt intVariable;

		[Tooltip("Optional GUIStyle in the active GUISKin.")]
		public FsmString style;

		public override void Reset()
		{
			base.Reset();
			prefix = string.Empty;
			style = string.Empty;
			intVariable = null;
		}
	}
}
