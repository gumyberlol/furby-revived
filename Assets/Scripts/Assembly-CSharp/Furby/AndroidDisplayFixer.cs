using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	[RequireComponent(typeof(PostProcessCoroutine))]
	public class AndroidDisplayFixer : MonoBehaviour
	{
		[SerializeField]
		private Shader m_shader;

		private IEnumerator Start()
		{
			if (FurbyGlobals.DeviceSettings.DeviceProperties.m_ApplicationModifiers.m_RequiresDisplayFixer)
			{
				base.camera.enabled = true;
				PostProcessCoroutine ppc = GetComponent<PostProcessCoroutine>();
				yield return StartCoroutine(ppc.PostProcess(BlitAndFixAlpha(ppc)));
			}
		}

		private IEnumerator BlitAndFixAlpha(PostProcessCoroutine postProcess)
		{
			Material mat = new Material(m_shader);
			while (true)
			{
				Blit(postProcess.Source, postProcess.Destination, Color.white, mat);
				yield return null;
			}
		}

		private void Blit(RenderTexture source, RenderTexture destination, Color col, Material mat)
		{
			mat.SetColor("_Color", col);
			Graphics.Blit(source, destination, mat);
		}
	}
}
