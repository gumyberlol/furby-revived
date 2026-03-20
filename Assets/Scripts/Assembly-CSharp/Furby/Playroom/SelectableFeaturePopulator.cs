using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class SelectableFeaturePopulator : MonoBehaviour
	{
		public GameObject m_CarouselItemPrefab;

		public GameObject m_TargetRoot;

		public bool m_IsTop;

		[SerializeField]
		public SelectableFeatureList m_FeatureList;

		public void Start()
		{
			UIGrid component = GetComponent<UIGrid>();
			if (m_FeatureList == null)
			{
				m_FeatureList = ScriptableObject.CreateInstance<SelectableFeatureList>();
			}
			bool flag = SelectableHelpers.HaveGoldenBaby();
			bool flag2 = SelectableHelpers.HaveGoldenCrystalBaby();
			int num = 0;
			foreach (SelectableFeature feature in m_FeatureList.Features)
			{
				if ((feature.IsGoldenItem() && !flag) || (feature.IsGoldenCrystalItem() && !flag2) || (feature.IsUnlockedBySeason() && !feature.m_PlayroomFeature.m_ThemePeriod.IsUnlockedNow()) || (feature.IsUnlockedByCrystal() && !Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForCrystal) || (feature.IsGoldenItemOrComAirTone() && !Singleton<GameDataStoreObject>.Instance.Data.RecognizedComAirTones.Contains(feature.m_PlayroomFeature.m_ComAirTone) && !flag))
				{
					continue;
				}
				bool flag3 = Singleton<GameDataStoreObject>.Instance.Data.m_purchasedItems.Contains("SelectableFeature_" + feature.m_PlayroomFeature.m_Name);
				if ((feature.IsUnlockedByScannedQRCode() && !Singleton<GameDataStoreObject>.Instance.Data.QRItemsUnlocked.Contains(feature.m_PlayroomFeature.m_UnlockCode + feature.m_PlayroomFeature.m_VariantCode) && !flag3) || (feature.IsUnlockedByComAirTone() && !Singleton<GameDataStoreObject>.Instance.Data.RecognizedComAirTones.Contains(feature.m_PlayroomFeature.m_ComAirTone) && !flag3))
				{
					continue;
				}
				if (feature.IsUnlockedAsGift())
				{
					string giftName = feature.GetName();
					if (!Singleton<GameDataStoreObject>.Instance.Data.HaveOpenedGift_ByName(giftName))
					{
						continue;
					}
				}
				GameObject gameObject = Object.Instantiate(m_CarouselItemPrefab) as GameObject;
				gameObject.transform.parent = component.transform;
				GameObject gameObject2 = gameObject.transform.Find("TextureFore").gameObject;
				UISprite component2 = gameObject2.GetComponent<UISprite>();
				component2.atlas = feature.PlayroomFeature.m_UIAtlas;
				component2.spriteName = feature.PlayroomFeature.m_SpriteName;
				component2.pivot = UIWidget.Pivot.Center;
				component2.MakePixelPerfect();
				gameObject.transform.parent = component.transform;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, -5f);
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				PlayroomFeatureSelect playroomFeatureSelect = (PlayroomFeatureSelect)gameObject.GetComponent("PlayroomFeatureSelect");
				playroomFeatureSelect.SetFeatureData(feature.PlayroomFeature);
				playroomFeatureSelect.m_TargetAssetBundle = feature.PlayroomFeature.m_ObjectName;
				playroomFeatureSelect.m_TargetRoot = m_TargetRoot;
				playroomFeatureSelect.m_IsTop = m_IsTop;
				num = (component2.depth = num + 1);
				gameObject.name = feature.PlayroomFeature.m_State.ToString() + feature.PlayroomFeature.m_Cost + "_" + feature.PlayroomFeature.m_Name.ToString();
			}
			component.Reposition();
		}
	}
}
