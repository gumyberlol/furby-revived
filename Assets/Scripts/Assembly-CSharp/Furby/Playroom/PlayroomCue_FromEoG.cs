using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class PlayroomCue_FromEoG : RelentlessMonoBehaviour
	{
		private void OnClick()
		{
			if (GetComponent<SpsSwitchAction>() != null)
			{
				Logging.LogError("BIG ERROR. There is a switch action on this button which shouldn't be here. It will cause many problems (double loading of scene).");
			}
			Logging.Log("PlayroomCue_FromEoG::OnCLick");
			GameObject gameObject = new GameObject("SENTINEL_TransitionToHood");
			gameObject.AddComponent<PlayroomStartup_TransitionToHood>();
			Object.DontDestroyOnLoad(gameObject);
			FurbyGlobals.ScreenSwitcher.SwitchScreen("Playroom");
		}
	}
}
