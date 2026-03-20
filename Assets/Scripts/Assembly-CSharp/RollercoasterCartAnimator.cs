using Relentless;
using SplineUtilities;
using UnityEngine;

public class RollercoasterCartAnimator : MonoBehaviour
{
	[GameEventEnum]
	public enum RollerCoasterEvents
	{
		RollercoasterStartedAnimation = 0
	}

	public Spline m_spline;

	public Transform[] m_trailingCartTransforms;

	public float m_trailingCartOffset = 0.03f;

	public WrapMode m_wrapMode = WrapMode.Once;

	public float m_speed = 1f;

	private float m_speedInit = 1f;

	public float m_passedTime;

	private void Start()
	{
		m_speedInit = m_speed;
		GameEventRouter.SendEvent(RollerCoasterEvents.RollercoasterStartedAnimation);
	}

	private void Update()
	{
		float num = SplineUtils.WrapValue(m_passedTime, 0f, 1f, m_wrapMode);
		m_passedTime += Time.deltaTime * m_speed;
		float num2 = SplineUtils.WrapValue(m_passedTime, 0f, 1f, m_wrapMode);
		if (num > num2)
		{
			GameEventRouter.SendEvent(RollerCoasterEvents.RollercoasterStartedAnimation);
		}
		base.transform.rotation = m_spline.GetOrientationOnSpline(SplineUtils.WrapValue(m_passedTime, 0f, 1f, m_wrapMode));
		base.transform.position = m_spline.GetPositionOnSpline(num2);
		for (int i = 0; i < m_trailingCartTransforms.Length; i++)
		{
			float v = m_passedTime - m_trailingCartOffset * (1f * (float)(i + 1));
			float param = SplineUtils.WrapValue(v, 0f, 1f, m_wrapMode);
			m_trailingCartTransforms[i].rotation = m_spline.GetOrientationOnSpline(SplineUtils.WrapValue(v, 0f, 1f, m_wrapMode));
			m_trailingCartTransforms[i].position = m_spline.GetPositionOnSpline(param);
		}
		m_speed = m_speedInit * m_spline.GetCustomValueOnSpline(num2);
	}
}
