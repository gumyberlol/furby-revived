using System.Collections;
using Furby;
using Furby.Utilities.EggCarton;
using Relentless;
using UnityEngine;

public class TitlePageMicrophoneChecker : MonoBehaviour
{
	[SerializeField]
	private ErrorMessageBox m_ErrorBox;

	private static bool s_FakeMicrophoneDisabled;

	public static bool IsMicrophoneDisabled()
	{
		return s_FakeMicrophoneDisabled;
	}

	private IEnumerator Start()
	{
		m_ErrorBox.Hide();
		yield return null;
		if (IsMicrophoneDisabled())
		{
			float oldTimeScale = Time.timeScale;
			Time.timeScale = 0f;
			m_ErrorBox.SetOKState("FURBYCOMMSERROR_MICROPHONE", "MENU_OPTION_OK", CartonGameEvent.EggDialogGenericAccept);
			m_ErrorBox.Show(true);
			WaitForGameEvent waiter = new WaitForGameEvent();
			yield return StartCoroutine(waiter.WaitForEvent(CartonGameEvent.EggDialogGenericAccept));
			m_ErrorBox.Hide();
			Time.timeScale = oldTimeScale;
		}
	}
}
