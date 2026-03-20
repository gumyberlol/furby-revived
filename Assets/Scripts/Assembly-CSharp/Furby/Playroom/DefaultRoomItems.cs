using System.Collections;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class DefaultRoomItems : RelentlessMonoBehaviour
	{
		[SerializeField]
		private PlayroomItemList m_GenericDefaultItems;

		[SerializeField]
		private PlayroomItemList m_GoldenDefaultItems;

		[SerializeField]
		private PlayroomItemList m_CrystalDefaultItems;

		[SerializeField]
		private PlayroomItemList m_GoldenCrystalDefaultItems;

		[SerializeField]
		[HideInInspector]
		public PlayroomItemList m_CurrentItems;

		[SerializeField]
		public SelectableLists m_Lists;

		[SerializeField]
		public TargetRoots m_TargetRoots;

		public IEnumerator Start()
		{
			yield return null;
			BabyInstance babyInstance = (BabyInstance)Object.FindObjectOfType(typeof(BabyInstance));
			if (babyInstance != null)
			{
				FurbyBaby furbyBaby = babyInstance.GetTargetFurbyBaby();
				if (furbyBaby != null)
				{
					SpecifyFeatureFromPreferencesOrDefaults(furbyBaby, m_Lists.m_LightFixtures, furbyBaby.PlayroomCustomizations.LightFixture, m_GoldenCrystalDefaultItems.m_LightFixture, m_CrystalDefaultItems.m_LightFixture, m_GoldenDefaultItems.m_LightFixture, m_GenericDefaultItems.m_LightFixture, m_CurrentItems.m_LightFixture);
					SpecifyFeatureFromPreferencesOrDefaults(furbyBaby, m_Lists.m_Rugs, furbyBaby.PlayroomCustomizations.Rug, m_GoldenCrystalDefaultItems.m_Rug, m_CrystalDefaultItems.m_Rug, m_GoldenDefaultItems.m_Rug, m_GenericDefaultItems.m_Rug, m_CurrentItems.m_Rug);
					SpecifyFeatureFromPreferencesOrDefaults(furbyBaby, m_Lists.m_Props, furbyBaby.PlayroomCustomizations.Prop, m_GoldenCrystalDefaultItems.m_Prop, m_CrystalDefaultItems.m_Prop, m_GoldenDefaultItems.m_Prop, m_GenericDefaultItems.m_Prop, m_CurrentItems.m_Prop);
					SpecifyFeatureFromPreferencesOrDefaults(furbyBaby, m_Lists.m_Wallart, furbyBaby.PlayroomCustomizations.WallArt, m_GoldenCrystalDefaultItems.m_WallArt, m_CrystalDefaultItems.m_WallArt, m_GoldenDefaultItems.m_WallArt, m_GenericDefaultItems.m_WallArt, m_CurrentItems.m_WallArt);
					SpecifyThemeFromPreferencesOrDefaults(furbyBaby, m_Lists.m_Themes, furbyBaby.PlayroomCustomizations.Theme, m_GoldenCrystalDefaultItems.m_Theme, m_CrystalDefaultItems.m_Theme, m_GoldenDefaultItems.m_Theme, m_GenericDefaultItems.m_Theme, m_CurrentItems.m_Theme);
				}
			}
			yield return StartCoroutine(LoadPlayroomItem(m_CurrentItems.m_LightFixture, m_TargetRoots.m_LightRoot));
			yield return StartCoroutine(LoadPlayroomItem(m_CurrentItems.m_WallArt, m_TargetRoots.m_WallArtRoot));
			yield return StartCoroutine(LoadPlayroomItem(m_CurrentItems.m_Prop, m_TargetRoots.m_PropRoot));
			yield return StartCoroutine(LoadPlayroomItem(m_CurrentItems.m_Rug, m_TargetRoots.m_RugRoot));
			m_CurrentItems.m_Theme.ReplaceTextures();
		}

		private void SpecifyFeatureFromPreferencesOrDefaults(FurbyBaby furbyBaby, SelectableFeatureList featureList, string previouslyAppliedPlayroomItemName, PlayroomItem goldenCrystal, PlayroomItem crystal, PlayroomItem golden, PlayroomItem generic, PlayroomItem current)
		{
			if (previouslyAppliedPlayroomItemName == string.Empty)
			{
				switch (furbyBaby.Tribe.TribeSet)
				{
				case Tribeset.Golden:
					current.m_AssetBundleName = golden.m_AssetBundleName;
					break;
				case Tribeset.CrystalGem:
					current.m_AssetBundleName = crystal.m_AssetBundleName;
					break;
				case Tribeset.CrystalGolden:
					current.m_AssetBundleName = goldenCrystal.m_AssetBundleName;
					break;
				default:
					current.m_AssetBundleName = generic.m_AssetBundleName;
					break;
				}
			}
			else
			{
				SelectableFeature selectableFeature = featureList.Features.Where((SelectableFeature feature) => feature.PlayroomFeature.m_Name == previouslyAppliedPlayroomItemName).FirstOrDefault();
				if (selectableFeature != null)
				{
					current.m_AssetBundleName = selectableFeature.PlayroomFeature.m_ObjectName;
				}
			}
		}

		private void SpecifyThemeFromPreferencesOrDefaults(FurbyBaby furbyBaby, SelectableThemeList themeList, string previouslyAppliedPlayroomThemeName, PlayroomTheme goldenCrystal, PlayroomTheme crystal, PlayroomTheme golden, PlayroomTheme generic, PlayroomTheme current)
		{
			if (previouslyAppliedPlayroomThemeName == string.Empty)
			{
				switch (furbyBaby.Tribe.TribeSet)
				{
				case Tribeset.Golden:
					current.m_InteriorTexture = golden.m_InteriorTexture;
					current.m_WallTexture = golden.m_WallTexture;
					current.m_InteriorMaterial = golden.m_InteriorMaterial;
					current.m_WallMaterial = golden.m_WallMaterial;
					break;
				case Tribeset.CrystalGem:
					current.m_InteriorTexture = crystal.m_InteriorTexture;
					current.m_WallTexture = crystal.m_WallTexture;
					current.m_InteriorMaterial = crystal.m_InteriorMaterial;
					current.m_WallMaterial = crystal.m_WallMaterial;
					break;
				case Tribeset.CrystalGolden:
					current.m_InteriorTexture = goldenCrystal.m_InteriorTexture;
					current.m_WallTexture = goldenCrystal.m_WallTexture;
					current.m_InteriorMaterial = goldenCrystal.m_InteriorMaterial;
					current.m_WallMaterial = goldenCrystal.m_WallMaterial;
					break;
				default:
					current.m_InteriorTexture = generic.m_InteriorTexture;
					current.m_WallTexture = generic.m_WallTexture;
					current.m_InteriorMaterial = generic.m_InteriorMaterial;
					current.m_WallMaterial = generic.m_WallMaterial;
					break;
				}
			}
			else
			{
				SelectableTheme selectableTheme = themeList.Themes.Where((SelectableTheme theme) => theme.PlayroomThemeData.m_Name == previouslyAppliedPlayroomThemeName).FirstOrDefault();
				if (selectableTheme != null)
				{
					current.m_InteriorTexture = selectableTheme.PlayroomThemeData.m_InteriorTexture;
					current.m_WallTexture = selectableTheme.PlayroomThemeData.m_WallTexture;
					current.m_InteriorMaterial = selectableTheme.PlayroomThemeData.m_InteriorMaterial;
					current.m_WallMaterial = selectableTheme.PlayroomThemeData.m_WallMaterial;
				}
			}
		}

		private IEnumerator LoadPlayroomItem(PlayroomItem item, GameObject targetRoot)
		{
			AssetBundleHelpers.AssetBundleLoad itemResult = new AssetBundleHelpers.AssetBundleLoad();
			yield return StartCoroutine(AssetBundleHelpers.Load("Playroom/" + item.m_AssetBundleName, true, itemResult, base.gameObject, typeof(GameObject), true));
			Logging.Log(itemResult.m_object);
			item.InstanceItemIntoScene((GameObject)itemResult.m_object, targetRoot);
		}
	}
}
