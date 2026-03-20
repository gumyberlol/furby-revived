using Relentless;
using UnityEngine;

namespace Furby
{
	public class XpAwards : ScriptableObject
	{
		[EasyEditArray]
		public XpAwardValue[] XpAwardValues;
	}
}
