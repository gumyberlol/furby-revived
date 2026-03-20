using UnityEngine;

public static class Ghost
{
	public static void Disincarnate(GameObject toGhost)
	{
		toGhost.hideFlags = HideFlags.HideInHierarchy;
		toGhost.SetActive(false);
	}

	public static void Incarnate(GameObject fromGhost)
	{
		if (!(fromGhost == null))
		{
			fromGhost.hideFlags = HideFlags.None;
			fromGhost.SetActive(true);
		}
	}
}
