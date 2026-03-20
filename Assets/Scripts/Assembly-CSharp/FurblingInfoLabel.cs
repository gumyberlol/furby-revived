using Furby;
using Relentless;
using UnityEngine;

public class FurblingInfoLabel : MonoBehaviour
{
	public enum FurblingInfoType
	{
		DateOfBirth = 0,
		FoodLikes = 1,
		FoodDislikes = 2,
		StyleLikes = 3,
		StyleDislikes = 4
	}

	public FurblingInfoType infoType;

	private FurMailMessageContents furmail;

	private void OnEnable()
	{
		UpdateUI();
	}

	private void Start()
	{
		UpdateUI();
	}

	private void UpdateUI()
	{
		UILabel component = GetComponent<UILabel>();
		if (!(component == null))
		{
			furmail = base.gameObject.GetOrAddComponent<FurMailMessageContents>();
			FurbyBaby selectedFurbyBaby = FurbyGlobals.Player.SelectedFurbyBaby;
			switch (infoType)
			{
			case FurblingInfoType.DateOfBirth:
				component.text = furmail.GetReceivedTimeText(selectedFurbyBaby.LastIncubationTime);
				break;
			case FurblingInfoType.FoodLikes:
				component.text = selectedFurbyBaby.FoodLikesString;
				break;
			case FurblingInfoType.FoodDislikes:
				component.text = selectedFurbyBaby.FoodDislikesString;
				break;
			case FurblingInfoType.StyleLikes:
				component.text = selectedFurbyBaby.StyleLikesString;
				break;
			case FurblingInfoType.StyleDislikes:
				component.text = selectedFurbyBaby.StyleDislikesString;
				break;
			}
		}
	}
}
