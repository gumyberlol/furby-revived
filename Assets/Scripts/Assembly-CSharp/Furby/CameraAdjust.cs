using UnityEngine;

namespace Furby
{
	public class CameraAdjust : MonoBehaviour
	{
		private bool ortho;

		private Camera cam;

		public GameObject adjustedPanel;

		public float transition;

		private float zoomT;

		private float zoom;

		private float originalFOV;

		private float zTarg;

		private float previousTarg;

		private float adjT;

		private Vector3 adjust;

		private Vector3 adjustTarg;

		private Vector3 previousAdjust;

		private bool first;

		public PlayMakerFSM FSM;

		private void Start()
		{
			cam = GetComponent<Camera>();
			ortho = cam.orthographic;
			adjust = new Vector3(0f, 0f, 0f);
			adjustTarg = cam.transform.position;
			previousAdjust = cam.transform.position;
			if (ortho)
			{
				originalFOV = 1f;
				zTarg = 1f;
				previousTarg = 1f;
			}
			else
			{
				originalFOV = cam.fieldOfView;
				zTarg = cam.fieldOfView;
				previousTarg = cam.fieldOfView;
			}
		}

		private void Update()
		{
			if (zoomT < transition)
			{
				zoomT += Time.deltaTime;
			}
			else if (first)
			{
				FSM.SendEvent("ZoomComplete");
				first = false;
			}
			if (ortho)
			{
				zoom = Mathf.Lerp(previousTarg, zTarg, zoomT / transition);
				cam.orthographicSize = zoom;
			}
			else
			{
				zoom = Mathf.Lerp(previousTarg, zTarg, zoomT / transition);
				cam.fieldOfView = zoom;
			}
			adjT += Time.deltaTime;
			adjust = Vector3.Lerp(previousAdjust, adjustTarg, adjT / transition);
			adjustedPanel.transform.position = adjust;
		}

		public void Zoom(float zoomTarget)
		{
			zoomT = 0f;
			previousTarg = zTarg;
			zTarg = zoomTarget * originalFOV;
			first = true;
		}

		public void Adjustment(Vector3 adj)
		{
			adjT = 0f;
			previousAdjust = adjustTarg;
			adj.z += cam.transform.position.z;
			adjustTarg = adj;
		}

		public void ZoomAdj(float zoomTarget, Vector3 adj)
		{
			Zoom(zoomTarget);
			Adjustment(adj);
		}
	}
}
