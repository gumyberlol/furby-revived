using UnityEngine;

public class ParticleManager : MonoBehaviour
{
	private ParticleSystem[] ParticleComponents;

	private float[] initialValues;

	private bool currentSetting;

	private void Start()
	{
		int num = 0;
		ParticleComponents = GetComponentsInChildren<ParticleSystem>();
		int num2 = base.transform.childCount + 1;
		initialValues = new float[num2];
		ParticleSystem[] particleComponents = ParticleComponents;
		foreach (ParticleSystem particleSystem in particleComponents)
		{
			initialValues[num] = particleSystem.emissionRate;
			num++;
		}
	}

	public void Activator(bool setting)
	{
		currentSetting = setting;
		int num = 0;
		ParticleSystem[] particleComponents = ParticleComponents;
		foreach (ParticleSystem particleSystem in particleComponents)
		{
			if (setting)
			{
				particleSystem.emissionRate = initialValues[num];
				num++;
			}
			else
			{
				particleSystem.emissionRate = 0f;
			}
		}
	}

	public void ChangeEmission(float percent)
	{
		int num = 0;
		ParticleSystem[] particleComponents = ParticleComponents;
		foreach (ParticleSystem particleSystem in particleComponents)
		{
			particleSystem.emissionRate = initialValues[num] * percent / 100f;
			num++;
		}
		if (percent == 0f)
		{
			currentSetting = false;
		}
	}

	public bool CurrentValue()
	{
		return currentSetting;
	}

	public void Remove()
	{
		Object.Destroy(base.gameObject);
	}

	public void ClearParticles()
	{
		ParticleSystem[] particleComponents = ParticleComponents;
		foreach (ParticleSystem particleSystem in particleComponents)
		{
			particleSystem.Clear();
		}
	}
}
