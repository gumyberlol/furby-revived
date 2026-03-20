using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class ThemeButton : MonoBehaviour
	{
		[SerializeField]
		private UILabel m_label;

		[SerializeField]
		private ThemeIcons m_iconSet;

		[SerializeField]
		private UISprite m_sprite;

		[SerializeField]
		private UISprite m_Tick;

		private ThemePeriod m_period;

		public event EventHandler Clicked;

		public void SetupFrom(ThemePeriod p)
		{
			string text = ((!(p != null)) ? "(auto)" : p.name);
			base.gameObject.name = "Button for " + text;
			string key = ((!(p != null)) ? "SETTINGS_THEME_BASE" : p.m_nameKey);
			string text2 = Singleton<Localisation>.Instance.GetText(key);
			m_label.text = text2;
			m_period = p;
			if (m_period == FurbyGlobals.ThemePeriodChooser.GetPeriod())
			{
				m_Tick.gameObject.SetActive(true);
			}
			else
			{
				m_Tick.gameObject.SetActive(false);
			}
			SetupSprite(p);
		}

		private void SetupSprite(ThemePeriod p)
		{
			if (p != null)
			{
				string spriteNameFor = m_iconSet.GetSpriteNameFor(p);
				m_sprite.spriteName = spriteNameFor;
				UIAtlas.Sprite sprite = m_sprite.atlas.GetSprite(spriteNameFor);
				Rect inner = sprite.inner;
				Transform transform = m_sprite.transform;
				transform.localScale = new Vector3(inner.width, inner.height, 1f);
			}
			else
			{
				UnityEngine.Object.Destroy(m_sprite.gameObject);
			}
		}

		public void OnClick()
		{
			GameData data = Singleton<GameDataStoreObject>.Instance.Data;
			string themePeriod = ((!(m_period != null)) ? string.Empty : m_period.name);
			data.SetThemePeriod(themePeriod);
			if (this.Clicked != null)
			{
				this.Clicked(this, EventArgs.Empty);
			}
		}
	}
}
