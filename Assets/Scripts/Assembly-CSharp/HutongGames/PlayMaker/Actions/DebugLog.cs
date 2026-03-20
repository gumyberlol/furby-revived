namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Sends a log message to the PlayMaker Log Window.")]
	public class DebugLog : FsmStateAction
	{
		public LogLevel logLevel;

		public FsmString text;

		public override void Reset()
		{
			logLevel = LogLevel.Info;
			text = string.Empty;
		}

		public override void OnEnter()
		{
			if (!string.IsNullOrEmpty(text.Value))
			{
				ActionHelpers.DebugLog(base.Fsm, logLevel, text.Value);
			}
			Finish();
		}
	}
}
