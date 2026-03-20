using System.Collections.Generic;
using UnityEngine;

namespace Eye3.Ware
{
	public static class GetObjectHierarchyExtension
	{
		public static string GetHierarchyPath(this GameObject go)
		{
			return GetHierarchyPath(go.transform);
		}

		public static string GetHierarchyPath(this Component cmp)
		{
			return GetHierarchyPath(cmp.transform);
		}

		public static string GetHierarchyPath(Transform trans)
		{
			List<string> list = new List<string>();
			while (trans.parent != null)
			{
				list.Add(trans.gameObject.name);
				trans = trans.parent;
			}
			if (trans == trans.root)
			{
				list.Add(trans.gameObject.name);
			}
			list.Reverse();
			return "/" + string.Join("/", list.ToArray());
		}

		public static string[] GetHierarchies(this GameObject go)
		{
			return GetHierarchies(go.transform);
		}

		public static string[] GetHierarchies(this Component cmp)
		{
			return GetHierarchies(cmp.transform);
		}

		public static string[] GetHierarchies(Transform trans)
		{
			List<string> list = new List<string>();
			Transform root = trans.root;
			list.Add("/" + root.gameObject.name);
			if (root.childCount == 0)
			{
				return list.ToArray();
			}
			Stack<Transform> stack = new Stack<Transform>();
			GetLeaves(root, stack);
			while (stack.Count > 0)
			{
				list.Add(GetHierarchyPath(stack.Pop()));
			}
			return list.ToArray();
		}

		public static Stack<Transform> GetLeaves(Transform trans, Stack<Transform> leaves)
		{
			if (trans.childCount == 0)
			{
				leaves.Push(trans);
			}
			foreach (Transform tran in trans)
			{
				GetLeaves(tran, leaves);
			}
			return leaves;
		}
	}
}
