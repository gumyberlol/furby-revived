using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.EggCarton
{
	public class CartonEggGrid : MonoBehaviour
	{
		private const int NUM_EGG_SLOTS = 12;

		[SerializeField]
		private Transform[] m_eggPositions;

		[SerializeField]
		private GameObject m_eggPrefab;

		[SerializeField]
		private AnimationClip m_idleAnimation;

		private GameObject[] m_CurrentEggs = new GameObject[12];

		private void Start()
		{
			for (int i = 0; i < 12; i++)
			{
				m_CurrentEggs[i] = null;
			}
			bool shouldOnlyAddNewEggs = false;
			HandleEggEntrancing(shouldOnlyAddNewEggs);
		}

		private void OnDisable()
		{
			SetComAirAutoConnect(true);
		}

		private void OnDestroy()
		{
			SetComAirAutoConnect(true);
		}

		public void HandleEggEntrancing(bool shouldOnlyAddNewEggs)
		{
			bool flag = false;
			foreach (FurbyBaby item in FurbyGlobals.BabyRepositoryHelpers.GetBabiesOfProgress(FurbyBabyProgresss.E))
			{
				if ((!shouldOnlyAddNewEggs || item.m_persistantData.newToCarton) && FurbyGlobals.Player.InProgressFurbyBaby != item)
				{
					int num = FindEmptySlot();
					GameObject gameObject = (GameObject)Object.Instantiate(m_eggPrefab);
					gameObject.transform.parent = m_eggPositions[num];
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localScale = Vector3.one;
					gameObject.transform.localRotation = Quaternion.identity;
					BabyInstance componentInChildren = gameObject.GetComponentInChildren<BabyInstance>();
					gameObject.layer = base.gameObject.layer;
					componentInChildren.SetTargetFurbyBaby(item);
					componentInChildren.InstantiateObject();
					if (item.m_persistantData.newToCarton)
					{
						PlayEntryAnimation(componentInChildren.gameObject);
						item.m_persistantData.newToCarton = false;
						Singleton<GameDataStoreObject>.Instance.Save();
						flag = true;
						SelectEgg componentInChildren2 = gameObject.GetComponentInChildren<SelectEgg>();
						componentInChildren2.DisableCollision();
					}
					else if (m_idleAnimation != null && componentInChildren.Instance != null)
					{
						componentInChildren.Instance.animation.Play(m_idleAnimation.name);
						componentInChildren.Instance.animation[m_idleAnimation.name].wrapMode = WrapMode.Loop;
						componentInChildren.Instance.animation[m_idleAnimation.name].normalizedTime = Random.Range(0f, 1f);
					}
					m_CurrentEggs[num] = gameObject;
				}
			}
			if (flag)
			{
				GameEventRouter.SendEvent(CartonGameEvent.EggCartonStartedEggsWillBeAdded);
				StartCoroutine(EnterEggCartonComAir());
			}
			else
			{
				GameEventRouter.SendEvent(CartonGameEvent.EggCartonStartedNoEggsAdded);
				SetComAirAutoConnect(false);
			}
		}

		private void SetComAirAutoConnect(bool shouldEnable)
		{
			if (Singleton<FurbyDataChannel>.Instance != null)
			{
				Singleton<FurbyDataChannel>.Instance.AutoConnect = shouldEnable;
			}
		}

		public void RemoveEgg(FurbyBaby eggToRemove)
		{
			for (int i = 0; i < 12; i++)
			{
				if (m_CurrentEggs[i] != null)
				{
					GameObject gameObject = m_CurrentEggs[i];
					BabyInstance componentInChildren = gameObject.GetComponentInChildren<BabyInstance>();
					FurbyBaby targetFurbyBaby = componentInChildren.GetTargetFurbyBaby();
					if (targetFurbyBaby == eggToRemove)
					{
						Object.Destroy(gameObject);
						m_CurrentEggs[i] = null;
						break;
					}
				}
			}
		}

		public int FindEmptySlot()
		{
			for (int i = 0; i < 12; i++)
			{
				if (m_CurrentEggs[i] == null)
				{
					return i;
				}
			}
			return -1;
		}

		private IEnumerator WaitForEggToFinishEntrancing()
		{
			yield return new WaitForSeconds(6.5f);
			GameEventRouter.SendEvent(CartonGameEvent.EggsFinishedEntrancing);
		}

		private IEnumerator EnterEggCartonComAir()
		{
			StartCoroutine(WaitForEggToFinishEntrancing());
			yield return new WaitForSeconds(2f);
			if (!FurbyGlobals.Player.NoFurbyForEitherReason() && Singleton<FurbyDataChannel>.Instance.FurbyConnected)
			{
				yield return this.SendAction(2f, FurbyAction.Baby_Born, null);
			}
			SetComAirAutoConnect(false);
			yield return ComAirExtensions.WaitWhileComAirIsBusy();
		}

		private void PlayEntryAnimation(GameObject targetObject)
		{
			GameObject instance = targetObject.GetComponent<ModelInstance>().Instance;
			bool flag = targetObject.transform.parent.parent.localPosition.x > 0f;
			string text = ((!flag) ? "enterRow02" : "enterRow01");
			instance.animation.Play(text);
			instance.animation.PlayQueued(m_idleAnimation.name);
			instance.animation[m_idleAnimation.name].wrapMode = WrapMode.Loop;
			GameEventRouter.SendEvent(flag ? CartonGameEvent.EggBouncesIntoLeftRow : CartonGameEvent.EggBouncesIntoRightRow);
		}
	}
}
