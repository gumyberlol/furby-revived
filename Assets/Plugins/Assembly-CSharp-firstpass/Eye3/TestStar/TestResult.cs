using System;
using System.Collections.Generic;

namespace Eye3.TestStar
{
	[Serializable]
	public class TestResult : MultiResult<Result>
	{
		public string scriptName
		{
			get
			{
				return subName;
			}
			set
			{
				subName = scriptName;
			}
		}

		public Result currentMethodResult
		{
			get
			{
				return GetCurrent();
			}
			set
			{
				SetCurrent(value);
			}
		}

		public TestResult(TestFixture tf)
		{
			name = tf.testName;
			subName = tf.testScriptType;
			InitResults(tf);
		}

		public TestResult(string testName)
			: base(testName)
		{
		}

		public TestResult(string testName, string scriptName)
			: base(testName, scriptName)
		{
		}

		public TestResult(string testName, string scriptName, TestState state)
			: base(testName, scriptName, state)
		{
		}

		public TestResult(string testName, Exception e)
			: base(testName, e)
		{
		}

		public Result AddOnce(string name)
		{
			Result result = results.Find((Result r) => r.name == name);
			if (result == null)
			{
				result = Add(name);
			}
			return result;
		}

		public Result Add(string name)
		{
			Result result = new Result(name);
			results.Add(result);
			return result;
		}

		public void SyncResults(TestFixture tf)
		{
			if (results.Count == 0)
			{
				InitResults(tf);
				return;
			}
			List<string> testNames = tf.GetTestMethodNames();
			for (int i = 0; i < testNames.Count; i++)
			{
				Result result = results.Find((Result r) => r.name == testNames[i]);
				if (result == null)
				{
					int index = ((i <= results.Count) ? i : results.Count);
					results.Insert(index, new Result(testNames[i]));
				}
				else if (results.IndexOf(result) != i)
				{
					results.Remove(result);
					results.Insert(i, result);
				}
			}
			Type type = tf.testScript.GetType();
			if (TestScriptMeta.SetUpDefined(type))
			{
				Result result2 = results.Find((Result r) => r.name == "SetUp");
				if (result2 != null)
				{
					results.Remove(result2);
					results.Insert(0, result2);
				}
				testNames.Insert(0, "SetUp");
			}
			if (TestScriptMeta.TearDownDefined(type))
			{
				testNames.Add("TearDown");
			}
			results.RemoveAll((Result r) => !testNames.Contains(r.name));
			if (base.state > TestState.passed)
			{
				base.state = TestState.noState;
			}
		}

		public void InitResults(TestFixture tf)
		{
			results.Clear();
			foreach (string testMethodName in tf.GetTestMethodNames())
			{
				Add(testMethodName);
			}
		}
	}
}
