using Furby;
using HutongGames.PlayMaker;
using Relentless;

[ActionCategory(ActionCategory.Audio)]
[Tooltip("Sends an Audio Watermarking Tone to Furby")]
public class PlayWatermark : FsmStateAction
{
	[Tooltip("Watermark Code")]
	public int m_WatermarkCode;

	public override void Reset()
	{
		m_WatermarkCode = 0;
	}

	public override void OnEnter()
	{
		Singleton<FurbyDataChannel>.Instance.PostAction((FurbyAction)m_WatermarkCode, null);
		Finish();
	}
}
