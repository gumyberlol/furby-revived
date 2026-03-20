using System.Collections.Generic;

namespace Relentless
{
	public class NGUILocaliser : RelentlessMonoBehaviour
	{
		public string LocalisedStringKey;

		public void OnEnable()
		{
			UpdateUI();
		}

		public void Start()
		{
			UpdateUI();
		}

		public void UpdateUI()
		{
			if (string.IsNullOrEmpty(LocalisedStringKey))
			{
				Logging.LogError(string.Format("Empty named text key : \"{0}\" on object {1}", LocalisedStringKey, base.name));
			}
			if (!Singleton<Localisation>.Exists)
			{
				return;
			}
			List<UILabel> list = new List<UILabel>();
			base.gameObject.GetComponentsInChildrenIncludeInactive(list);
			string text = Singleton<Localisation>.Instance.GetText(LocalisedStringKey);
			foreach (UILabel item in list)
			{
				if (!string.IsNullOrEmpty(text))
				{
					item.text = text;
				}
				else
				{
					item.text = "*" + LocalisedStringKey + "*";
				}
			}
		}
	}
}
