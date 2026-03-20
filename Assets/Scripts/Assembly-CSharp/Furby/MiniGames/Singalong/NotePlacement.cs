using Relentless;
using UnityEngine;

namespace Furby.MiniGames.Singalong
{
	public class NotePlacement : Singleton<NotePlacement>
	{
		[SerializeField]
		private Transform[] m_roots;

		public Transform GetRoot(int index)
		{
			return m_roots[index];
		}
	}
}
