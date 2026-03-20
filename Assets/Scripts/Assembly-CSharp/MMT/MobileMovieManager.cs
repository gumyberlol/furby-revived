using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MMT
{
	public class MobileMovieManager : MonoBehaviour
	{
		private enum GfxDeviceRenderer
		{
			kGfxRendererOpenGL = 0,
			kGfxRendererD3D9 = 1,
			kGfxRendererD3D11 = 2,
			kGfxRendererGCM = 3,
			kGfxRendererNull = 4,
			kGfxRendererHollywood = 5,
			kGfxRendererXenon = 6,
			kGfxRendererOpenGLES = 7,
			kGfxRendererOpenGLES20Mobile = 8,
			kGfxRendererMolehill = 9,
			kGfxRendererOpenGLES20Desktop = 10,
			kGfxRendererCount = 11
		}

		private enum GfxDeviceEventType
		{
			kGfxDeviceEventInitialize = 0,
			kGfxDeviceEventShutdown = 1,
			kGfxDeviceEventBeforeReset = 2,
			kGfxDeviceEventAfterReset = 3
		}

		private const string PLATFORM_DLL = "theorawrapper";

		public static MobileMovieManager Instance;

		[DllImport("theorawrapper")]
		private static extern void UnityRenderEvent(int eventID);

		[DllImport("theorawrapper")]
		private static extern void UnitySetGraphicsDevice(IntPtr device, int deviceType, int eventType);

		private void Awake()
		{
			Instance = this;
			UnitySetGraphicsDevice(IntPtr.Zero, 8, 0);
			GL.InvalidateState();
		}

		private void OnEnable()
		{
			StartCoroutine(DecodeCoroutine());
		}

		private void OnDisable()
		{
			StopCoroutine("DecodeCoroutine");
		}

		private IEnumerator DecodeCoroutine()
		{
			while (true)
			{
				yield return new WaitForEndOfFrame();
				UnityRenderEvent(7);
				GL.InvalidateState();
			}
		}
	}
}
