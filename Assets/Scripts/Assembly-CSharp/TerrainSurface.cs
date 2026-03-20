using UnityEngine;

public class TerrainSurface
{
	public static float[] GetTextureMix(Vector3 worldPos)
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		TerrainData terrainData = activeTerrain.terrainData;
		Vector3 position = activeTerrain.transform.position;
		int x = (int)((worldPos.x - position.x) / terrainData.size.x * (float)terrainData.alphamapWidth);
		int y = (int)((worldPos.z - position.z) / terrainData.size.z * (float)terrainData.alphamapHeight);
		float[,,] alphamaps = terrainData.GetAlphamaps(x, y, 1, 1);
		float[] array = new float[alphamaps.GetUpperBound(2) + 1];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = alphamaps[0, 0, i];
		}
		return array;
	}

	public static int GetMainTexture(Vector3 worldPos)
	{
		float[] textureMix = GetTextureMix(worldPos);
		float num = 0f;
		int result = 0;
		for (int i = 0; i < textureMix.Length; i++)
		{
			if (textureMix[i] > num)
			{
				result = i;
				num = textureMix[i];
			}
		}
		return result;
	}
}
