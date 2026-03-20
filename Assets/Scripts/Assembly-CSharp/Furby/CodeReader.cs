using System.Collections;
using System.Threading;
using UnityEngine;
using ZXing;

namespace Furby
{
	public class CodeReader : MonoBehaviour
	{
		public class Result
		{
			public string text;
		}

		public delegate void ResultHandler(Result r);

		[SerializeField]
		private UITexture m_uiTexture;

		private WebCamTexture m_webCam;

		private object m_mutex = new object();

		private Color32[] m_pixels;

		private int m_width;

		private int m_height;

		private Result m_result;

		private Thread m_workerThread;

		private BarcodeReader m_reader;

		public event ResultHandler CodeScanned;

		public IEnumerator Start()
		{
			m_reader = new BarcodeReader
			{
				AutoRotate = true
			};
			m_workerThread = new Thread(WorkerThreadFunc);
			m_workerThread.Start();
			yield return StartCoroutine(TryGetWebCam());
			yield return StartCoroutine(DoRecognition());
		}

		private IEnumerator TryGetWebCam()
		{
			do
			{
				m_webCam = m_uiTexture.mainTexture as WebCamTexture;
				yield return null;
			}
			while (m_webCam == null);
		}

		private IEnumerator DoRecognition()
		{
			while (true)
			{
				lock (m_mutex)
				{
					EmitPreviousResults();
					ProvideNewData();
				}
				yield return null;
			}
		}

		private void EmitPreviousResults()
		{
			if (m_result != null)
			{
				if (this.CodeScanned != null)
				{
					Debug.Log("Got code: " + m_result.text);
					this.CodeScanned(m_result);
				}
				m_result = null;
			}
		}

		private void ProvideNewData()
		{
			if (m_pixels == null && m_webCam.didUpdateThisFrame)
			{
				m_pixels = m_webCam.GetPixels32();
				m_width = m_webCam.width;
				m_height = m_webCam.height;
			}
		}

		private void WorkerThreadFunc()
		{
			bool flag = true;
			while (flag)
			{
				if (m_pixels != null)
				{
					ZXing.Result result = m_reader.Decode(m_pixels, m_width, m_height);
					Result result2 = null;
					if (result != null)
					{
						Debug.Log(string.Format("Got text \"{0}\"", result.Text));
						result2 = new Result();
						result2.text = result.Text;
					}
					lock (m_mutex)
					{
						m_pixels = null;
						if (result2 != null)
						{
							m_result = result2;
						}
					}
				}
				try
				{
					Thread.Sleep(100);
				}
				catch (ThreadInterruptedException)
				{
					flag = false;
				}
			}
		}

		public void OnDestroy()
		{
			m_workerThread.Interrupt();
			m_workerThread.Join();
		}
	}
}
