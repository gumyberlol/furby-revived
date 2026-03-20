using System;
using System.Collections.Generic;
using System.Reflection;
using Eye3.Ware;
using UnityEngine;

public class TestFixture : MonoBehaviour
{
	public string testName;

	public float testStart;

	public float testEnd = 120f;

	public bool runTest = true;

	public List<GameObject> ghosts = new List<GameObject>();

	public List<PrefabRef> prefabs = new List<PrefabRef>();

	public TestScript testScript;

	public bool expanded = true;

	[SerializeField]
	private string currentType = string.Empty;

	public string lastScriptType = string.Empty;

	public string testScriptType
	{
		get
		{
			if (testScript == null)
			{
				currentType = string.Empty;
			}
			else
			{
				string text = testScript.GetType().ToString();
				if (text != currentType)
				{
					lastScriptType = currentType;
					currentType = text;
				}
			}
			return currentType;
		}
	}

	public int TestChoiceIdx(string[] scriptNames)
	{
		if (!string.IsNullOrEmpty(testScriptType))
		{
			return Array.IndexOf(scriptNames, testScriptType);
		}
		return 0;
	}

	public static List<string> GetScriptNames()
	{
		List<string> list = new List<string>();
		list.AddRange(TestScriptMeta.available.Keys);
		list.Sort();
		list.Insert(0, "No Script Assigned");
		return list;
	}

	public List<string> GetTestMethodNames()
	{
		List<string> list = new List<string>();
		if (testScript == null)
		{
			return list;
		}
		MethodInfo[] testMethods = TestScriptMeta.GetTestMethods(testScript.GetType());
		foreach (MethodInfo methodInfo in testMethods)
		{
			list.Add(methodInfo.Name);
		}
		return list;
	}

	public void SetName(string newName)
	{
		testName = Util.MakeUnique(newName, SceneFixture.Instance().GetFixtureNamesContaining(newName));
	}

	public void SetNameIfMistitled(string name)
	{
		if (testName.Contains(SceneFixture.untitledTestString))
		{
			SetName(name);
		}
		else
		{
			if (testName.Contains(name))
			{
				return;
			}
			foreach (string key in TestScriptMeta.available.Keys)
			{
				if (testName.Contains(key))
				{
					SetName(name);
					break;
				}
			}
		}
	}

	public void SetTest(string name)
	{
		if (testScript == null)
		{
			base.enabled = true;
		}
		SetNameIfMistitled(name);
		testScript = (TestScript)ScriptableObject.CreateInstance(name);
	}

	public void UnsetTest()
	{
		testScript = null;
	}

	public static TestFixture New()
	{
		TestFixture testFixture = SceneFixture.Instance().gameObject.AddComponent<TestFixture>();
		testFixture.enabled = false;
		return testFixture;
	}

	public static TestFixture New(string name)
	{
		TestFixture testFixture = New();
		testFixture.SetName(name);
		return testFixture;
	}

	public static TestFixture New(string name, List<GameObject> ghosts)
	{
		TestFixture testFixture = New(name);
		testFixture.ghosts.AddRange(ghosts);
		return testFixture;
	}

	public void OnEnable()
	{
	}

	public void OnDisable()
	{
	}
}
