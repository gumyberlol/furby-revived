using System.Collections.Generic;
using Relentless;

public class TrackedMonoBehaviour<T> : RelentlessMonoBehaviour where T : class
{
	private static HashSet<T> s_instances = new HashSet<T>();

	public static HashSet<T> Instances
	{
		get
		{
			return s_instances;
		}
	}

	protected virtual void OnEnable()
	{
		T val = this as T;
		if (val != null)
		{
			s_instances.Add(val);
		}
	}

	protected virtual void OnDisable()
	{
		T val = this as T;
		if (val != null)
		{
			s_instances.Remove(val);
		}
	}
}
