using System.Collections;
using System.Collections.Generic;
using Furby.Neighbourhood;
using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class HoodStartup_FromEoG : RelentlessMonoBehaviour
	{
		private const string BLIMP_NAME = "Blimp_Gold_DoNotRename";

		private const string UFO_NAME = "Ufo_Gold_DoNotRename";

		public FurbyBaby m_TargetBaby;

		private GameObject m_CachedObject;

		private bool m_isCrystal;

		private void OnLevelWasLoaded()
		{
			if (!Application.loadedLevelName.ToLower().Contains("empty"))
			{
				StartCoroutine(RevealSequenceCoroutine());
			}
			HoodController[] array = (HoodController[])Object.FindObjectsOfType(typeof(HoodController));
			HoodController[] array2 = array;
			foreach (HoodController hoodController in array2)
			{
				hoodController.m_TheFurblingInFocus = m_TargetBaby;
				m_isCrystal = hoodController.m_NeighbourhoodTribeSet == Tribeset.CrystalGem || hoodController.m_NeighbourhoodTribeSet == Tribeset.CrystalGolden;
			}
		}

		private IEnumerator RevealSequenceCoroutine()
		{
			yield return null;
			CacheGoldenRoom((!m_isCrystal) ? "Blimp_Gold_DoNotRename" : "Ufo_Gold_DoNotRename");
			if (m_TargetBaby == null)
			{
				m_TargetBaby = FurbyGlobals.Player.InProgressFurbyBaby;
			}
			if (FurbyGlobals.BabyRepositoryHelpers.HaveCompletedTheGame(m_isCrystal) && ShouldTriggerTheEndSequence())
			{
				SetToStartPosition();
				if (m_TargetBaby.Tribe.TribeSet == Tribeset.CrystalGem || m_TargetBaby.Tribe.TribeSet == Tribeset.CrystalGolden)
				{
					m_CachedObject.SetActive(true);
				}
				yield return StartCoroutine(RevealSequence_TribeTower(VanDeliverySequenceType.VanDeliveryAndGoldenTower));
				yield return StartCoroutine(RevealSequence_CommonEnding());
				yield return new WaitForSeconds(10f);
				switch (m_TargetBaby.Tribe.TribeSet)
				{
				case Tribeset.MainTribes:
					yield return StartCoroutine(GoldenTowerSequence_ForMain());
					break;
				case Tribeset.Spring:
					yield return StartCoroutine(GoldenTowerSequence_ForSpring());
					break;
				}
				GameObject removalObject = GameObject.Find("RemovalVanPanel_DoNotRename");
				RemovalVanController removalVehicle = removalObject.GetComponent<RemovalVanController>();
				removalVehicle.ReEnableUI();
			}
			else
			{
				switch (m_TargetBaby.Tribe.TribeSet)
				{
				case Tribeset.MainTribes:
					m_CachedObject.SetActive(true);
					yield return StartCoroutine(RevealSequence_TribeTower(VanDeliverySequenceType.SimpleVanDelivery));
					break;
				case Tribeset.Spring:
					m_CachedObject.SetActive(false);
					yield return StartCoroutine(RevealSequence_TribeTower(VanDeliverySequenceType.SimpleVanDelivery));
					break;
				case Tribeset.CrystalGem:
					m_CachedObject.SetActive(true);
					yield return StartCoroutine(RevealSequence_TribeTower(VanDeliverySequenceType.SimpleVanDelivery));
					break;
				case Tribeset.Promo:
					yield return StartCoroutine(RevealSequence_Promo());
					break;
				case Tribeset.Golden:
					yield return StartCoroutine(RevealSequence_Golden());
					break;
				case Tribeset.CrystalGolden:
					m_CachedObject.SetActive(true);
					yield return StartCoroutine(RevealSequence_CrystalGolden());
					break;
				}
				yield return StartCoroutine(RevealSequence_CommonEnding());
			}
			SelfDestruct();
		}

		private void CacheGoldenRoom(string name)
		{
			m_CachedObject = GameObject.Find(name);
			if (m_CachedObject != null)
			{
				m_CachedObject.SetActive(false);
			}
		}

		private void SetToStartPosition()
		{
			if (m_isCrystal)
			{
				SetUfoToStartPosition();
			}
			else
			{
				SetBlimpToStartPosition();
			}
		}

		private void SetBlimpToStartPosition()
		{
			if (m_CachedObject == null)
			{
				Logging.Log("(SetBlimpToStartPosition returns early; no blimp in scene.)");
				return;
			}
			GameObject childGameObject = m_CachedObject.gameObject.GetChildGameObject("Blimp_StartPos");
			childGameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
			childGameObject.transform.localPosition = new Vector3(-550f, childGameObject.transform.localPosition.y, childGameObject.transform.localPosition.z);
		}

		private void SetUfoToStartPosition()
		{
		}

		public void DEBUG_InvokeSequenceMain()
		{
			CacheGoldenRoom("Blimp_Gold_DoNotRename");
			SetBlimpToStartPosition();
			StartCoroutine(GoldenTowerSequence_ForMain());
		}

		public void DEBUG_InvokeSequenceSpring()
		{
			CacheGoldenRoom("Blimp_Gold_DoNotRename");
			SetBlimpToStartPosition();
			StartCoroutine(GoldenTowerSequence_ForMain());
		}

		private IEnumerator RevealSequence_Promo()
		{
			RemovalVanController removalVehicle = GameObject.Find("RemovalVanPanel_DoNotRename").GetComponent<RemovalVanController>();
			removalVehicle.PrepareForRemoval(m_TargetBaby);
			yield return new WaitForSeconds(1f);
			removalVehicle.SendRemovalVanToCenter();
			GameEventRouter.SendEvent(HoodEvents.Hood_NewResident_BeingPlaced);
			GameEventRouter.SendEvent(HoodEvents.Hood_NewResident_Placed);
		}

		private IEnumerator RevealSequence_Golden()
		{
			m_CachedObject.SetActive(true);
			RemovalVanController removalVehicle = m_CachedObject.GetComponent<RemovalVanController>();
			removalVehicle.PrepareForRemoval(m_TargetBaby);
			yield return new WaitForSeconds(1f);
			removalVehicle.SendRemovalVanToTower(0, m_TargetBaby);
			GameEventRouter.SendEvent(HoodEvents.Hood_NewResident_BeingPlacedGold);
			GameEventRouter.SendEvent(HoodEvents.Hood_NewResident_Placed);
		}

		private IEnumerator RevealSequence_CrystalGolden()
		{
			m_CachedObject.SetActive(true);
			RemovalVanController removalVehicle = m_CachedObject.GetComponent<RemovalVanController>();
			removalVehicle.PrepareForRemoval(m_TargetBaby);
			CrystalGoldenAnimationIndirection indirection = m_CachedObject.GetComponent<CrystalGoldenAnimationIndirection>();
			indirection.Play();
			yield return new WaitForSeconds(1f);
			GameEventRouter.SendEvent(HoodEvents.Hood_NewResident_BeingPlacedGold);
			GameEventRouter.SendEvent(HoodEvents.Hood_NewResident_Placed);
		}

		private IEnumerator GoldenTowerSequence_ForSpring()
		{
			float durationSecs_1_BeforeStartingTowerAnims = 2f;
			float durationSecs_2_BeforeStoppingDustEmission = 4f;
			float durationSecs_3_BeforeStoppingTowerAnims = 3f;
			float durationSecs_CropDustingDuration_FlyBy = 8f;
			GameObject vfx = m_CachedObject.gameObject.GetChildGameObject("VFX_GoldenDust");
			TweenPosition tweener = m_CachedObject.gameObject.GetChildGameObject("Blimp_MoveAnim").GetComponent<TweenPosition>();
			tweener.from = new Vector3(0f, 0f, 0f);
			tweener.to = new Vector3(-1200f, 0f, 0f);
			tweener.duration = durationSecs_CropDustingDuration_FlyBy;
			tweener.method = UITweener.Method.Linear;
			tweener.style = UITweener.Style.Once;
			tweener.Play(true);
			m_CachedObject.SetActive(true);
			GameEventRouter.SendEvent(HoodEvents.Hood_BlimpCropDust_Begin);
			yield return new WaitForSeconds(durationSecs_1_BeforeStartingTowerAnims);
			GameEventRouter.SendEvent(HoodEvents.Hood_GoldenTowers_Begin);
			yield return new WaitForSeconds(durationSecs_2_BeforeStoppingDustEmission);
			GameEventRouter.SendEvent(HoodEvents.Hood_BlimpCropDust_End);
			vfx.GetComponent<ParticleSystem>().emissionRate = 0f;
			GameObject looseObjectInTheScene = new GameObject("vfx.transform");
			vfx.transform.parent = looseObjectInTheScene.transform;
			yield return new WaitForSeconds(durationSecs_3_BeforeStoppingTowerAnims);
			GameEventRouter.SendEvent(HoodEvents.Hood_GoldenTowers_End);
		}

		private IEnumerator GoldenTowerSequence_ForMain()
		{
			float durationSecs_1_BeforeStartingTowerAnims = 2f;
			float durationSecs_2_BeforeStoppingDustEmission = 4f;
			float durationSecs_3_BeforeStoppingTowerAnims = 3f;
			float durationSecs_CropDustingDuration_FlyBy = 8f;
			GameObject vfx = m_CachedObject.gameObject.GetChildGameObject("VFX_GoldenDust");
			TweenPosition tweener = m_CachedObject.gameObject.GetChildGameObject("Blimp_MoveAnim").GetComponent<TweenPosition>();
			tweener.from = new Vector3(0f, 0f, 0f);
			tweener.to = new Vector3(-1200f, 0f, 0f);
			tweener.duration = durationSecs_CropDustingDuration_FlyBy;
			tweener.method = UITweener.Method.Linear;
			tweener.style = UITweener.Style.Once;
			tweener.Play(true);
			m_CachedObject.SetActive(true);
			GameEventRouter.SendEvent(HoodEvents.Hood_BlimpCropDust_Begin);
			yield return new WaitForSeconds(durationSecs_1_BeforeStartingTowerAnims);
			GameEventRouter.SendEvent(HoodEvents.Hood_GoldenTowers_Begin);
			yield return new WaitForSeconds(durationSecs_2_BeforeStoppingDustEmission);
			GameEventRouter.SendEvent(HoodEvents.Hood_BlimpCropDust_End);
			vfx.GetComponent<ParticleSystem>().emissionRate = 0f;
			GameObject looseObjectInTheScene = new GameObject("vfx.transform");
			vfx.transform.parent = looseObjectInTheScene.transform;
			yield return new WaitForSeconds(durationSecs_3_BeforeStoppingTowerAnims);
			GameEventRouter.SendEvent(HoodEvents.Hood_GoldenTowers_End);
			float durationSecs_CropDustingDuration_ToMiddle = 5f;
			m_CachedObject.transform.localScale = new Vector3(-1f, 1f, 1f);
			tweener.Reset();
			tweener.from = new Vector3(0f, 0f, 0f);
			tweener.to = new Vector3(-550f, 0f, 0f);
			tweener.duration = durationSecs_CropDustingDuration_ToMiddle;
			tweener.method = UITweener.Method.EaseOut;
			tweener.style = UITweener.Style.Once;
			tweener.Play(true);
			yield return new WaitForSeconds(durationSecs_CropDustingDuration_ToMiddle);
			Object.Destroy(looseObjectInTheScene);
		}

		private IEnumerator RevealSequence_Gem()
		{
			yield return new WaitForSeconds(4f);
			GameEventRouter.SendEvent(HoodEvents.Hood_NewResident_BeingPlacedGold);
			GameEventRouter.SendEvent(HoodEvents.Hood_NewResident_Placed);
		}

		private IEnumerator RevealSequence_TribeTower(VanDeliverySequenceType sequenceType)
		{
			int towerIndex = 0;
			switch (m_TargetBaby.Tribe.name)
			{
			case "Wave-Stripe":
			case "SpringHeart-SpringDiamond":
			case "CrystalGreenToBlue-CrystalPinkToBlue":
				towerIndex = 0;
				break;
			case "Chev-Heart":
			case "SpringStar-SpringRainbow":
			case "CrystalOrangeToPink-CrystalRainbow":
				towerIndex = 1;
				break;
			case "Tri-Diag":
			case "SpringHound-SpringZigZag":
			case "CrystalPinkToPurple-CrystalYellowToOrange":
				towerIndex = 2;
				break;
			case "Pea-Cube":
				towerIndex = 3;
				break;
			case "Dot-Bolt":
				towerIndex = 4;
				break;
			case "Limit-Ed":
				towerIndex = 5;
				break;
			}
			GameObject o = GameObject.Find("RemovalVanPanel_DoNotRename");
			RemovalVanController removalVehicle = o.GetComponent<RemovalVanController>();
			removalVehicle.PrepareForRemoval(m_TargetBaby);
			if (!m_isCrystal && sequenceType == VanDeliverySequenceType.VanDeliveryAndGoldenTower)
			{
				removalVehicle.DontReenableUIOnCompletion();
			}
			yield return new WaitForSeconds(1f);
			removalVehicle.SendRemovalVanToTower(towerIndex, m_TargetBaby);
			if (!m_isCrystal && sequenceType == VanDeliverySequenceType.VanDeliveryAndGoldenTower)
			{
				GameEventRouter.SendEvent(HoodEvents.Hood_GoldenTowerSequenceScheduled);
			}
			GameEventRouter.SendEvent(HoodEvents.Hood_NewResident_BeingPlaced);
			GameEventRouter.SendEvent(HoodEvents.Hood_NewResident_Placed);
			List<FurbyBaby> furbiesOfTribe = FurbyGlobals.BabyRepositoryHelpers.GetBabiesInHoodOfTribe(m_TargetBaby.Tribe);
			if (furbiesOfTribe.Contains(m_TargetBaby))
			{
				if (furbiesOfTribe.Count == 1)
				{
					GameEventRouter.SendEvent(HoodEvents.Hood_FirstOfATribe);
				}
			}
			else if (furbiesOfTribe.Count == 0)
			{
				GameEventRouter.SendEvent(HoodEvents.Hood_FirstOfATribe);
			}
		}

		private IEnumerator RevealSequence_CommonEnding()
		{
			yield return new WaitForSeconds(1f);
			if (FurbyGlobals.Player.InProgressFurbyBaby == m_TargetBaby)
			{
				FurbyGlobals.Player.InProgressFurbyBaby = null;
			}
			if (!m_isCrystal)
			{
				GameEventRouter.SendEvent(HoodEvents.Hood_NewResident_PlacedFully);
			}
		}

		private void SelfDestruct()
		{
			Object.Destroy(base.transform.gameObject);
		}

		private bool ShouldTriggerTheEndSequence()
		{
			switch (m_TargetBaby.Tribe.TribeSet)
			{
			case Tribeset.Promo:
			case Tribeset.Golden:
			case Tribeset.CrystalGolden:
				return false;
			case Tribeset.MainTribes:
			case Tribeset.Spring:
			case Tribeset.CrystalGem:
				return true;
			default:
				Logging.LogError("ShouldTriggerTheEndSequence -> can't work out tribeset");
				return false;
			}
		}
	}
}
