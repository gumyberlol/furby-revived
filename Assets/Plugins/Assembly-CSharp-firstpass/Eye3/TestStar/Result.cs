using System;
using UnityEngine;

namespace Eye3.TestStar
{
	[Serializable]
	public class Result
	{
		public DateTime when;

		public bool expanded;

		public string runId;

		public string name;

		public string subName;

		private Exception exc;

		[SerializeField]
		protected PersistentException _niceException;

		public string message;

		[SerializeField]
		protected TestState st = TestState.noState;

		public Exception exception
		{
			get
			{
				return exc;
			}
			set
			{
				exc = value;
				if (value != null)
				{
					niceException = new PersistentException(value);
					state = TestState.failed;
					expanded = true;
				}
			}
		}

		public PersistentException niceException
		{
			get
			{
				if (_niceException != null && string.IsNullOrEmpty(_niceException.message))
				{
					_niceException = null;
				}
				return _niceException;
			}
			set
			{
				_niceException = value;
			}
		}

		public TestState state
		{
			get
			{
				return CalculateState();
			}
			set
			{
				when = DateTime.UtcNow;
				st = value;
			}
		}

		public Result()
		{
		}

		public Result(string name)
		{
			this.name = name;
		}

		public Result(string name, string subName)
			: this(name)
		{
			this.subName = subName;
		}

		public Result(string name, TestState state)
			: this(name)
		{
			this.state = state;
		}

		public Result(string name, string subName, TestState state)
			: this(name, subName)
		{
			this.state = state;
		}

		public Result(string name, Exception e)
			: this(name)
		{
			exception = e;
		}

		public Result(string name, string subName, Exception e)
			: this(name, subName)
		{
			exception = e;
		}

		public virtual void Flush()
		{
			runId = string.Empty;
			when = default(DateTime);
			exc = null;
			niceException = null;
			st = TestState.noState;
			message = string.Empty;
		}

		protected virtual TestState CalculateState()
		{
			return st;
		}

		public virtual bool Failed()
		{
			return state == TestState.failed;
		}
	}
}
