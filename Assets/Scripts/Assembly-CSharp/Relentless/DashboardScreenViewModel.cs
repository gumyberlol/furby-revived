using Furby;
using UnityEngine;

namespace Relentless
{
	public class DashboardScreenViewModel : RelentlessMonoBehaviour, IScreenViewModel
	{
		public void OnShow()
		{
			SetChildLabelText("FurbyNameLabel", FurbyGlobals.Player.FullName);
			FurbyBaby inProgressFurbyBaby = FurbyGlobals.Player.InProgressFurbyBaby;
			SetChildLabelText("BabyNameLabel", inProgressFurbyBaby.Name);
		}

		public void OnExit()
		{
		}

		public void OnHide()
		{
		}

		private void SetChildLabelText(string gameObjectName, string newText)
		{
			UILabel labelOn = GetLabelOn(gameObjectName);
			if (!(labelOn == null))
			{
				labelOn.text = newText;
			}
		}

		private UILabel GetLabelOn(string nameOfGameObject)
		{
			GameObject childGameObject = base.gameObject.GetChildGameObject(nameOfGameObject);
			if (childGameObject == null)
			{
				Logging.LogError("Failed to get game object " + nameOfGameObject);
				return null;
			}
			UILabel component = childGameObject.GetComponent<UILabel>();
			if (component == null)
			{
				Logging.LogError("Failed to get UILabel component on game object " + nameOfGameObject);
				return null;
			}
			return component;
		}
	}
}
