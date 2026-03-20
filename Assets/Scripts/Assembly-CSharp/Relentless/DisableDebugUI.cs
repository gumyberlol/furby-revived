namespace Relentless
{
	public class DisableDebugUI : RelentlessMonoBehaviour
	{
		private void OnEnable()
		{
			if (SingletonInstance<ApplicationSettingsBehaviour>.Instance == null)
			{
				Logging.LogError("DisableDebugUI: Invalid ApplicationSettings - Instance is null");
			}
			else if (SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings == null)
			{
				Logging.LogError("DisableDebugUI: Invalid ApplicationSettings - ApplicationSettings is null");
			}
			else if (SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.friendlyProfileName == ProfileFriendlyName.None)
			{
				Logging.LogError("DisableDebugUI: Invalid ApplicationSettings - No friendly name");
			}
			else if (SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.friendlyProfileName != ProfileFriendlyName.Developer)
			{
				base.gameObject.SetActive(false);
			}
		}
	}
}
