namespace Relentless
{
	public class GuiNamedTextAction : RelentlessMonoBehaviour
	{
		private void OnClick()
		{
			NGUILocaliser component = GetComponent<NGUILocaliser>();
			if ((bool)component)
			{
				SendMessageUpwards("NamedTextEvent", component.LocalisedStringKey);
			}
			else
			{
				Logging.LogError("Failed to find NGUILocaliser component");
			}
		}
	}
}
