using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Eye3.TestStar;
using Eye3.Ware;
using UnityEngine;

public class SceneTestRunner : MonoBehaviour
{
	private Queue<MethodInfo> testMethods = new Queue<MethodInfo>();

	public TestFixture testFixture;

	public TestResult currentResult;

	public bool running;

	public static SceneTestRunner Instance()
	{
		return (SceneTestRunner)UnityEngine.Object.FindObjectOfType(typeof(SceneTestRunner));
	}

	public TestResult InitRun(string runId, TestResult tr)
	{
		testFixture = SceneFixture.Instance().GetFixture(tr.name);
		tr.SyncResults(testFixture);
		tr.RemoveAll("SetUp");
		tr.RemoveAll("TearDown");
		tr.runId = runId;
		currentResult = tr;
		return currentResult;
	}

	public void StartRun()
	{
		InitTestMethods();
		if (testMethods.Count == 0)
		{
			currentResult.state = TestState.skipped;
			return;
		}
		IncarnateGhosts(testFixture);
		InitPrefabs(testFixture);
		try
		{
			testFixture.testScript.SetUp();
		}
		catch (Exception exception)
		{
			foreach (Result item in currentResult)
			{
				item.state = TestState.skipped;
			}
			Result result = new Result("SetUp");
			result.exception = exception;
			currentResult.results.Insert(0, result);
			currentResult.currIdx = -1;
			return;
		}
		StartCoroutine("RunMethod");
	}

	public void IncarnateGhosts(TestFixture testFixture)
	{
		List<GameObject> list = new List<GameObject>();
		SceneFixture.Instance().ghosts.Clean();
		foreach (GameObject ghost in SceneFixture.Instance().ghosts)
		{
			Ghost.Incarnate(ghost);
			list.Add(ghost);
		}
		testFixture.ghosts.Clean();
		foreach (GameObject ghost2 in testFixture.ghosts)
		{
			Ghost.Incarnate(ghost2);
			list.Add(ghost2);
		}
		testFixture.testScript.exGhosts = list;
	}

	public void InitPrefabs(TestFixture testFixture)
	{
		Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();
		foreach (PrefabRef prefab in SceneFixture.Instance().prefabs)
		{
			if (!string.IsNullOrEmpty(prefab.name) && prefab.prefab != null)
			{
				dictionary.Add(prefab.name, prefab.prefab);
			}
		}
		foreach (PrefabRef prefab2 in testFixture.prefabs)
		{
			if (!string.IsNullOrEmpty(prefab2.name) && prefab2.prefab != null)
			{
				dictionary[prefab2.name] = prefab2.prefab;
			}
		}
		testFixture.testScript.prefabs = dictionary;
	}

	public IEnumerator RunMethod()
	{
		while (testMethods.Count > 0)
		{
			MethodInfo activeMethod = testMethods.Dequeue();
			currentResult.SetCurrent(activeMethod.Name);
			Result mResult = currentResult.GetCurrent();
			TestMethodState tms = new TestMethodState(Time.time, Time.fixedTime, Time.frameCount);
			object[] args = new object[1] { tms };
			mResult.state = TestState.running;
			while (tms.state < TestState.passed)
			{
				tms.waitForFixedUpdate = false;
				tms.waitForFrames = -1;
				tms.waitForSeconds = -1f;
				mResult.state = TestState.running;
				try
				{
					tms = (TestMethodState)activeMethod.Invoke(testFixture.testScript, args);
				}
				catch (TargetInvocationException ex)
				{
					mResult.exception = ex.GetBaseException();
					break;
				}
				if (tms.waitForSeconds > 0f)
				{
					mResult.state = TestState.waiting;
					yield return new WaitForSeconds(tms.waitForSeconds);
				}
				else if (tms.waitForFrames > 0)
				{
					int targetFrame = Time.frameCount + tms.waitForFrames;
					mResult.state = TestState.waiting;
					while (Time.frameCount != targetFrame)
					{
						yield return null;
					}
				}
				else if (tms.waitForFixedUpdate)
				{
					mResult.state = TestState.waiting;
					yield return new WaitForFixedUpdate();
				}
				else
				{
					yield return null;
				}
				tms.timesCalled++;
			}
			if (mResult.niceException == null)
			{
				if (tms.state == TestState.running)
				{
					throw new RunException("method run should never be in this state");
				}
				if (tms.state != TestState.completed && string.IsNullOrEmpty(tms.message))
				{
					mResult.exception = new MessageRequired("A message is required when setting a method to state " + tms.state);
				}
				else
				{
					mResult.state = tms.state;
					mResult.message = tms.message;
				}
			}
			if (mResult.state == TestState.failed)
			{
				while (testMethods.Count > 0)
				{
					activeMethod = testMethods.Dequeue();
					currentResult.SetCurrent(activeMethod.Name);
					mResult = currentResult.GetCurrent();
					mResult.state = TestState.skipped;
				}
			}
			else if (mResult.state == TestState.completed)
			{
				mResult.state = TestState.passed;
			}
		}
		try
		{
			testFixture.testScript.TearDown();
		}
		catch (Exception ex2)
		{
			Exception e = ex2;
			Result result = new Result("TearDown")
			{
				exception = e
			};
			currentResult.results.Add(result);
		}
		currentResult.currIdx = -1;
	}

	public void Update()
	{
		if (!running && testFixture != null && currentResult.state == TestState.noState)
		{
			StartRun();
			running = true;
		}
		else
		{
			running = false;
		}
	}

	private void InitTestMethods()
	{
		testMethods.Clear();
		MethodInfo[] array = TestScriptMeta.GetTestMethods(testFixture.testScript.GetType());
		foreach (MethodInfo item in array)
		{
			testMethods.Enqueue(item);
		}
	}
}
