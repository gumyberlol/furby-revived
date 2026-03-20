using System;
using System.Collections;
using System.IO;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class SpriteFurbyAdultDisplay : RelentlessMonoBehaviour
	{
		private class LoadingException : ApplicationException
		{
			public LoadingException(string filename)
				: base(filename)
			{
			}
		}

		public enum FurbyDataSelectionType
		{
			UseCurrent = 0,
			UseSpecifiedSlot = 1
		}

		private delegate void TextureLoadedHandler(Texture t);

		[SerializeField]
		private UITexture StreamedTexture;

		[SerializeField]
		private UITexture PersonalityEyes;

		[SerializeField]
		private FurbyDataSelectionType m_furbySelectionType;

		[SerializeField]
		private int m_specifiedSaveSlot;

		[SerializeField]
		private FurbyData m_furbyData;

		private Material m_furMaterial;

		public FurbyData Furby
		{
			get
			{
				return m_furbyData;
			}
			set
			{
				m_furbyData = value;
				Refresh();
			}
		}

		private void Start()
		{
			if (m_furbyData == null)
			{
				SetDefaultFurby();
			}
		}

		private void OnEnable()
		{
			if (m_furbyData == null)
			{
				SetDefaultFurby();
			}
			else
			{
				Refresh();
			}
		}

		public void SetDefaultFurby()
		{
			switch (m_furbySelectionType)
			{
			case FurbyDataSelectionType.UseCurrent:
				if ((bool)FurbyGlobals.Player && !FurbyGlobals.Player.NoFurbyOnSaveGame())
				{
					m_furbyData = FurbyGlobals.Player.Furby;
				}
				break;
			case FurbyDataSelectionType.UseSpecifiedSlot:
			{
				GameData slot = Singleton<GameDataStoreObject>.Instance.GetSlot(m_specifiedSaveSlot);
				bool flag = false;
				AdultFurbyType adultFurbyType = slot.FurbyType;
				if (adultFurbyType == AdultFurbyType.Unknown)
				{
					adultFurbyType = AdultFurbyType.NoFurby;
				}
				foreach (FurbyData furby in FurbyGlobals.AdultLibrary.Furbies)
				{
					if (furby.AdultType == adultFurbyType)
					{
						m_furbyData = furby;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					Logging.LogError("Couldn't find a matching Furby type for save slot " + m_specifiedSaveSlot + " type is: " + slot.FurbyType);
				}
				break;
			}
			}
			Refresh();
		}

		public void Refresh()
		{
			if (!m_furbyData)
			{
				GetComponent<UIPanel>().enabled = false;
				return;
			}
			GetComponent<UIPanel>().enabled = true;
			StopAllCoroutines();
			StartCoroutine(LoadPictureAndEyes());
		}

		private IEnumerator LoadPictureAndEyes()
		{
			if (m_furbyData == null)
			{
				yield break;
			}
			Texture baseTexture = null;
			string dir = "CH_furbyAdult_02";
			string name = m_furbyData.Colouring.FurbySpriteName;
			string suffix = "02.png";
			string filename = string.Format("{0}/{1}{2}", dir, name, suffix);
			yield return StartCoroutine(LoadTexture(filename, delegate(Texture t)
			{
				baseTexture = t;
			}));
			bool showEyes = m_furbySelectionType == FurbyDataSelectionType.UseCurrent;
			Texture eyesTexture = null;
			if (showEyes)
			{
				string personality = Singleton<FurbyDataChannel>.Instance.FurbyStatus.Personality.ToString();
				Logging.Log(string.Format("Personality is \"{0}\"", personality));
				string eyesFilename = string.Format("{0}/CH_furbyAdult_Eyes_{1}.png", dir, personality);
				yield return StartCoroutine(LoadTexture(eyesFilename, delegate(Texture t)
				{
					eyesTexture = t;
				}));
			}
			StreamedTexture.mainTexture = baseTexture;
			bool haveBaseTexture = baseTexture != null;
			StreamedTexture.gameObject.SetActive(haveBaseTexture);
			PersonalityEyes.mainTexture = eyesTexture;
			bool haveEyesTexture = eyesTexture != null;
			PersonalityEyes.gameObject.SetActive(haveBaseTexture && haveEyesTexture);
		}

		private IEnumerator LoadTexture(string filename, TextureLoadedHandler whenDone)
		{
			string prefix = Application.streamingAssetsPath;
			string slash = Path.DirectorySeparatorChar.ToString();
			filename = prefix + slash + filename;
			if (!filename.Contains("://"))
			{
				filename = "file://" + filename;
			}
			Logging.Log("Loading texture from " + filename);
			WWW www = new WWW(filename);
			while (!www.isDone)
			{
				Logging.Log("DL progress: " + www.progress);
				yield return null;
			}
			string err = www.error;
			if (string.IsNullOrEmpty(err))
			{
				whenDone(www.texture);
			}
			else
			{
				Logging.LogError(string.Format("While loading \"{0}\", error: {1}", filename, err));
			}
		}
	}
}
