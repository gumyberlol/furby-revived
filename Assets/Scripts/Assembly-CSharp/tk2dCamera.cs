using System;
using UnityEngine;

[AddComponentMenu("2D Toolkit/Camera/tk2dCamera")]
[ExecuteInEditMode]
public class tk2dCamera : MonoBehaviour
{
	public tk2dCameraResolutionOverride[] resolutionOverride;

	private tk2dCameraResolutionOverride currentResolutionOverride;

	public int nativeResolutionWidth = 960;

	public int nativeResolutionHeight = 640;

	public bool enableResolutionOverrides = true;

	[HideInInspector]
	public Camera mainCamera;

	public static tk2dCamera inst;

	[NonSerialized]
	public float orthoSize = 1f;

	private Vector2 _targetResolution = Vector2.zero;

	private Vector2 _scaledResolution = Vector2.zero;

	private Vector2 _screenOffset = Vector2.zero;

	[HideInInspector]
	public bool forceResolutionInEditor;

	[HideInInspector]
	public Vector2 forceResolution = new Vector2(960f, 640f);

	private Rect _screenExtents;

	public Vector2 ScaledResolution
	{
		get
		{
			return _scaledResolution;
		}
	}

	public Rect ScreenExtents
	{
		get
		{
			return _screenExtents;
		}
	}

	public Vector2 ScreenOffset
	{
		get
		{
			return _screenOffset;
		}
	}

	[Obsolete]
	public Vector2 resolution
	{
		get
		{
			return ScaledResolution;
		}
	}

	public Vector2 TargetResolution
	{
		get
		{
			return _targetResolution;
		}
	}

	private void Awake()
	{
		mainCamera = GetComponent<Camera>();
		if (mainCamera != null)
		{
			UpdateCameraMatrix();
		}
		inst = this;
	}

	private void LateUpdate()
	{
		UpdateCameraMatrix();
	}

	public void UpdateCameraMatrix()
	{
		inst = this;
		float pixelWidth = mainCamera.pixelWidth;
		float pixelHeight = mainCamera.pixelHeight;
		_targetResolution = new Vector2(pixelWidth, pixelHeight);
		if (!enableResolutionOverrides)
		{
			currentResolutionOverride = null;
		}
		if (enableResolutionOverrides && (currentResolutionOverride == null || (currentResolutionOverride != null && ((float)currentResolutionOverride.width != pixelWidth || (float)currentResolutionOverride.height != pixelHeight))))
		{
			currentResolutionOverride = null;
			if (resolutionOverride != null)
			{
				tk2dCameraResolutionOverride[] array = resolutionOverride;
				foreach (tk2dCameraResolutionOverride tk2dCameraResolutionOverride2 in array)
				{
					if (tk2dCameraResolutionOverride2.Match((int)pixelWidth, (int)pixelHeight))
					{
						currentResolutionOverride = tk2dCameraResolutionOverride2;
						break;
					}
				}
			}
		}
		float num;
		Vector2 screenOffset;
		if (currentResolutionOverride == null)
		{
			num = 1f;
			screenOffset = new Vector2(0f, 0f);
		}
		else
		{
			switch (currentResolutionOverride.autoScaleMode)
			{
			case tk2dCameraResolutionOverride.AutoScaleMode.FitHeight:
				num = pixelHeight / (float)nativeResolutionHeight;
				break;
			case tk2dCameraResolutionOverride.AutoScaleMode.FitWidth:
				num = pixelWidth / (float)nativeResolutionWidth;
				break;
			case tk2dCameraResolutionOverride.AutoScaleMode.FitVisible:
			{
				float num2 = (float)nativeResolutionWidth / (float)nativeResolutionHeight;
				float num3 = pixelWidth / pixelHeight;
				num = ((!(num3 < num2)) ? (pixelHeight / (float)nativeResolutionHeight) : (pixelWidth / (float)nativeResolutionWidth));
				break;
			}
			default:
				num = currentResolutionOverride.scale;
				break;
			}
			tk2dCameraResolutionOverride.FitMode fitMode = currentResolutionOverride.fitMode;
			screenOffset = ((fitMode == tk2dCameraResolutionOverride.FitMode.Constant || fitMode != tk2dCameraResolutionOverride.FitMode.Center) ? (-currentResolutionOverride.offsetPixels) : new Vector2(Mathf.Round(((float)nativeResolutionWidth * num - pixelWidth) / 2f), Mathf.Round(((float)nativeResolutionHeight * num - pixelHeight) / 2f)));
		}
		float x = screenOffset.x;
		float y = screenOffset.y;
		float num4 = pixelWidth + screenOffset.x;
		float num5 = pixelHeight + screenOffset.y;
		_screenExtents.Set(x / num, num5 / num, (num4 - x) / num, (y - num5) / num);
		float farClipPlane = mainCamera.farClipPlane;
		float nearClipPlane = mainCamera.nearClipPlane;
		orthoSize = (num5 - y) / 2f;
		_scaledResolution = new Vector2(num4 / num, num5 / num);
		_screenOffset = screenOffset;
		bool flag = false;
		float num6 = ((Application.platform != RuntimePlatform.WindowsPlayer && Application.platform != RuntimePlatform.WindowsWebPlayer && Application.platform != RuntimePlatform.WindowsEditor) ? 0f : 1f);
		float value = 2f / (num4 - x) * num;
		float value2 = 2f / (num5 - y) * num;
		float value3 = -2f / (farClipPlane - nearClipPlane);
		float value4 = (0f - (num4 + x + num6)) / (num4 - x);
		float value5 = (0f - (y + num5 - num6)) / (num5 - y);
		float value6 = (0f - 2f * farClipPlane * nearClipPlane) / (farClipPlane - nearClipPlane);
		Matrix4x4 projectionMatrix = default(Matrix4x4);
		projectionMatrix[0, 0] = value;
		projectionMatrix[0, 1] = 0f;
		projectionMatrix[0, 2] = 0f;
		projectionMatrix[0, 3] = value4;
		projectionMatrix[1, 0] = 0f;
		projectionMatrix[1, 1] = value2;
		projectionMatrix[1, 2] = 0f;
		projectionMatrix[1, 3] = value5;
		projectionMatrix[2, 0] = 0f;
		projectionMatrix[2, 1] = 0f;
		projectionMatrix[2, 2] = value3;
		projectionMatrix[2, 3] = value6;
		projectionMatrix[3, 0] = 0f;
		projectionMatrix[3, 1] = 0f;
		projectionMatrix[3, 2] = 0f;
		projectionMatrix[3, 3] = 1f;
		mainCamera.projectionMatrix = projectionMatrix;
	}
}
