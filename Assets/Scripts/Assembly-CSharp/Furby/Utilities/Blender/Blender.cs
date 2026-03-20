using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Blender
{
	public class Blender : Singleton<Blender>
	{
		public delegate void ItemHandler(BlenderItem item);

		public delegate void Handler();

		public PlayMakerFSM m_GameState;

		public Transform m_ItemTray;

		public SkinnedMeshRenderer m_ContentsMesh;

		public Renderer m_BottleContentsMesh;

		public GameObject m_ItemPrefab;

		public UILabel m_StartButton;

		private BabyBlenderReaction m_BlenderReaction;

		private int m_ScoreValue;

		public GameObject blender;

		public bool IsFull
		{
			get
			{
				return GetFreePinningPoint() == null;
			}
		}

		public event ItemHandler Added;

		public event ItemHandler Removed;

		public event Handler Activated;

		public event Handler BlendCompleted;

		public event Handler Drunk;

		public void Start()
		{
			Singleton<FurbyDataChannel>.Instance.SetConnectionTone(FurbyCommand.Application);
			Singleton<FurbyDataChannel>.Instance.SetHeartBeatActive(true);
		}

		public void Update()
		{
			FsmVariables fsmVariables = m_GameState.FsmVariables;
			float value = fsmVariables.GetFsmFloat("BlenderItemFade").Value;
			UISprite[] componentsInChildren = m_ItemTray.GetComponentsInChildren<UISprite>();
			foreach (UISprite uISprite in componentsInChildren)
			{
				uISprite.alpha = value;
			}
		}

		public void OnClickExit()
		{
			m_GameState.SendEvent("Exit");
		}

		public void OnInsertItem(CarouselItem trayItem)
		{
			if (!(m_GameState.ActiveStateName == "Prepare"))
			{
				return;
			}
			Transform freePinningPoint = GetFreePinningPoint();
			Ingredient itemIngredient = trayItem.m_ItemIngredient;
			if (freePinningPoint != null)
			{
				GameObject gameObject = Object.Instantiate(m_ItemPrefab) as GameObject;
				BlenderItem component = gameObject.GetComponent<BlenderItem>();
				UISprite component2 = gameObject.GetComponent<UISprite>();
				Vector3 one = Vector3.one;
				component2.spriteName = itemIngredient.Graphic;
				component.m_ItemIngredient = itemIngredient;
				one.x = component2.sprite.inner.width;
				one.y = component2.sprite.inner.height;
				gameObject.transform.parent = freePinningPoint;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localScale = one;
				component.OnIndexChange(m_ItemTray.childCount);
				gameObject.transform.parent.GetComponent<Animation>().Play();
				if (null == GetFreePinningPoint())
				{
					m_GameState.SendEvent("Added (Full)");
				}
				else
				{
					m_GameState.SendEvent("Added (Pending)");
				}
				if (this.Added != null)
				{
					this.Added(component);
				}
			}
		}

		public void OnClick()
		{
			switch (m_GameState.ActiveStateName)
			{
			case "Edible":
				OnPourBlend();
				break;
			case "Prepare":
				OnStartBlend();
				break;
			}
		}

		public void OnReset()
		{
			ClearDebugTextVerdict();
		}

		public void OnFinishBlend()
		{
			m_BlenderReaction = GetBabyReaction();
			m_ScoreValue = GetBlenderScore();
			UpdateFoodPreferences();
			GameEventRouter.SendEvent(BlenderEvent.FinishedBlending);
			foreach (BlenderItem bottleItem in GetBottleItems())
			{
				Object.Destroy(bottleItem.gameObject);
			}
			if (this.BlendCompleted != null)
			{
				this.BlendCompleted();
			}
		}

		private void MakeBlendColour()
		{
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			int num = 0;
			foreach (BlenderItem bottleItem in GetBottleItems())
			{
				num++;
				m_ContentsMesh.material.SetColor("_DebugColour" + num, bottleItem.m_ItemIngredient.Colour);
			}
			float num2 = 1f / (float)num;
			foreach (BlenderItem bottleItem2 in GetBottleItems())
			{
				Vector3 vector = new Vector3(bottleItem2.m_ItemIngredient.Colour.r, bottleItem2.m_ItemIngredient.Colour.g, bottleItem2.m_ItemIngredient.Colour.b);
				zero2.x += vector.x * num2;
				zero2.y += vector.y * num2;
				zero2.z += vector.z * num2;
			}
			zero = HSV.RGBtoHSV(zero2);
			m_ContentsMesh.material.SetColor("_DebugColourMix", new Color(zero2.x, zero2.y, zero2.z, 1f));
			m_ContentsMesh.material.SetFloat("_Hue", zero.x);
			m_ContentsMesh.material.SetFloat("_Sat", zero.y);
			m_ContentsMesh.material.SetFloat("_Val", zero.z);
			if (m_BottleContentsMesh != null)
			{
				m_BottleContentsMesh.material.SetFloat("_Hue", zero.x);
				m_BottleContentsMesh.material.SetFloat("_Sat", zero.y);
				m_BottleContentsMesh.material.SetFloat("_Val", zero.z);
			}
		}

		public void OnStartBlend()
		{
			MakeBlendColour();
			if (null == GetFreePinningPoint())
			{
				m_GameState.SendEvent("Activate");
				if (this.Activated != null)
				{
					this.Activated();
				}
			}
		}

		public void OnSlideInBlender()
		{
			if (blender.animation.isPlaying)
			{
				blender.animation.Stop();
			}
			blender.animation.Play("blender_enterScreen");
		}

		public void OnPourBlend()
		{
			m_GameState.SendEvent("Pour");
			if (this.Drunk != null)
			{
				this.Drunk();
			}
		}

		public void OnBabyReact()
		{
			switch (m_BlenderReaction)
			{
			case BabyBlenderReaction.Generic:
				GameEventRouter.SendEvent(BlenderEvent.BabyReactionGeneric);
				break;
			case BabyBlenderReaction.Vomit:
				GameEventRouter.SendEvent(BlenderEvent.BabyReactionVomit);
				break;
			case BabyBlenderReaction.Burp:
				GameEventRouter.SendEvent(BlenderEvent.BabyReactionBurp);
				break;
			case BabyBlenderReaction.Fart:
				GameEventRouter.SendEvent(BlenderEvent.BabyReactionFart);
				break;
			case BabyBlenderReaction.Sour:
				GameEventRouter.SendEvent(BlenderEvent.BabyReactionSour);
				break;
			case BabyBlenderReaction.Spicy:
				GameEventRouter.SendEvent(BlenderEvent.BabyReactionSpicy);
				break;
			}
		}

		private void DoDebugTextVerdict()
		{
			GameObject gameObject = GameObject.Find("ReactionText");
			if (!(gameObject != null))
			{
				return;
			}
			TextMesh component = gameObject.GetComponent<TextMesh>();
			if (component != null)
			{
				if (m_ScoreValue == 0)
				{
					component.text = "Hates it";
				}
				else if (m_ScoreValue == 1)
				{
					component.text = "Thinks it's ok";
				}
				else
				{
					component.text = "Loves it";
				}
			}
		}

		private void ClearDebugTextVerdict()
		{
			GameObject gameObject = GameObject.Find("ReactionText");
			if (gameObject != null)
			{
				TextMesh component = gameObject.GetComponent<TextMesh>();
				if (component != null)
				{
					component.text = string.Empty;
				}
			}
		}

		public void OnBabyVerdict()
		{
			switch (m_ScoreValue)
			{
			case 0:
				GameEventRouter.SendEvent(BlenderEvent.BabyVerdictBad);
				break;
			case 1:
				GameEventRouter.SendEvent(BlenderEvent.BabyVerdictOkay);
				break;
			case 2:
				GameEventRouter.SendEvent(BlenderEvent.BabyVerdictGood);
				break;
			case 3:
				GameEventRouter.SendEvent(BlenderEvent.BabyVerdictGreat);
				break;
			case 4:
				GameEventRouter.SendEvent(BlenderEvent.BabyVerdictGreat);
				break;
			default:
				GameEventRouter.SendEvent(BlenderEvent.BabyVerdictPerfect);
				break;
			}
			DoDebugTextVerdict();
		}

		private IEnumerator OnShowScoringGUI()
		{
			yield return new WaitForSeconds(2f);
			base.gameObject.SendGameEvent(BabyEndMinigameEvent.SetScore, m_ScoreValue);
			base.gameObject.SendGameEvent(BabyEndMinigameEvent.ShowDialog);
		}

		public void OnShowScoring()
		{
			StartCoroutine(OnShowScoringGUI());
		}

		public void OnRemoveItem(BlenderItem dragItem)
		{
			if (m_GameState.ActiveStateName == "Prepare")
			{
				Transform freePinningPoint = GetFreePinningPoint();
				if (null == freePinningPoint)
				{
					m_GameState.SendEvent("Removed (Full)");
				}
				else
				{
					m_GameState.SendEvent("Removed (Pending)");
				}
				if (this.Removed != null)
				{
					this.Removed(dragItem);
				}
				Object.Destroy(dragItem.gameObject);
			}
		}

		private BabyBlenderReaction GetBabyReaction()
		{
			int num = 0;
			foreach (BlenderItem bottleItem in GetBottleItems())
			{
				Ingredient itemIngredient = bottleItem.m_ItemIngredient;
				int reaction = (int)itemIngredient.Reaction;
				if ((num & reaction) != 0)
				{
					return (BabyBlenderReaction)reaction;
				}
				num |= reaction;
			}
			if (num == 0 || Mathf.IsPowerOfTwo(num))
			{
				return (BabyBlenderReaction)num;
			}
			return (BabyBlenderReaction)(Mathf.NextPowerOfTwo(num) >> 1);
		}

		private int GetBlenderScore()
		{
			float num = 0f;
			FurbyBaby selectedFurbyBaby = FurbyGlobals.Player.SelectedFurbyBaby;
			FurbyBabyPersonality personality = selectedFurbyBaby.Personality;
			foreach (BlenderItem bottleItem in GetBottleItems())
			{
				Ingredient itemIngredient = bottleItem.m_ItemIngredient;
				int num2 = itemIngredient.Score[personality];
				num += (float)num2;
			}
			return Mathf.RoundToInt(num / 3f);
		}

		private void UpdateFoodPreferences()
		{
			FurbyBaby selectedFurbyBaby = FurbyGlobals.Player.SelectedFurbyBaby;
			FurbyBabyPersonality personality = selectedFurbyBaby.Personality;
			foreach (BlenderItem bottleItem in GetBottleItems())
			{
				Ingredient itemIngredient = bottleItem.m_ItemIngredient;
				int score = itemIngredient.Score[personality];
				selectedFurbyBaby.OnFoodFeedback(itemIngredient.Name, score);
			}
		}

		private Transform GetFreePinningPoint()
		{
			for (int i = 0; i < m_ItemTray.childCount; i++)
			{
				Transform child = m_ItemTray.GetChild(i);
				if (child.childCount == 0)
				{
					return child;
				}
			}
			return null;
		}

		private IEnumerable<BlenderItem> GetBottleItems()
		{
			for (int i = 0; i < m_ItemTray.childCount; i++)
			{
				Transform childXform = m_ItemTray.GetChild(i);
				if (childXform.childCount != 0)
				{
					yield return childXform.GetComponentInChildren<BlenderItem>();
				}
			}
		}
	}
}
