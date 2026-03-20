using UnityEngine;

namespace Furby.MiniGames.HideAndSeek
{
	public class HexGridPosition : MonoBehaviour
	{
		public Vector2 HexPosition;

		public float GetDistance(HexGridPosition other)
		{
			float num = other.HexPosition.x - HexPosition.x;
			float num2 = other.HexPosition.y - HexPosition.y;
			if (Mathf.Sign(num) == Mathf.Sign(num2))
			{
				return Mathf.Abs(num + num2);
			}
			return Mathf.Max(Mathf.Abs(num), Mathf.Abs(num2));
		}
	}
}
