using System;
using System.Collections.Generic;
using Eye3.Ware;
using UnityEngine;

public class SceneFixture : MonoBehaviour
{
	public static SceneFixture instance;

	public List<PrefabRef> prefabs = new List<PrefabRef>();

	public List<GameObject> ghosts = new List<GameObject>();

	public static readonly string newTestString = "New Test";

	public static readonly string untitledTestString = "Untitled Test";

	public TestFixture[] fixtures
	{
		get
		{
			return base.gameObject.Cmps<TestFixture>();
		}
	}

	public List<TestFixture> GetActiveTests()
	{
		List<TestFixture> list = new List<TestFixture>();
		TestFixture[] array = fixtures;
		foreach (TestFixture testFixture in array)
		{
			if (testFixture.testScript != null && testFixture.enabled)
			{
				list.Add(testFixture);
			}
		}
		return list;
	}

	public List<string> GetActiveTestNames()
	{
		List<string> list = new List<string>();
		foreach (TestFixture activeTest in GetActiveTests())
		{
			list.Add(activeTest.testName);
		}
		return list;
	}

	public static SceneFixture Instance()
	{
		if (instance == null)
		{
			instance = (SceneFixture)UnityEngine.Object.FindObjectOfType(typeof(SceneFixture));
		}
		return instance;
	}

	public TestFixture GetFixture(string key)
	{
		return Array.Find(fixtures, (TestFixture tf) => tf.testName == key);
	}

	public List<string> GetFixtureNamesContaining(string name)
	{
		List<string> list = new List<string>();
		TestFixture[] array = fixtures;
		foreach (TestFixture testFixture in array)
		{
			if (!string.IsNullOrEmpty(testFixture.testName) && testFixture.testName.Contains(name))
			{
				list.Add(testFixture.testName);
			}
		}
		return list;
	}

	public string GetUntitledTestString()
	{
		List<string> fixtureNamesContaining = GetFixtureNamesContaining(untitledTestString);
		return Util.MakeUnique(untitledTestString, fixtureNamesContaining);
	}

	public List<string> AvailableTestTargetNames()
	{
		List<string> list = new List<string>();
		list.Add(newTestString);
		TestFixture[] array = fixtures;
		foreach (TestFixture testFixture in array)
		{
			list.Add(testFixture.testName);
		}
		return list;
	}

	public void AssociateNewGhosts(int targetIdx, List<GameObject> ghosts)
	{
		string text = AvailableTestTargetNames()[targetIdx];
		foreach (GameObject ghost in ghosts)
		{
			Ghost.Disincarnate(ghost);
		}
		if (text == newTestString)
		{
			TestFixture.New(GetUntitledTestString(), ghosts);
			return;
		}
		TestFixture[] array = fixtures;
		foreach (TestFixture testFixture in array)
		{
			if (testFixture.testName == text)
			{
				testFixture.ghosts.AddRange(ghosts);
			}
		}
	}

	public void AddFixture()
	{
		TestFixture.New(GetUntitledTestString());
	}

	public void OnEnable()
	{
		ghosts.Clean();
		TestFixture[] array = fixtures;
		foreach (TestFixture testFixture in array)
		{
			if (testFixture.testScript == null)
			{
				testFixture.enabled = false;
			}
		}
	}

	public void OnDisable()
	{
	}
}
