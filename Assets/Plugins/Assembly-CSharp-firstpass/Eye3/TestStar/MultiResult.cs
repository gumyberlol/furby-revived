using System;
using System.Collections;
using System.Collections.Generic;

namespace Eye3.TestStar
{
	[Serializable]
	public abstract class MultiResult<T> : Result, IEnumerable, IEnumerable<T> where T : Result
	{
		public List<T> results = new List<T>();

		public int currIdx;

		public MultiResult()
		{
		}

		public MultiResult(string name)
			: base(name)
		{
		}

		public MultiResult(string name, string subName)
			: base(name, subName)
		{
		}

		public MultiResult(string name, TestState state)
			: base(name, state)
		{
		}

		public MultiResult(string name, string subName, TestState state)
			: base(name, subName, state)
		{
		}

		public MultiResult(string name, Exception e)
			: base(name, e)
		{
		}

		public MultiResult(string name, string subName, Exception e)
			: base(name, subName, e)
		{
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		protected override TestState CalculateState()
		{
			if (st > TestState.running || results.Count == 0)
			{
				return st;
			}
			if (results.Exists((T r) => r.state == TestState.running || r.state == TestState.waiting))
			{
				st = TestState.running;
				return st;
			}
			if (results.Exists((T r) => r.state == TestState.failed))
			{
				st = TestState.failed;
				return st;
			}
			if (results.Exists((T r) => r.state > TestState.failed))
			{
				st = TestState.completed;
				return st;
			}
			if (results.TrueForAll((T r) => r.state == TestState.passed))
			{
				st = TestState.passed;
			}
			return st;
		}

		public virtual bool ResultsLeft()
		{
			if (currIdx < results.Count)
			{
				return true;
			}
			return false;
		}

		public virtual T GetCurrent()
		{
			try
			{
				return results[currIdx];
			}
			catch (ArgumentOutOfRangeException)
			{
				return (T)null;
			}
		}

		public virtual void SetCurrent(string name)
		{
			T result = GetResult(name);
			if (result == null)
			{
				currIdx = -1;
			}
			else
			{
				currIdx = results.IndexOf(result);
			}
		}

		public void SetCurrent(T newResult)
		{
			results[currIdx] = newResult;
		}

		public void Add(T result)
		{
			results.Add(result);
		}

		public T GetResult(string name)
		{
			return results.Find((T r) => string.Equals(r.name, name, StringComparison.OrdinalIgnoreCase));
		}

		public T GetResultBySubName(string subName)
		{
			return results.Find((T r) => string.Equals(r.subName, subName, StringComparison.OrdinalIgnoreCase));
		}

		public void ReplaceResult(T result)
		{
			T result2 = GetResult(result.name);
			if (result2 != null)
			{
				int index = results.IndexOf(result2);
				results[index] = result;
			}
			else
			{
				results.Add(result);
			}
		}

		public int RemoveAllBySubName(string subName)
		{
			return results.RemoveAll((T r) => string.Equals(r.subName, subName, StringComparison.OrdinalIgnoreCase));
		}

		public int RemoveAll(string name)
		{
			return results.RemoveAll((T r) => string.Equals(r.name, name, StringComparison.OrdinalIgnoreCase));
		}

		public void Remove(T result)
		{
			results.Remove(result);
		}

		public override void Flush()
		{
			foreach (T result in results)
			{
				T current = result;
				current.Flush();
			}
			base.Flush();
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (T result in results)
			{
				yield return result;
			}
		}
	}
}
