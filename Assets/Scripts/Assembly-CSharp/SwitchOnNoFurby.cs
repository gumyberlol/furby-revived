using Furby;
using UnityEngine;

public class SwitchOnNoFurby : MonoBehaviour
{
	[SerializeField]
	private Transform m_furbyModeBranch;

	[SerializeField]
	private Transform m_noFurbyModeBranch;

	private void OnEnable()
	{
		Transform transform = ((!FurbyGlobals.Player.NoFurbyOnSaveGame()) ? m_furbyModeBranch : m_noFurbyModeBranch);
		DoIt(transform, m_noFurbyModeBranch);
		DoIt(transform, m_furbyModeBranch);
	}

	private void DoIt(Transform enabled, Transform t)
	{
		t.gameObject.SetActive(t == enabled);
	}
}
