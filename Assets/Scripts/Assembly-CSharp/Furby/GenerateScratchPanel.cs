using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class GenerateScratchPanel : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_letter;

		[SerializeField]
		private float m_stride;

		[SerializeField]
		private DictionaryLogic.DictionaryMode m_dictionaryMode;

		[SerializeField]
		private Color m_noWordsExistColor = Color.grey;

		[SerializeField]
		private Color m_wordsExistColor = Color.white;

		[SerializeField]
		private DictionaryWordList m_dictionary;

		private Dictionary<string, UILabel> m_letterDictionary = new Dictionary<string, UILabel>();

		private void Start()
		{
			float num = 0f;
			for (char c = 'A'; c <= 'Z'; c = (char)(c + 1))
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(m_letter);
				gameObject.transform.parent = base.transform;
				gameObject.transform.localScale = m_letter.transform.localScale;
				gameObject.transform.localPosition = new Vector3(0f, num, 0f);
				UILabel component = gameObject.GetComponent<UILabel>();
				component.text = c.ToString();
				m_letterDictionary[component.text] = component;
				num += m_stride;
			}
			BoxCollider boxCollider = (BoxCollider)base.collider;
			boxCollider.center = new Vector3(0f, num / 2f, 0f);
			boxCollider.size = new Vector3(boxCollider.size.x, num, 1f);
			RefreshLettering();
			GameEventRouter.AddDelegateForEnums(HandleSwitchEvent, DictionaryGameEvent.SwitchTranslationMode);
		}

		private void OnDestroy()
		{
			GameEventRouter.RemoveDelegateForEnums(HandleSwitchEvent, DictionaryGameEvent.SwitchTranslationMode);
		}

		protected void HandleSwitchEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			if (m_dictionaryMode == DictionaryLogic.DictionaryMode.EnglishToFurbish)
			{
				m_dictionaryMode = DictionaryLogic.DictionaryMode.FurbishToEnglish;
			}
			else
			{
				m_dictionaryMode = DictionaryLogic.DictionaryMode.EnglishToFurbish;
			}
			RefreshLettering();
		}

		private void RefreshLettering()
		{
			foreach (UILabel value in m_letterDictionary.Values)
			{
				value.color = m_noWordsExistColor;
			}
			int numWords = m_dictionary.GetNumWords(m_dictionaryMode);
			for (int i = 0; i < numWords; i++)
			{
				string nativeNamedTextWithNonAlphabetCharactersStriped = m_dictionary.GetWord(i, m_dictionaryMode).GetNativeNamedTextWithNonAlphabetCharactersStriped();
				string key = nativeNamedTextWithNonAlphabetCharactersStriped[0].ToString().ToUpper();
				if (m_letterDictionary.ContainsKey(key))
				{
					m_letterDictionary[key].color = m_wordsExistColor;
				}
			}
		}

		private void OnDrag(Vector2 dragPos)
		{
			UpdateDrag();
		}

		private void OnPress()
		{
			UpdateDrag();
		}

		private void UpdateDrag()
		{
			BoxCollider boxCollider = (BoxCollider)base.collider;
			if (!(boxCollider == null) && !(UICamera.currentCamera == null) && UICamera.currentTouch != null)
			{
				UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
				Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);
				float enter;
				if (new Plane(base.transform.rotation * Vector3.back, base.transform.position).Raycast(ray, out enter))
				{
					Vector3 vector = base.transform.localPosition + boxCollider.center - boxCollider.size * 0.5f;
					Vector3 vector2 = base.transform.localPosition - vector;
					Vector3 vector3 = base.transform.InverseTransformPoint(ray.GetPoint(enter));
					float num = (vector3 + vector2).y / boxCollider.size.y;
					int num2 = Mathf.FloorToInt(num * 26f);
					GameEventRouter.SendEvent(DictionaryGameEvent.SelectLetter, null, ((char)(65 + num2)).ToString());
				}
			}
		}
	}
}
