using Relentless;
using UnityEngine;

namespace Furby
{
	public class CrystalSparkleSFXTrigger : MonoBehaviour
	{
		private void Awake()
		{
			base.gameObject.SendGameEvent(CrystalSparkleVFXTriggers.VFXCrystalSparkleVFXPresent);
		}
	}
}
