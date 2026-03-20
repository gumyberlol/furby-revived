using Relentless;
using UnityEngine;

public class ChangeModelInstance : ChangeLookAndFeel
{
	public GameObject m_modelPrefab;

	protected override void OnChangeTheme()
	{
		ModelInstance component = GetComponent<ModelInstance>();
		component.ModelPrefab = m_modelPrefab;
	}
}
