using UnityEngine;

namespace Relentless
{
	public class GUIScreen : RelentlessMonoBehaviour
	{
		public PrefabGroupLayoutAsset GuiScreenLayout;

		[HideInInspector]
		public GUIScreenManager ScreenManager { get; set; }

		public void Build()
		{
			GuiScreenLayout.Build(base.transform);
			if (Application.isPlaying)
			{
				base.gameObject.BroadcastMessage("SetGUIManager", ScreenManager, SendMessageOptions.DontRequireReceiver);
				IScreenViewModel screenViewModel = GetComponent(typeof(IScreenViewModel)) as IScreenViewModel;
				if (screenViewModel != null)
				{
					screenViewModel.OnShow();
				}
			}
		}

		public void Unbuild()
		{
			GuiScreenLayout.Unbuild(base.transform);
			if (Application.isPlaying)
			{
				IScreenViewModel screenViewModel = GetComponent(typeof(IScreenViewModel)) as IScreenViewModel;
				if (screenViewModel != null)
				{
					screenViewModel.OnExit();
				}
			}
		}
	}
}
