using System.Linq;
using UnityEngine;

namespace Furby
{
	public class EnableDisableOnCartonFull : MonoBehaviour
	{
		[SerializeField]
		private Color m_fadeColour = Color.grey;

		[SerializeField]
		private GameObject m_enableWhenDisabled;

		private void Start()
		{
			if (FurbyGlobals.BabyRepositoryHelpers.EggCarton.Count() >= FurbyGlobals.BabyLibrary.GetMaxEggsInCarton())
			{
				UIWidget[] componentsInChildren = GetComponentsInChildren<UIWidget>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].color *= m_fadeColour;
				}
				Collider[] componentsInChildren2 = GetComponentsInChildren<Collider>();
				foreach (Collider collider in componentsInChildren2)
				{
					collider.enabled = false;
				}
				if (m_enableWhenDisabled != null)
				{
					m_enableWhenDisabled.SetActive(true);
				}
			}
		}
	}
}
