using System.Collections;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class FlairLibrary : MonoBehaviour
	{
		public class PrefabLoader
		{
			private GameObject m_prefab;

			private Flair m_flair;

			public GameObject Prefab
			{
				get
				{
					return m_prefab;
				}
			}

			public Flair Flair
			{
				get
				{
					return m_flair;
				}
			}

			public PrefabLoader(Flair flair)
			{
				m_flair = flair;
			}

			public IEnumerator Load(GameObject activeObject)
			{
				AssetBundleHelpers.AssetBundleLoad assetResult = new AssetBundleHelpers.AssetBundleLoad();
				IEnumerator assetEnumerator = AssetBundleHelpers.Load(m_flair.Path, m_flair.IsCompressed, assetResult, activeObject, typeof(GameObject), true);
				while (assetEnumerator.MoveNext())
				{
					yield return assetEnumerator.Current;
				}
				if (assetResult.isLoaded)
				{
					m_prefab = (GameObject)assetResult.m_object;
				}
			}
		}

		public FlairCollection Collection;

		public PrefabLoader GetPrefabLoader(string name)
		{
			Flair flair = Collection.Flairs.Where((Flair flr) => flr.Name == name).First();
			if (flair == null)
			{
				return null;
			}
			return new PrefabLoader(flair);
		}
	}
}
