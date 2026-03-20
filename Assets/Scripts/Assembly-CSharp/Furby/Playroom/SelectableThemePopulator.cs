using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class SelectableThemePopulator : MonoBehaviour
	{
		public GameObject m_CarouselItemPrefab;

		public bool m_IsTop;

		[SerializeField]
		private SelectableThemeList m_ThemeList;

		public void Start()
		{
			UIGrid component = GetComponent<UIGrid>();
			if (m_ThemeList == null)
			{
				m_ThemeList = ScriptableObject.CreateInstance<SelectableThemeList>();
			}
			bool flag = SelectableHelpers.HaveGoldenBaby();
			bool flag2 = SelectableHelpers.HaveGoldenCrystalBaby();
			int num = 0;
			foreach (SelectableTheme theme in m_ThemeList.Themes)
			{
				if ((theme.IsGoldenItem() && !flag) || (theme.IsGoldenCrystalItem() && !flag2) || (theme.IsUnlockedBySeason() && !theme.PlayroomThemeData.m_ThemePeriod.IsUnlockedNow()) || (theme.IsUnlockedByCrystal() && !Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForCrystal) || (theme.IsGoldenItemOrComAirTone() && !Singleton<GameDataStoreObject>.Instance.Data.RecognizedComAirTones.Contains(theme.PlayroomThemeData.m_ComAirTone) && !flag))
				{
					continue;
				}
				bool flag3 = Singleton<GameDataStoreObject>.Instance.Data.m_purchasedItems.Contains("SelectableFeature_" + theme.PlayroomThemeData.m_Name);
				if ((theme.IsUnlockedByScannedQRCode() && !Singleton<GameDataStoreObject>.Instance.Data.QRItemsUnlocked.Contains(theme.PlayroomThemeData.m_UnlockCode + theme.PlayroomThemeData.m_VariantCode) && !flag3) || (theme.IsUnlockedByComAirTone() && !Singleton<GameDataStoreObject>.Instance.Data.RecognizedComAirTones.Contains(theme.PlayroomThemeData.m_ComAirTone) && !flag3))
				{
					continue;
				}
				if (theme.IsUnlockedAsGift())
				{
					string giftName = theme.GetName();
					if (!Singleton<GameDataStoreObject>.Instance.Data.HaveOpenedGift_ByName(giftName))
					{
						continue;
					}
				}
				GameObject gameObject = Object.Instantiate(m_CarouselItemPrefab) as GameObject;
				gameObject.transform.parent = component.transform;
				gameObject.name = theme.m_PlayroomThemeData.m_Name;
				GameObject gameObject2 = gameObject.transform.Find("TextureFore").gameObject;
				UISprite component2 = gameObject2.GetComponent<UISprite>();
				component2.atlas = theme.m_PlayroomThemeData.m_UIAtlas;
				component2.spriteName = theme.m_PlayroomThemeData.m_SpriteName;
				component2.pivot = UIWidget.Pivot.Center;
				component2.MakePixelPerfect();
				gameObject.transform.parent = component.transform;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, -5f);
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				PlayroomThemeSelect playroomThemeSelect = (PlayroomThemeSelect)gameObject.GetComponent("PlayroomThemeSelect");
				playroomThemeSelect.SetThemeData(theme.m_PlayroomThemeData);
				playroomThemeSelect.m_IsTop = m_IsTop;
				num = (component2.depth = num + 1);
				gameObject.name = theme.PlayroomThemeData.m_State.ToString() + theme.PlayroomThemeData.m_cost + "_" + theme.PlayroomThemeData.m_Name.ToString();
			}
			component.Reposition();
		}
	}
}
