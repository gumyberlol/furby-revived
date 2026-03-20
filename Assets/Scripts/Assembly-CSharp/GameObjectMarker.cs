using System.Linq;
using UnityEngine;

public class GameObjectMarker : MonoBehaviour
{
	public GameObjectType m_type;

	public static GameObject GetGameObjectByMarker(GameObjectType type)
	{
		GameObjectMarker[] array = Object.FindObjectsOfType(typeof(GameObjectMarker)) as GameObjectMarker[];
		return (array != null) ? (from obj in array
			where obj.m_type == type
			select obj.gameObject).FirstOrDefault() : null;
	}
}
