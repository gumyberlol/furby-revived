using System;
using System.Collections.Generic;

namespace Eye3.TestStar
{
	[Serializable]
	public class SceneResult : MultiResult<TestResult>
	{
		public string path
		{
			get
			{
				return subName;
			}
			set
			{
				subName = value;
			}
		}

		public TestResult currentTestResult
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

		public SceneResult()
		{
		}

		public SceneResult(string name)
			: base(name)
		{
		}

		public SceneResult(string name, string path)
			: base(name, path)
		{
		}

		public TestResult AddOnce(TestFixture tf)
		{
			TestResult testResult = results.Find((TestResult r) => r.name == tf.testName);
			if (testResult == null)
			{
				return Add(tf);
			}
			if (testResult.scriptName != tf.testScriptType)
			{
				testResult.scriptName = tf.testScriptType;
			}
			return testResult;
		}

		public TestResult Add(TestFixture tf)
		{
			TestResult testResult = new TestResult(tf);
			results.Add(testResult);
			return testResult;
		}

		public void SyncResults()
		{
			if (results.Count == 0)
			{
				InitResults();
				return;
			}
			SceneFixture sceneFixture = SceneFixture.Instance();
			if (!(sceneFixture != null))
			{
				return;
			}
			List<TestFixture> activeTests = sceneFixture.GetActiveTests();
			for (int i = 0; i < activeTests.Count; i++)
			{
				TestResult testResult = results.Find((TestResult r) => r.name == activeTests[i].testName);
				if (testResult == null)
				{
					int index = ((i <= results.Count) ? i : results.Count);
					testResult = new TestResult(activeTests[i]);
					results.Insert(index, testResult);
				}
				else if (results.IndexOf(testResult) != i)
				{
					results.Remove(testResult);
					results.Insert(i, testResult);
				}
				testResult.SyncResults(activeTests[i]);
			}
			results.RemoveAll((TestResult r) => !activeTests.Exists((TestFixture tf) => r.name == tf.testName));
			if (base.state > TestState.passed)
			{
				base.state = TestState.noState;
			}
		}

		public void InitResults()
		{
			SceneFixture sceneFixture = SceneFixture.Instance();
			if (!(sceneFixture != null))
			{
				return;
			}
			results.Clear();
			foreach (TestFixture activeTest in sceneFixture.GetActiveTests())
			{
				Add(activeTest);
			}
		}
	}
}
