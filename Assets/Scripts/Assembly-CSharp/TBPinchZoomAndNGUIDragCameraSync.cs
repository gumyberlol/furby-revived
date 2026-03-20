using UnityEngine;

public class TBPinchZoomAndNGUIDragCameraSync : MonoBehaviour
{
	public UIDraggableCamera m_draggableCamera;

	public TBPinchZoom m_pinchZoom;

	private void OnEnable()
	{
		FingerGestures.OnPinchMove += FingerGestures_OnPinchMove;
	}

	private void OnDisable()
	{
		FingerGestures.OnPinchMove -= FingerGestures_OnPinchMove;
	}

	private void FingerGestures_OnPinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta)
	{
		m_draggableCamera.ConstrainToBounds(true);
		float num = 1f - m_pinchZoom.ZoomAmount;
		Vector2 scale = new Vector2(num, num);
		m_draggableCamera.scale = scale;
	}
}
