using System.Collections;
using System.Collections.Generic;
using System.IO;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class ThumbGrabber : MonoBehaviour
	{
		[SerializeField]
		private BabyInstance m_babyInstance;

		[SerializeField]
		private FurbyTribeType[] m_tribesToGrab;

		[SerializeField]
		private Camera[] m_cameras;

		[SerializeField]
		private bool m_justFlairs = true;

		[SerializeField]
		private AnimationClip m_animation;

		[SerializeField]
		private string m_outputPath = "D:/tempFurby/";

		private bool m_hasGrabbed;

		private bool m_shouldGrab;

		private string m_currentName = string.Empty;

		private bool m_maskModeFurby = true;

		private Texture2D m_alphaMask;

		private void SetAnimation(BabyInstance babyInstance)
		{
			if (m_animation != null)
			{
				babyInstance.Instance.animation.Play(m_animation.name);
				babyInstance.Instance.animation[m_animation.name].time = 0f;
				babyInstance.Instance.animation[m_animation.name].speed = 0f;
			}
		}

		private IEnumerator WaitForLoad()
		{
			yield return new WaitForSeconds(0.1f);
			while (AssetBundleHelpers.IsLoading())
			{
				yield return null;
			}
			yield return null;
		}

		private IEnumerator Start()
		{
			yield return null;
			FurbyTribeType[] tribesToGrab = m_tribesToGrab;
			foreach (FurbyTribeType tribeToGrab in tribesToGrab)
			{
				foreach (FurbyTribeType.BabyUnlockLevel unlockLevel in tribeToGrab.UnlockLevels)
				{
					FurbyBabyTypeInfo[] babyTypes = unlockLevel.BabyTypes;
					foreach (FurbyBabyTypeInfo typeInfo in babyTypes)
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
						yield return StartCoroutine(WaitForLoad());
						SetAnimation(m_babyInstance);
						yield return null;
						if (m_alphaMask == null)
						{
							base.camera.backgroundColor = new Color(1f, 0f, 1f, 0f);
							yield return new WaitForSeconds(2f);
							Logging.Log("No mask created yet");
							m_hasGrabbed = false;
							m_shouldGrab = true;
							while (!m_hasGrabbed)
							{
								yield return null;
							}
							base.camera.backgroundColor = new Color(0f, 0f, 0f, 0f);
							yield return new WaitForSeconds(0.5f);
						}
						m_currentName = typeInfo.GetColoringAssetBundleName();
						m_hasGrabbed = false;
						m_shouldGrab = true;
						while (!m_hasGrabbed)
						{
							yield return null;
						}
						yield return new WaitForSeconds(0.2f);
						if (m_justFlairs)
						{
							break;
						}
					}
					if (m_justFlairs)
					{
						break;
					}
				}
				if (m_justFlairs)
				{
					break;
				}
			}
			m_maskModeFurby = false;
			base.camera.backgroundColor = new Color(1f, 0f, 1f, 0f);
			FurbyTribeType[] tribesToGrab2 = m_tribesToGrab;
			int num = 0;
			if (num >= tribesToGrab2.Length)
			{
				yield break;
			}
			FurbyTribeType tribeToGrab2 = tribesToGrab2[num];
			using (IEnumerator<FurbyTribeType.BabyUnlockLevel> enumerator2 = tribeToGrab2.UnlockLevels.GetEnumerator())
			{
				if (!enumerator2.MoveNext())
				{
					yield break;
				}
				FurbyTribeType.BabyUnlockLevel unlockLevel2 = enumerator2.Current;
				FurbyBabyTypeInfo[] babyTypes2 = unlockLevel2.BabyTypes;
				int num2 = 0;
				if (num2 >= babyTypes2.Length)
				{
					yield break;
				}
				FurbyBabyTypeInfo typeInfo2 = babyTypes2[num2];
				foreach (Flair flair in FurbyGlobals.FlairLibrary.Collection.Flairs)
				{
					FurbyBaby newBaby2 = FurbyGlobals.BabyRepositoryHelpers.CreateNewBaby(typeInfo2.TypeID);
					newBaby2.Progress = FurbyBabyProgresss.P;
					newBaby2.SetPersonality(FurbyBabyPersonality.None, 0);
					if (m_babyInstance.Instance != null)
					{
						Object.Destroy(m_babyInstance.Instance);
					}
					yield return new WaitForSeconds(0.2f);
					m_babyInstance.SetTargetFurbyBaby(newBaby2);
					m_babyInstance.InstantiateObject();
					m_babyInstance.Instance.animation.Stop();
					yield return StartCoroutine(WaitForLoad());
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
					JiggleBone[] jigglers = flairObject.GetComponentsInChildren<JiggleBone>();
					JiggleBone[] array = jigglers;
					foreach (JiggleBone jiggler in array)
					{
						jiggler.enabled = false;
					}
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
					SetAnimation(m_babyInstance);
					yield return null;
					m_currentName = "FLAIR_" + flair.Name;
					m_hasGrabbed = false;
					m_shouldGrab = true;
					while (!m_hasGrabbed)
					{
						yield return null;
					}
					yield return new WaitForSeconds(0.2f);
					Object.Destroy(flairObject);
				}
			}
		}

		private void OnPostRender()
		{
			if (!m_shouldGrab)
			{
				return;
			}
			if (m_alphaMask == null)
			{
				Logging.Log("Creating mask");
				m_alphaMask = new Texture2D(base.camera.targetTexture.width, base.camera.targetTexture.height);
				m_alphaMask.ReadPixels(new Rect(0f, 0f, base.camera.targetTexture.width, base.camera.targetTexture.height), 0, 0);
				m_alphaMask.Apply();
			}
			else
			{
				Logging.Log("Creating " + m_currentName);
				Texture2D texture2D = new Texture2D(base.camera.targetTexture.width, base.camera.targetTexture.height);
				texture2D.ReadPixels(new Rect(0f, 0f, base.camera.targetTexture.width, base.camera.targetTexture.height), 0, 0);
				texture2D.Apply();
				if (m_maskModeFurby)
				{
					for (int i = 0; i < base.camera.targetTexture.width; i++)
					{
						for (int j = 0; j < base.camera.targetTexture.height; j++)
						{
							Color pixel = texture2D.GetPixel(i, j);
							Color pixel2 = m_alphaMask.GetPixel(i, j);
							float a = ((pixel2.r != 1f || pixel2.g != 0f || pixel2.b != 1f) ? 1 : 0);
							texture2D.SetPixel(i, j, new Color(pixel.r, pixel.g, pixel.b, a));
						}
					}
					texture2D.Apply();
				}
				else
				{
					for (int k = 0; k < base.camera.targetTexture.width; k++)
					{
						for (int l = 0; l < base.camera.targetTexture.height; l++)
						{
							Color pixel3 = texture2D.GetPixel(k, l);
							Color pixel4 = texture2D.GetPixel(k, l);
							float a2 = ((pixel4.r != 1f || pixel4.g != 0f || pixel4.b != 1f) ? 1 : 0);
							texture2D.SetPixel(k, l, new Color(pixel3.r, pixel3.g, pixel3.b, a2));
						}
					}
					texture2D.Apply();
				}
				byte[] bytes = texture2D.EncodeToPNG();
				Directory.CreateDirectory(m_outputPath);
				File.WriteAllBytes(m_outputPath + m_currentName + ".png", bytes);
			}
			m_shouldGrab = false;
			m_hasGrabbed = true;
			Resources.UnloadUnusedAssets();
		}
	}
}
