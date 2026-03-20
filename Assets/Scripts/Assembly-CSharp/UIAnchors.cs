using UnityEngine;

public class UIAnchors : MonoBehaviour
{
	public int MaxWidth = 640;

	public int MaxHeight = 960;

	public Camera UICamera;

	public Transform Top;

	public Transform TopLeft;

	public Transform TopRight;

	public Transform Bottom;

	public Transform BottomLeft;

	public Transform BottomRight;

	public Transform Left;

	public Transform Right;

	private void Reset()
	{
		if (UICamera == null)
		{
			UICamera = Camera.main;
		}
	}

	private void Awake()
	{
		MaxWidth /= 2;
		MaxHeight /= 2;
	}

	private void Start()
	{
		float num = (float)Screen.width / (float)Screen.height;
		float num2 = UICamera.orthographicSize;
		float num3 = num2 * num;
		if (MaxWidth != 0 && num3 > (float)MaxWidth)
		{
			num3 = MaxWidth;
		}
		if (MaxHeight != 0 && num2 > (float)MaxHeight)
		{
			num2 = MaxHeight;
		}
		if ((bool)Top)
		{
			Top.localPosition = new Vector3(0f, num2, Top.localPosition.z);
		}
		if ((bool)TopLeft)
		{
			TopLeft.localPosition = new Vector3(0f - num3, num2, TopLeft.localPosition.z);
		}
		if ((bool)TopRight)
		{
			TopRight.localPosition = new Vector3(num3, num2, TopRight.localPosition.z);
		}
		if ((bool)Bottom)
		{
			Bottom.localPosition = new Vector3(0f, 0f - num2, Bottom.localPosition.z);
		}
		if ((bool)BottomLeft)
		{
			BottomLeft.localPosition = new Vector3(0f - num3, 0f - num2, BottomLeft.localPosition.z);
		}
		if ((bool)BottomRight)
		{
			BottomRight.localPosition = new Vector3(num3, 0f - num2, BottomRight.localPosition.z);
		}
		if ((bool)Left)
		{
			Left.localPosition = new Vector3(0f - num3, 0f, Left.localPosition.z);
		}
		if ((bool)Right)
		{
			Right.localPosition = new Vector3(num3, 0f, Right.localPosition.z);
		}
	}
}
