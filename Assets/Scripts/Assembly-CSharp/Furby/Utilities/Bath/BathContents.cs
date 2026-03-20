using Relentless;
using UnityEngine;

namespace Furby.Utilities.Bath
{
	public class BathContents : Singleton<BathContents>
	{
		private float waterTemperature_;

		private float waterAmount_;

		public GUIText debugText;

		public float coldWaterTemperature = 10f;

		public float hotWaterTemperature = 40f;

		public float optimumLowTemp = 28f;

		public float optimumHighTemp = 34f;

		public ParticleSystem steamParticles;

		public float steamLowTemp = 28f;

		public float steamHighTemp = 41f;

		public float lowSteamEmitterRate = 2f;

		public float highSteamEmitterRate = 15f;

		public ParticleSystem mistParticles;

		public float mistLowTemp = 9f;

		public float mistHighTemp = 18f;

		public float lowMistEmitterRate = 3f;

		public float highMistEmitterRate = 1f;

		private PlayMakerFSM bathStateMachine_;

		public Transform pins;

		public BathCarouselItem pinnedItem;

		private BathCarouselItem item_;

		public float waterTemperature
		{
			get
			{
				return waterTemperature_;
			}
		}

		public float waterAmount
		{
			get
			{
				return waterAmount_;
			}
		}

		private void Start()
		{
			waterTemperature_ = 0f;
			waterAmount_ = 0f;
			bathStateMachine_ = GetComponent<PlayMakerFSM>();
		}

		private void Update()
		{
			string text = null;
			if ((bool)debugText)
			{
				text = "Water Level:" + waterAmount_ + " \nTemperature:" + waterTemperature_;
			}
			if (steamParticles != null)
			{
				if (waterTemperature_ < steamLowTemp || waterAmount_ < 0.1f)
				{
					steamParticles.enableEmission = false;
				}
				else
				{
					steamParticles.enableEmission = true;
					float num = (waterTemperature_ - steamLowTemp) / (steamHighTemp - steamLowTemp);
					float num2 = lowSteamEmitterRate + num * (highSteamEmitterRate - lowSteamEmitterRate);
					steamParticles.emissionRate = num2;
					if ((bool)debugText)
					{
						text = text + "\nSteam Rate:" + num2;
					}
				}
			}
			if (mistParticles != null)
			{
				if (waterTemperature_ > mistHighTemp || waterAmount_ < 0.1f)
				{
					mistParticles.enableEmission = false;
				}
				else
				{
					mistParticles.enableEmission = true;
					float num3 = (waterTemperature_ - mistLowTemp) / (mistHighTemp - mistLowTemp);
					float num4 = lowMistEmitterRate + num3 * (highMistEmitterRate - lowMistEmitterRate);
					mistParticles.emissionRate = num4;
					if ((bool)debugText)
					{
						text = text + "\nMist Rate:" + num4;
					}
				}
			}
			if ((bool)debugText)
			{
				debugText.text = text;
			}
		}

		private void Clamp()
		{
			if (waterAmount_ > 1f)
			{
				waterAmount_ = 1f;
			}
			if (waterAmount_ < 0f)
			{
				waterAmount_ = 0f;
			}
		}

		private void AddWater(float amount, float temperature)
		{
			float num = waterAmount_ * waterTemperature_;
			float num2 = amount * temperature;
			float num3 = waterAmount_ + amount;
			float num4 = (num + num2) / num3;
			waterAmount_ = num3;
			waterTemperature_ = num4;
			Clamp();
		}

		private void RemoveWater(float amount)
		{
			waterAmount_ -= amount;
			Clamp();
		}

		public void AddHotWater(float amount)
		{
			AddWater(amount, hotWaterTemperature);
		}

		public void AddColdWater(float amount)
		{
			AddWater(amount, coldWaterTemperature);
		}

		public void DrainWater(float amount)
		{
			RemoveWater(amount);
		}

		public float EvaluateBath()
		{
			float num = 0f;
			num = ((!(waterAmount > 0.2f)) ? 0f : 1f);
			float num2 = Mathf.Abs(waterTemperature_ - optimumLowTemp) / (optimumHighTemp - optimumLowTemp);
			num *= 0.5f + num2 * 0.5f;
			num = ((!(null == item_)) ? (num * ((!(item_.m_ItemIngredient.Name == "icecube")) ? 1f : 0.6f)) : (num * 0.88f));
			if (num > 1f)
			{
				num = 1f;
			}
			if (num < 0f)
			{
				num = 0f;
			}
			Logging.Log("EvaluateBath(): Rating = " + num);
			return num;
		}

		private BathCarouselItem GetPinnedItem()
		{
			if (null == pinnedItem)
			{
				Logging.Log("BathContents::GetPinnedItem: Item not set");
			}
			return pinnedItem;
		}

		private Transform GetRandomPinningPoint()
		{
			if (null == pins)
			{
				Logging.Log("BathContents::GetRandomPinningPoint: No pins set!");
				return null;
			}
			int childCount = pins.childCount;
			if (childCount == 0)
			{
				Logging.Log("BathContents::GetRandomPinningPoint: No pins set!");
				return null;
			}
			int num = Random.Range(0, childCount);
			int num2 = 0;
			int num3 = num;
			while (num2 < childCount)
			{
				Transform child = pins.GetChild(num3);
				if (child.childCount == 0)
				{
					return child;
				}
				num2++;
				num3 = (num3 + 1) % childCount;
			}
			return null;
		}

		private bool PinItem()
		{
			BathCarouselItem bathCarouselItem = GetPinnedItem();
			if (!bathCarouselItem)
			{
				return false;
			}
			bathCarouselItem.transform.parent = null;
			Transform randomPinningPoint = GetRandomPinningPoint();
			if (null == randomPinningPoint)
			{
				return false;
			}
			bathCarouselItem.transform.parent = randomPinningPoint;
			bathCarouselItem.transform.localPosition = default(Vector3);
			bathCarouselItem.m_ItemIngredient = item_.m_ItemIngredient;
			UISprite component = bathCarouselItem.GetComponent<UISprite>();
			UISprite component2 = item_.GetComponent<UISprite>();
			component.spriteName = component2.spriteName;
			Vector3 one = Vector3.one;
			one.x = component2.sprite.inner.width;
			one.y = component2.sprite.inner.height;
			bathCarouselItem.transform.localScale = one;
			bathCarouselItem.gameObject.SetActive(true);
			return true;
		}

		private bool UnpinItem()
		{
			BathCarouselItem bathCarouselItem = GetPinnedItem();
			if (!bathCarouselItem)
			{
				return false;
			}
			bathCarouselItem.gameObject.SetActive(false);
			return true;
		}

		public bool AddItem(BathCarouselItem newItem)
		{
			if (null != item_)
			{
				return false;
			}
			item_ = newItem;
			PinItem();
			UISprite component = newItem.GetComponent<UISprite>();
			component.alpha = 0.5f;
			bathStateMachine_.SendEvent("ItemAdded");
			return true;
		}

		public bool DiscardItem()
		{
			return RemoveItem(pinnedItem);
		}

		public bool RemoveItem(BathCarouselItem oldItem)
		{
			if (!(pinnedItem == oldItem) && oldItem != item_)
			{
				return false;
			}
			BathCarouselItem bathCarouselItem = item_;
			UnpinItem();
			item_ = null;
			UISprite component = bathCarouselItem.GetComponent<UISprite>();
			component.alpha = 1f;
			bathStateMachine_.SendEvent("ItemRemoved");
			return true;
		}
	}
}
