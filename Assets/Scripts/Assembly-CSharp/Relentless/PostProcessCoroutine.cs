using System.Collections;
using UnityEngine;

namespace Relentless
{
	public class PostProcessCoroutine : MonoBehaviour
	{
		public delegate IEnumerator PostProcessDelegate(PostProcessCoroutine source);

		private RenderTexture m_source;

		private RenderTexture m_destination;

		private IEnumerator m_work;

		private bool m_finished;

		public RenderTexture Source
		{
			get
			{
				return m_source;
			}
		}

		public RenderTexture Destination
		{
			get
			{
				return m_destination;
			}
		}

		private void Awake()
		{
			base.enabled = false;
		}

		public IEnumerator PostProcess(IEnumerator work)
		{
			base.enabled = true;
			m_work = work;
			m_finished = false;
			while (!m_finished)
			{
				yield return null;
			}
			base.enabled = false;
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			RenderTexture renderTexture = RenderTexture.active;
			m_source = source;
			m_destination = destination;
			if (!m_finished)
			{
				m_finished = !m_work.MoveNext();
				object current = m_work.Current;
			}
			else
			{
				Graphics.Blit(source, destination);
			}
			RenderTexture.active = renderTexture;
		}
	}
}
