using System.IO;
using UnityEngine;

namespace Uniject.Impl
{
	public class UnityResourceLoader : IResourceLoader
	{
		public static volatile string HackJsonBucket = string.Empty;

		public TextReader openTextFile(string path)
		{
			bool flag = false;
			if (string.CompareOrdinal("unibillInventory.json", path) == 0 && !string.IsNullOrEmpty(HackJsonBucket))
			{
				flag = true;
			}
			if (flag)
			{
				return new StringReader(HackJsonBucket);
			}
			TextAsset textAsset = (TextAsset)Resources.Load(path, typeof(TextAsset));
			if (textAsset != null)
			{
				return new StringReader(textAsset.text);
			}
			return null;
		}
	}
}
