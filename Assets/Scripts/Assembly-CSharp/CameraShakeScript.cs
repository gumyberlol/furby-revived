using UnityEngine;

public class CameraShakeScript : MonoBehaviour
{
	private Vector3 m_OriginalPos;

	private Quaternion m_OriginalRot;

	private bool m_ShakingEnabled;

	private float m_ShakeIntensity;

	private float m_ShakeDecay;

	public Vector3 OriginalPos
	{
		get
		{
			return m_OriginalPos;
		}
		set
		{
			m_OriginalPos = value;
		}
	}

	public Quaternion OriginalRot
	{
		get
		{
			return m_OriginalRot;
		}
		set
		{
			m_OriginalRot = value;
		}
	}

	public float ShakeDecay
	{
		get
		{
			return m_ShakeDecay;
		}
		set
		{
			m_ShakeDecay = value;
		}
	}

	public bool ShakingEnabled
	{
		get
		{
			return m_ShakingEnabled;
		}
		set
		{
			m_ShakingEnabled = value;
		}
	}

	public float ShakeIntensity
	{
		get
		{
			return m_ShakeIntensity;
		}
		set
		{
			m_ShakeIntensity = value;
		}
	}

	private void Start()
	{
		ShakingEnabled = false;
	}

	private void Update()
	{
		if (ShakeIntensity > 0f)
		{
			base.transform.position = new Vector3(OriginalPos.x + Random.insideUnitSphere.x * ShakeIntensity, OriginalPos.y + Random.insideUnitSphere.y * ShakeIntensity, OriginalPos.z);
			ShakeIntensity -= ShakeDecay;
		}
		else if (ShakingEnabled)
		{
			ShakingEnabled = false;
			ReapplyPositionAndRotation();
		}
	}

	public void DoShake(float intensity, float decay)
	{
		CachePositionAndRotation();
		ShakeIntensity = intensity;
		ShakeDecay = decay;
		ShakingEnabled = true;
	}

	private void CachePositionAndRotation()
	{
		OriginalPos = base.transform.position;
		OriginalRot = base.transform.rotation;
	}

	private void ReapplyPositionAndRotation()
	{
		base.transform.position = OriginalPos;
		base.transform.rotation = OriginalRot;
	}
}
