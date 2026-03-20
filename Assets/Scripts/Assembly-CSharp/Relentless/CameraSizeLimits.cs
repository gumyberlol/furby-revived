using UnityEngine;

namespace Relentless
{
	public class CameraSizeLimits : MonoBehaviour
	{
		public enum Scope
		{
			LimitJustThisObject = 0,
			LimitAllCameras = 1
		}

		private static Camera s_blackBarCam;

		public float MaxRatio = 0.75f;

		public float MinRatio = 2f / 3f;

		public Scope LimitScope;

		private void Awake()
		{
			if (base.camera != null && LimitScope == Scope.LimitJustThisObject)
			{
				LimitCameraSize(base.camera);
			}
			else
			{
				OnLevelWasLoaded(0);
			}
		}

		private void OnLevelWasLoaded(int level)
		{
			if (LimitScope == Scope.LimitAllCameras)
			{
				Camera[] allCameras = Camera.allCameras;
				foreach (Camera cam in allCameras)
				{
					LimitCameraSize(cam);
				}
				if (s_blackBarCam == null)
				{
					AddBlackBarCam();
				}
			}
		}

		private void AddBlackBarCam()
		{
			GameObject gameObject = new GameObject("Black Bars");
			Object.DontDestroyOnLoad(gameObject);
			Camera camera = (s_blackBarCam = gameObject.AddComponent<Camera>());
			camera.depth = -100f;
			camera.cullingMask = 0;
			camera.clearFlags = CameraClearFlags.Color;
			camera.backgroundColor = Color.black;
			camera.rect = new Rect(0f, 0f, 1f, 1f);
		}

		public void LimitCameraSize(Camera cam)
		{
			bool flag = cam.targetTexture != null;
			bool flag2 = cam == s_blackBarCam;
			if (!flag && !flag2)
			{
				float num = (float)Screen.width / (float)Screen.height;
				cam.rect = new Rect(0f, 0f, 1f, 1f);
				if (num < MinRatio)
				{
					float num2 = num / MinRatio;
					cam.aspect = MinRatio;
					Rect rect = cam.rect;
					rect.height = num2;
					rect.y = (1f - num2) / 2f;
					cam.rect = rect;
				}
				else if (num > MaxRatio)
				{
					float num3 = MaxRatio / num;
					cam.aspect = MaxRatio;
					Rect rect2 = cam.rect;
					rect2.width = num3;
					rect2.x = (1f - num3) / 2f;
					cam.rect = rect2;
				}
			}
		}
	}
}
