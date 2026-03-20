using System;
using UnityEngine;

public class PlatformSpecifics : MonoBehaviour
{
	[Serializable]
	public class MaterialPerPlatform
	{
		public Platform platform;

		public Material mat;

		public MaterialPerPlatform(Platform _platform, Material _mat)
		{
			platform = _platform;
			mat = _mat;
		}
	}

	[Serializable]
	public class LocalScalePerPlatform
	{
		public Platform platform;

		public Vector3 localScale;

		public LocalScalePerPlatform(Platform _platform, Vector3 _localScale)
		{
			platform = _platform;
			localScale = _localScale;
		}
	}

	[Serializable]
	public class LocalScalePerAspectRatio
	{
		public AspectRatio aspectRatio;

		public Vector3 localScale;

		public LocalScalePerAspectRatio(AspectRatio _aspectRatio, Vector3 _localScale)
		{
			aspectRatio = _aspectRatio;
			localScale = _localScale;
		}
	}

	[Serializable]
	public class LocalPositionPerPlatform
	{
		public Platform platform;

		public Vector3 localPosition;

		public LocalPositionPerPlatform(Platform _platform, Vector3 _localPosition)
		{
			platform = _platform;
			localPosition = _localPosition;
		}
	}

	[Serializable]
	public class LocalPositionPerAspectRatio
	{
		public AspectRatio aspectRatio;

		public Vector3 localPosition;

		public LocalPositionPerAspectRatio(AspectRatio _aspectRatio, Vector3 _localPosition)
		{
			aspectRatio = _aspectRatio;
			localPosition = _localPosition;
		}
	}

	[Serializable]
	public class FontPerPlatform
	{
		public Platform platform;

		public Font font;

		public Material mat;

		public FontPerPlatform(Platform _platform, Font _font, Material _mat)
		{
			platform = _platform;
			font = _font;
			mat = _mat;
		}
	}

	[Serializable]
	public class TextMeshTextPerPlatform
	{
		public Platform platform;

		public string text;

		public TextMeshTextPerPlatform(Platform _platform, string _text)
		{
			platform = _platform;
			text = _text;
		}
	}

	public Platform[] restrictPlatform;

	public MaterialPerPlatform[] materialPerPlatform;

	public LocalScalePerPlatform[] localScalePerPlatform;

	public LocalScalePerAspectRatio[] localScalePerAspectRatio;

	public LocalPositionPerPlatform[] localPositionPerPlatform;

	public LocalPositionPerAspectRatio[] localPositionPerAspectRatio;

	public FontPerPlatform[] fontPerPlatform;

	public TextMeshTextPerPlatform[] textMeshTextPerPlatform;

	private void Awake()
	{
		Init();
		ApplySpecifics(Platforms.platform);
	}

	public void Init()
	{
		if (restrictPlatform == null)
		{
			restrictPlatform = new Platform[0];
		}
		if (materialPerPlatform == null)
		{
			materialPerPlatform = new MaterialPerPlatform[0];
		}
		if (localScalePerPlatform == null)
		{
			localScalePerPlatform = new LocalScalePerPlatform[0];
		}
		if (localScalePerAspectRatio == null)
		{
			localScalePerAspectRatio = new LocalScalePerAspectRatio[0];
		}
		if (localPositionPerPlatform == null)
		{
			localPositionPerPlatform = new LocalPositionPerPlatform[0];
		}
		if (localPositionPerAspectRatio == null)
		{
			localPositionPerAspectRatio = new LocalPositionPerAspectRatio[0];
		}
		if (fontPerPlatform == null)
		{
			fontPerPlatform = new FontPerPlatform[0];
		}
		if (textMeshTextPerPlatform == null)
		{
			textMeshTextPerPlatform = new TextMeshTextPerPlatform[0];
		}
	}

	public void ApplySpecifics(Platform platform)
	{
		ApplySpecifics(platform, true);
	}

	public void ApplySpecifics(Platform platform, bool applyPlatformRestriction)
	{
		if (!applyPlatformRestriction || ApplyRestrictPlatform(platform))
		{
			ApplyMaterial(platform);
			ApplyLocalScale(platform);
			ApplyLocalPosition(platform);
			ApplyFont(platform);
			ApplyTextMeshText(platform);
		}
	}

	public bool ApplyRestrictPlatform(Platform platform)
	{
		if (restrictPlatform != null && restrictPlatform.Length > 0)
		{
			bool flag = false;
			for (int i = 0; i < restrictPlatform.Length; i++)
			{
				if (platform == restrictPlatform[i])
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				if (Application.isEditor)
				{
					UnityEngine.Object.DestroyImmediate(base.gameObject, true);
				}
				else
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
				return false;
			}
			return true;
		}
		return true;
	}

	public void ApplyMaterial(Platform platform)
	{
		if (this.materialPerPlatform == null)
		{
			return;
		}
		MaterialPerPlatform[] array = this.materialPerPlatform;
		foreach (MaterialPerPlatform materialPerPlatform in array)
		{
			if (platform == materialPerPlatform.platform)
			{
				base.renderer.sharedMaterial = materialPerPlatform.mat;
				break;
			}
		}
	}

	public void ApplyLocalScale(Platform platform)
	{
		if (this.localScalePerPlatform != null)
		{
			LocalScalePerPlatform[] array = this.localScalePerPlatform;
			foreach (LocalScalePerPlatform localScalePerPlatform in array)
			{
				if (platform == localScalePerPlatform.platform)
				{
					base.transform.localScale = localScalePerPlatform.localScale;
					break;
				}
			}
		}
		if (!Platforms.IsPlatformAspectBased(platform.ToString()) || this.localScalePerAspectRatio == null)
		{
			return;
		}
		LocalScalePerAspectRatio[] array2 = this.localScalePerAspectRatio;
		foreach (LocalScalePerAspectRatio localScalePerAspectRatio in array2)
		{
			if (AspectRatios.GetAspectRatio() == localScalePerAspectRatio.aspectRatio)
			{
				base.transform.localScale = localScalePerAspectRatio.localScale;
				break;
			}
		}
	}

	public void ApplyLocalPosition(Platform platform)
	{
		if (this.localPositionPerPlatform != null)
		{
			LocalPositionPerPlatform[] array = this.localPositionPerPlatform;
			foreach (LocalPositionPerPlatform localPositionPerPlatform in array)
			{
				if (platform == localPositionPerPlatform.platform)
				{
					base.transform.localPosition = localPositionPerPlatform.localPosition;
					break;
				}
			}
		}
		if (!Platforms.IsPlatformAspectBased(platform.ToString()) || this.localPositionPerAspectRatio == null)
		{
			return;
		}
		LocalPositionPerAspectRatio[] array2 = this.localPositionPerAspectRatio;
		foreach (LocalPositionPerAspectRatio localPositionPerAspectRatio in array2)
		{
			if (AspectRatios.GetAspectRatio() == localPositionPerAspectRatio.aspectRatio)
			{
				base.transform.localPosition = localPositionPerAspectRatio.localPosition;
				break;
			}
		}
	}

	public void ApplyFont(Platform platform)
	{
		if (this.fontPerPlatform == null)
		{
			return;
		}
		FontPerPlatform[] array = this.fontPerPlatform;
		foreach (FontPerPlatform fontPerPlatform in array)
		{
			if (platform == fontPerPlatform.platform)
			{
				GetComponent<TextMesh>().font = fontPerPlatform.font;
				base.renderer.sharedMaterial = fontPerPlatform.mat;
				break;
			}
		}
	}

	public void ApplyTextMeshText(Platform platform)
	{
		if (this.textMeshTextPerPlatform == null)
		{
			return;
		}
		TextMeshTextPerPlatform[] array = this.textMeshTextPerPlatform;
		foreach (TextMeshTextPerPlatform textMeshTextPerPlatform in array)
		{
			if (platform == textMeshTextPerPlatform.platform)
			{
				GetComponent<TextMesh>().text = textMeshTextPerPlatform.text;
				break;
			}
		}
	}
}
