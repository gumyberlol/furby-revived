using Relentless;
using UnityEngine;

[RequireComponent(typeof(TransformTracker))]
public class SetTransformTracker : MonoBehaviour
{
	public GameObjectType m_type;

	private void Awake()
	{
		TransformTracker component = GetComponent<TransformTracker>();
		GameObject gameObjectByMarker = GameObjectMarker.GetGameObjectByMarker(m_type);
		if ((bool)gameObjectByMarker)
		{
			component.Target = gameObjectByMarker.transform;
		}
	}
}
