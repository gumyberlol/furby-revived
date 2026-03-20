using UnityEngine;

namespace Relentless
{
	public class RelentlessMonoBehaviour : MonoBehaviour
	{
		public delegate void Task();

		public void Invoke(Task task, float time)
		{
			Invoke(task.Method.Name, time);
		}

		public void InvokeRepeating(Task task, float time, float repeatRate)
		{
			InvokeRepeating(task.Method.Name, time, repeatRate);
		}

		public bool IsInvoking(Task task)
		{
			return IsInvoking(task.Method.Name);
		}

		public void CancelInvoke(Task task)
		{
			CancelInvoke(task.Method.Name);
		}
	}
}
