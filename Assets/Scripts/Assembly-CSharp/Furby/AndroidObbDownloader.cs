using System.Collections;
using System.IO;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class AndroidObbDownloader : MonoBehaviour
	{
		[GameEventEnum]
		public enum AndroidObbEvent
		{
			NoExternalStorage = 0,
			DownloadRequired = 1,
			DownloadFailed = 2
		}

		[SerializeField]
		private GameObject[] m_objectsToEnableOnSuccess;

		[SerializeField]
		private NamedTextTable m_buildInformation;

		[SerializeField]
		private GameObject m_jenkinsPanel;

		[SerializeField]
		private string m_publicKey = "REPLACE THIS WITH YOUR PUBLIC KEY";

		private string m_expPath;

		private string m_mainPath;

		private IEnumerator Start()
		{
			yield return null;
			yield return StartCoroutine(DownloadDataFromGooglePlay());
			GameObject[] objectsToEnableOnSuccess = m_objectsToEnableOnSuccess;
			foreach (GameObject gObj in objectsToEnableOnSuccess)
			{
				gObj.SetActive(gObj);
			}
		}

		private IEnumerator DownloadDataFromGooglePlay()
		{
			WaitForGameEvent waiter = new WaitForGameEvent();
			GooglePlayDownloader.Initialise(m_publicKey);
			m_expPath = GooglePlayDownloader.GetExpansionFilePath();
			if (string.IsNullOrEmpty(m_expPath))
			{
				GameEventRouter.SendEvent(AndroidObbEvent.NoExternalStorage);
				yield return StartCoroutine(waiter.WaitForEvent(SharedGuiEvents.Quit));
				Application.Quit();
				yield break;
			}
			m_mainPath = GooglePlayDownloader.GetMainOBBPath(m_expPath);
			if (!string.IsNullOrEmpty(m_mainPath))
			{
				yield break;
			}
			GameEventRouter.SendEvent(AndroidObbEvent.DownloadRequired);
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(SharedGuiEvents.Quit, SharedGuiEvents.DialogAccept));
				if (waiter.ReturnedEvent.Equals(SharedGuiEvents.Quit))
				{
					Application.Quit();
					break;
				}
				GameEventRouter.SendEvent(SharedGuiEvents.DialogHide);
				GooglePlayDownloader.FetchOBB();
				yield return new WaitForSeconds(1f);
				m_mainPath = GooglePlayDownloader.GetMainOBBPath(m_expPath);
				if (!string.IsNullOrEmpty(m_mainPath))
				{
					break;
				}
				if (Debug.isDebugBuild)
				{
					yield return StartCoroutine(DownloadFromJenkins());
					m_mainPath = GooglePlayDownloader.GetMainOBBPath(m_expPath);
					if (!string.IsNullOrEmpty(m_mainPath))
					{
						break;
					}
				}
				GameEventRouter.SendEvent(AndroidObbEvent.DownloadFailed);
			}
		}

		private IEnumerator DownloadFromJenkins()
		{
			if (m_buildInformation == null)
			{
				Logging.Log("No build information");
				yield break;
			}
			m_jenkinsPanel.GetComponent<UIPanel>().enabled = true;
			int changelist = GooglePlayDownloader.GetVersion();
			string branch = m_buildInformation.GetString("BUILD_BRANCH");
			string url = string.Format("http://testapp.relentless.co.uk:8090/deploy/Store/Furby/Game/{1}/Android/CL_{0}/main.{0}.com.hasbro.furbyboom.obb", changelist, branch);
			string targetPath = string.Format("{0}/main.{1}.{2}.obb", m_expPath, changelist, GooglePlayDownloader.GetPackage());
			Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
			UISlider slider = m_jenkinsPanel.GetComponentInChildren<UISlider>();
			using (WWW www = new WWW(url))
			{
				while (!www.isDone)
				{
					slider.sliderValue = www.progress;
					yield return null;
				}
				if (!string.IsNullOrEmpty(www.error))
				{
					Logging.Log(www.error);
					m_jenkinsPanel.GetComponentInChildren<UILabel>().text = www.error;
				}
				else
				{
					File.WriteAllBytes(targetPath, www.bytes);
					Logging.Log(string.Format("Downloaded to {0}", targetPath));
					Application.Quit();
				}
			}
			m_jenkinsPanel.GetComponent<UIPanel>().enabled = false;
		}
	}
}
