using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Help
{
	public class HelpPage : MonoBehaviour
	{
		private HelpData m_data;

		private HelpPageData m_pageData;

		public void SetupFrom(HelpData data, HelpPageData pageData)
		{
			m_data = data;
			m_pageData = pageData;
		}

		public void ActivatePage()
		{
			if (m_pageData.m_LaunchVideo)
			{
				m_data.m_VideoText_Caption.text = string.Empty;
				if ((bool)m_data.m_SubtitleGameObject)
				{
					m_data.m_SubtitleGameObject.SetActive(false);
				}
				switch (m_pageData.m_VideoTextType)
				{
				case VideoText.Caption:
					m_data.m_VideoText_Caption.text = Singleton<Localisation>.Instance.GetText(m_pageData.m_VideoTextString);
					break;
				case VideoText.Subtitle:
					if ((bool)m_data.m_SubtitleGameObject)
					{
						m_data.m_SubtitleGameObject.SetActive(true);
					}
					break;
				}
				StartCoroutine(PlayVideo());
			}
			else
			{
				StartCoroutine(LoadAndDisplayTexture());
			}
		}

		private IEnumerator PlayVideo()
		{
			MobileMovieTexturePlayer.StopAllVideos();
			yield return new WaitForSeconds(0.05f);
			m_data.m_moviePlayer.PlayVideo(m_pageData.m_videoFilename, m_pageData.m_fabricEventName);
		}

		private void OnDestroy()
		{
			DeactivatePage();
		}

		public void DeactivatePage()
		{
			if (m_pageData.m_LaunchVideo)
			{
				m_data.m_moviePlayer.StopVideo();
			}
			switch (m_pageData.m_VideoTextType)
			{
			case VideoText.Caption:
				m_data.m_VideoText_Caption.text = string.Empty;
				break;
			case VideoText.Subtitle:
				if ((bool)m_data.m_SubtitleGameObject)
				{
					m_data.m_SubtitleGameObject.SetActive(false);
				}
				break;
			}
		}

		private IEnumerator LoadAndDisplayTexture()
		{
			UITexture uiTex = GetComponent<UITexture>();
			string prefix = Application.streamingAssetsPath;
			string dir = m_data.m_rootDirectory;
			string file = m_pageData.m_textureName;
			string url = string.Format("{0}/{1}/{2}.jpg", prefix, dir, file);
			if (!url.Contains("://"))
			{
				url = string.Format("file://{0}", url);
			}
			Logging.Log(string.Format("Loading from \"{0}\"", url));
			WWW www = new WWW(url);
			yield return www;
			Texture2D t = www.texture;
			Texture prev = uiTex.mainTexture;
			uiTex.mainTexture = t;
			if (prev != null)
			{
				Object.Destroy(prev);
			}
			base.transform.localScale = new Vector3(t.width, t.height, 1f);
			TweenAlpha alpha = GetComponent<TweenAlpha>();
			alpha.from = 0f;
			alpha.to = 1f;
			alpha.Play(true);
		}

		public void UnloadTexture()
		{
			UITexture component = GetComponent<UITexture>();
			Texture mainTexture = component.mainTexture;
			component.mainTexture = null;
			Object.Destroy(mainTexture);
		}
	}
}
