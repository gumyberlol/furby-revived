using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class CallJavaCode : MonoBehaviour
{
	[DllImport("javabridge")]
	private static extern IntPtr getCacheDir();
}
