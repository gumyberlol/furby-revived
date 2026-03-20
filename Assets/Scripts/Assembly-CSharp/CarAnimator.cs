using SplineUtilities;
using UnityEngine;

public class CarAnimator : MonoBehaviour
{
	public Spline spline;

	public WrapMode wrapMode = WrapMode.Once;

	public float speed = 1f;

	private float speedInit = 1f;

	public float passedTime;

	public float rotationOffset;

	private void Start()
	{
		speedInit = speed;
	}

	private void Update()
	{
		passedTime += Time.deltaTime * speed;
		float param = SplineUtils.WrapValue(passedTime, 0f, 1f, wrapMode);
		base.transform.rotation = spline.GetOrientationOnSpline(SplineUtils.WrapValue(passedTime + rotationOffset, 0f, 1f, wrapMode));
		base.transform.position = spline.GetPositionOnSpline(param);
		speed = speedInit * spline.GetCustomValueOnSpline(param);
	}
}
