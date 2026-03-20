using UnityEngine;

namespace Furby
{
	public class CamAdjustManager : MonoBehaviour
	{
		public float bgZoom;

		public Vector3 bgAdjust;

		public float perspZoom;

		public Vector3 perspAdjust;

		public float otherZoom;

		public Vector3 otherAdjust;

		private Vector3 origin = new Vector3(0f, 0f, 0f);

		public CameraAdjust bgCamera;

		public CameraAdjust perspCamera;

		public CameraAdjust otherCamera;

		public void Adjust()
		{
			bgCamera.ZoomAdj(bgZoom, bgAdjust);
			perspCamera.ZoomAdj(perspZoom, perspAdjust);
			otherCamera.ZoomAdj(otherZoom, otherAdjust);
		}

		public void Reset()
		{
			bgCamera.ZoomAdj(1f, origin);
			perspCamera.ZoomAdj(1f, origin);
			otherCamera.ZoomAdj(1f, origin);
		}
	}
}
