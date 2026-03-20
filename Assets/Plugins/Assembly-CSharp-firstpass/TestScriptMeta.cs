using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

[Serializable]
public static class TestScriptMeta
{
	[Serializable]
	public class Item
	{
		[SerializeField]
		private string cmpName;

		public string humanName;

		public string templatePath;

		public int templateOffset;

		public string compilerName
		{
			get
			{
				return (!string.IsNullOrEmpty(cmpName)) ? cmpName : humanName;
			}
			set
			{
				cmpName = value;
			}
		}

		public string extension
		{
			get
			{
				return Path.GetExtension(templatePath);
			}
		}

		public string className
		{
			get
			{
				return Path.GetFileNameWithoutExtension(templatePath);
			}
		}

		public string fullTemplatePath
		{
			get
			{
				return Path.Combine(Application.dataPath, templatePath);
			}
		}

		public Item(string humanName, string templatePath, int templateOffset)
		{
			this.humanName = humanName;
			this.templatePath = templatePath;
			this.templateOffset = templateOffset;
		}

		public Item(string humanName, string templatePath, int templateOffset, string compilerName)
			: this(humanName, templatePath, templateOffset)
		{
			cmpName = compilerName;
		}
	}

	public static readonly string[] ignoreProps = new string[5] { "m_Script", "startTime", "created", "prefabs", "exGhosts" };

	public static Dictionary<string, string> available = new Dictionary<string, string>();

	public static List<Item> metaList = new List<Item>(new Item[3]
	{
		new Item("CSharp", "Library/3rdParty/Eye3/TestStar/Editor/Eye3/TestStar/Templates/TestScriptCSharp.cs", 6),
		new Item("JavaScript", "Library/3rdParty/Eye3/TestStar/Editor/Eye3/TestStar/Templates/TestScriptJavaScript.js", 4),
		new Item("Boo", "Library/3rdParty/Eye3/TestStar/Editor/Eye3/TestStar/Templates/TestScriptBoo.boo", 4)
	});

	public static string[] templateNames
	{
		get
		{
			return metaList.Select((Item m) => m.humanName).ToArray();
		}
	}

	public static Item Get(string name)
	{
		return metaList.Find((Item m) => m.humanName == name || m.compilerName == name);
	}

	public static Item Get(int idx)
	{
		return metaList[idx];
	}

	public static bool SetUpDefined(Type type)
	{
		return type.GetMethod("SetUp", BindingFlags.Instance | BindingFlags.Public) != null;
	}

	public static bool TearDownDefined(Type type)
	{
		return type.GetMethod("TearDown", BindingFlags.Instance | BindingFlags.Public) != null;
	}

	public static MethodInfo[] GetTestMethods(Type type)
	{
		Type typeFromHandle = typeof(TestMethodState);
		List<MethodInfo> list = new List<MethodInfo>();
		MethodInfo[] methods = type.GetMethods();
		foreach (MethodInfo methodInfo in methods)
		{
			if (methodInfo.ReturnType == typeFromHandle)
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
				if (parameters.Length == 1 && parameters[0].ParameterType == typeFromHandle)
				{
					list.Add(methodInfo);
				}
			}
		}
		return list.ToArray();
	}
}
