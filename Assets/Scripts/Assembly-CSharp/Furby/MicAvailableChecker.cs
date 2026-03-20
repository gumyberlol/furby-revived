using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class MicAvailableChecker : ScriptableObject
	{
		public delegate void ResultHandler(bool isMicAvailable);

		[SerializeField]
		private CodeControlledDialogBox m_errorDialog;

		[SerializeField]
		private bool m_forceUnavailableForDebugging;

		[SerializeField]
		private float m_textScale = 0.5f;

		public IEnumerator Check(Transform errorSpawnPoint, ResultHandler handler)
		{
			bool available = !TitlePageMicrophoneChecker.IsMicrophoneDisabled();
			if (!available)
			{
				IEnumerator i = ShowError(errorSpawnPoint);
				while (i.MoveNext())
				{
					yield return null;
				}
			}
			handler(available);
		}

		private IEnumerator ShowError(Transform spawnPoint)
		{
			CodeControlledDialogBox dialog = Object.Instantiate(m_errorDialog) as CodeControlledDialogBox;
			dialog.transform.parent = spawnPoint;
			dialog.transform.localPosition = Vector3.zero;
			dialog.transform.localScale = Vector3.one;
			dialog.gameObject.AddComponent<DisableOtherLayers>();
			Transform[] componentsInChildren = dialog.GetComponentsInChildren<Transform>();
			foreach (Transform t in componentsInChildren)
			{
				t.gameObject.layer = spawnPoint.gameObject.layer;
			}
			dialog.SetDialogType(CodeControlledDialogBox.DialogType.OneButton);
			dialog.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.DialogText, "ERROR_MICACCESS_FURBYBLOCK");
			dialog.DialogText.transform.localScale *= m_textScale;
			dialog.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.OkButton, "MENU_BUTTON_ACCEPT");
			dialog.SetGameEvent(CodeControlledDialogBox.WidgetIdentifier.OkButton, SettingsPageEvents.GenericDialogAccept);
			bool disableCamera = false;
			dialog.Show(disableCamera);
			WaitForGameEvent waiter = new WaitForGameEvent();
			IEnumerator i2 = waiter.WaitForEvent(SettingsPageEvents.GenericDialogAccept);
			while (i2.MoveNext())
			{
				yield return null;
			}
			dialog.Hide();
			Object.Destroy(dialog.gameObject);
		}
	}
}
