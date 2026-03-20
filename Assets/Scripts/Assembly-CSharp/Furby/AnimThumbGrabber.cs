using System.Collections;
using System.IO;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class AnimThumbGrabber : MonoBehaviour
	{
		[SerializeField]
		private BabyInstance m_babyInstance;

		[SerializeField]
		private FurbyBabyTypeInfo[] m_furbyBabies;

		[SerializeField]
		private AnimationClip[] m_animationsToRender;

		[SerializeField]
		private string m_pathToExportTo = "D:\\tempFurby\\";

		[SerializeField]
		private bool m_renderFurblings = true;

		[SerializeField]
		private bool m_renderFlairs = true;

		private bool m_hasGrabbed;

		private bool m_shouldGrab;

		private string m_currentName = string.Empty;

		private Texture2D m_texToSave;

		private IEnumerator Start()
		{
			yield return null;
			m_texToSave = new Texture2D(base.camera.targetTexture.width, base.camera.targetTexture.height, TextureFormat.RGB24, false);
			if (m_renderFurblings)
			{
				FurbyBabyTypeInfo[] furbyBabies = m_furbyBabies;
				foreach (FurbyBabyTypeInfo typeInfo in furbyBabies)
				{
					FurbyBaby newBaby = FurbyGlobals.BabyRepositoryHelpers.CreateNewBaby(typeInfo.TypeID);
					newBaby.Progress = FurbyBabyProgresss.P;
					newBaby.SetPersonality(FurbyBabyPersonality.None, 0);
					if (m_babyInstance.Instance != null)
					{
						Object.Destroy(m_babyInstance.Instance);
					}
					yield return new WaitForSeconds(0.2f);
					m_babyInstance.SetTargetFurbyBaby(newBaby);
					m_babyInstance.InstantiateObject();
					m_babyInstance.Instance.animation.Stop();
					yield return new WaitForSeconds(1f);
					AnimationClip[] animationsToRender = m_animationsToRender;
					foreach (AnimationClip anim in animationsToRender)
					{
						m_babyInstance.Instance.animation.Stop();
						m_babyInstance.Instance.animation.Play(anim.name);
						m_babyInstance.Instance.animation[anim.name].speed = 0f;
						int frames = (int)(anim.length * anim.frameRate);
						for (int frameIndex = 0; frameIndex < frames; frameIndex++)
						{
							m_babyInstance.Instance.animation[anim.name].normalizedTime = (float)frameIndex / (float)frames;
							m_currentName = typeInfo.GetColoringAssetBundleName() + "_" + anim.name + "_" + string.Format("{0:000}", frameIndex);
							m_hasGrabbed = false;
							m_shouldGrab = true;
							while (!m_hasGrabbed)
							{
								yield return null;
							}
						}
					}
					yield return new WaitForSeconds(0.2f);
				}
			}
			if (m_renderFlairs)
			{
				StartCoroutine(CaptureFlairs());
			}
		}

		private IEnumerator CaptureFlairs()
		{
			FurbyBaby newBaby = FurbyGlobals.BabyRepositoryHelpers.CreateNewBaby(m_furbyBabies[0].TypeID);
			newBaby.Progress = FurbyBabyProgresss.P;
			newBaby.SetPersonality(FurbyBabyPersonality.None, 0);
			if (m_babyInstance.Instance != null)
			{
				Object.Destroy(m_babyInstance.Instance);
			}
			yield return new WaitForSeconds(0.2f);
			m_babyInstance.SetTargetFurbyBaby(newBaby);
			m_babyInstance.InstantiateObject();
			m_babyInstance.Instance.animation.Stop();
			yield return new WaitForSeconds(2f);
			Renderer[] componentsInChildren = m_babyInstance.Instance.GetComponentsInChildren<Renderer>(true);
			foreach (Renderer renderer in componentsInChildren)
			{
				if (renderer.name.Contains("fins") || renderer.name.Contains("main"))
				{
					renderer.enabled = false;
				}
				Material[] materials = renderer.materials;
				foreach (Material mat in materials)
				{
					mat.shader = Shader.Find("Unlit/Depth Cutout");
				}
			}
			yield return new WaitForSeconds(1f);
			foreach (Flair flair in FurbyGlobals.FlairLibrary.Collection.Flairs)
			{
				Logging.Log("Instantiating flair " + flair);
				FlairLibrary.PrefabLoader loader = FurbyGlobals.FlairLibrary.GetPrefabLoader(flair.Name);
				GameObject rootObject = m_babyInstance.Instance.GetChildGameObject(loader.Flair.AttachNode);
				GameObject trackObject = new GameObject();
				trackObject.transform.parent = m_babyInstance.Instance.transform;
				trackObject.transform.localPosition = Vector3.zero;
				trackObject.transform.localScale = Vector3.one;
				trackObject.transform.localRotation = Quaternion.identity;
				trackObject.transform.parent = rootObject.transform;
				yield return StartCoroutine(loader.Load(base.gameObject));
				GameObject flairObject = (GameObject)Object.Instantiate(loader.Prefab);
				if (rootObject != null)
				{
					flairObject.transform.parent = trackObject.transform;
					flairObject.SetLayerInChildren(base.gameObject.layer);
					flairObject.transform.localScale = Vector3.one;
					flairObject.transform.localPosition = Vector3.zero;
					flairObject.transform.localRotation = Quaternion.identity;
				}
				Renderer[] componentsInChildren2 = flairObject.GetComponentsInChildren<Renderer>(true);
				foreach (Renderer renderer2 in componentsInChildren2)
				{
					renderer2.enabled = true;
				}
				yield return new WaitForSeconds(1f);
				AnimationClip[] animationsToRender = m_animationsToRender;
				foreach (AnimationClip anim in animationsToRender)
				{
					m_babyInstance.Instance.animation.Stop();
					m_babyInstance.Instance.animation.Play(anim.name);
					m_babyInstance.Instance.animation[anim.name].speed = 0f;
					int frames = (int)(anim.length * anim.frameRate);
					for (int frameIndex = 0; frameIndex < frames; frameIndex++)
					{
						m_babyInstance.Instance.animation[anim.name].normalizedTime = (float)frameIndex / (float)frames;
						m_currentName = "FLAIR_" + flair.Name + "_" + anim.name + "_" + string.Format("{0:000}", frameIndex);
						m_hasGrabbed = false;
						m_shouldGrab = true;
						while (!m_hasGrabbed)
						{
							yield return null;
						}
					}
				}
				while (!m_hasGrabbed)
				{
					yield return null;
				}
				yield return new WaitForSeconds(1f);
				Object.Destroy(flairObject);
			}
		}

		private void OnPostRender()
		{
			if (m_shouldGrab)
			{
				Logging.Log("Creating " + m_currentName);
				m_texToSave.ReadPixels(new Rect(0f, 0f, base.camera.targetTexture.width, base.camera.targetTexture.height), 0, 0);
				m_texToSave.Apply();
				byte[] bytes = m_texToSave.EncodeToPNG();
				Directory.CreateDirectory(m_pathToExportTo);
				File.WriteAllBytes(m_pathToExportTo + m_currentName + ".png", bytes);
				Resources.UnloadUnusedAssets();
				m_shouldGrab = false;
				m_hasGrabbed = true;
			}
		}
	}
}
