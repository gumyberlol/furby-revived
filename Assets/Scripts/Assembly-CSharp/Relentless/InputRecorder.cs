using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Relentless
{
	public class InputRecorder : RelentlessMonoBehaviour
	{
		public enum eInputMode
		{
			PASSTHRU = 0,
			RECORD = 1,
			PLAY = 2
		}

		private const int INPUT_ID_INDEX = 0;

		private const int INPUT_TYPE_INDEX = 1;

		private const int INPUT_NAME_INDEX = 2;

		private const int INPUT_VALUE_INDEX = 3;

		public eInputMode m_inputMode;

		public int m_frameRate = 50;

		public float m_timeScale = 1f;

		private bool m_enabled;

		private StreamWriter m_streamWriter;

		private StreamReader m_streamReader;

		private Dictionary<string, bool> m_lastButtonDownInputs;

		private Dictionary<string, bool> m_lastButtonUpInputs;

		private Dictionary<string, bool> m_lastButtonInputs;

		private Dictionary<string, bool> m_lastMouseButtonDownInputs;

		private Dictionary<string, bool> m_lastMouseButtonUpInputs;

		private Dictionary<string, bool> m_lastMouseButtonInputs;

		private Dictionary<string, bool> m_lastKeyInputs;

		private Dictionary<string, bool> m_lastKeyUpInputs;

		private Dictionary<string, bool> m_lastKeyDownInputs;

		private Dictionary<string, Dictionary<string, bool>> m_boolDictionaries;

		private Dictionary<string, float> m_lastAxisInputs;

		private Dictionary<string, float> m_lastAxisRawInputs;

		private Dictionary<string, Dictionary<string, float>> m_floatDictionaries;

		private Dictionary<string, float> m_lastMousePositionInputs;

		private Vector3 m_lastMousePosition;

		private static InputRecorder m_instance;

		public static InputRecorder Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = (InputRecorder)Object.FindObjectOfType(typeof(InputRecorder));
					if (m_instance == null)
					{
						m_instance = new GameObject("InputRecord").AddComponent<InputRecorder>();
					}
				}
				return m_instance;
			}
		}

		public Vector3 mousePosition
		{
			get
			{
				return ProcessGetMousePosition();
			}
		}

		private void Awake()
		{
			m_lastButtonUpInputs = new Dictionary<string, bool>();
			m_lastButtonDownInputs = new Dictionary<string, bool>();
			m_lastButtonInputs = new Dictionary<string, bool>();
			m_lastMouseButtonUpInputs = new Dictionary<string, bool>();
			m_lastMouseButtonDownInputs = new Dictionary<string, bool>();
			m_lastMouseButtonInputs = new Dictionary<string, bool>();
			m_lastKeyInputs = new Dictionary<string, bool>();
			m_lastKeyUpInputs = new Dictionary<string, bool>();
			m_lastKeyDownInputs = new Dictionary<string, bool>();
			m_boolDictionaries = new Dictionary<string, Dictionary<string, bool>>
			{
				{ "ButtonUp", m_lastButtonUpInputs },
				{ "ButtonDown", m_lastButtonDownInputs },
				{ "Button", m_lastButtonInputs },
				{ "MouseButtonUp", m_lastMouseButtonUpInputs },
				{ "MouseButtonDown", m_lastMouseButtonDownInputs },
				{ "MouseButton", m_lastMouseButtonInputs },
				{ "Key", m_lastKeyInputs },
				{ "KeyUp", m_lastKeyUpInputs },
				{ "KeyDown", m_lastKeyDownInputs }
			};
			m_lastAxisInputs = new Dictionary<string, float>();
			m_lastAxisRawInputs = new Dictionary<string, float>();
			m_lastMousePositionInputs = new Dictionary<string, float>();
			m_floatDictionaries = new Dictionary<string, Dictionary<string, float>>
			{
				{ "Axis", m_lastAxisInputs },
				{ "AxisRaw", m_lastAxisRawInputs },
				{ "MousePosition", m_lastMousePositionInputs }
			};
			m_lastMousePosition = new Vector3(0f, 0f, 0f);
			Object.DontDestroyOnLoad(base.transform.gameObject);
			Random.seed = 0;
		}

		private void Start()
		{
		}

		public void Enable()
		{
			m_enabled = true;
			switch (m_inputMode)
			{
			case eInputMode.RECORD:
				m_streamWriter = new StreamWriter("InputRecording.txt");
				Time.captureFramerate = m_frameRate;
				break;
			case eInputMode.PLAY:
				m_streamReader = new StreamReader("InputRecording.txt");
				Time.captureFramerate = m_frameRate;
				break;
			}
		}

		public void Disable()
		{
			if (m_streamReader != null)
			{
				m_streamReader.Dispose();
				m_streamReader = null;
			}
			if (m_streamWriter != null)
			{
				m_streamWriter.Dispose();
				m_streamWriter = null;
			}
			m_enabled = false;
		}

		private void Update()
		{
			if (!m_enabled || m_streamReader == null || m_inputMode != eInputMode.PLAY)
			{
				return;
			}
			string text = m_streamReader.ReadLine();
			while (text != null && text.StartsWith("*"))
			{
				foreach (KeyValuePair<string, Dictionary<string, float>> floatDictionary in m_floatDictionaries)
				{
					string[] array = text.Split(',');
					if (array[1].Equals(floatDictionary.Key))
					{
						float value = float.Parse(array[3]);
						if (floatDictionary.Value.ContainsKey(array[2]))
						{
							floatDictionary.Value[array[2]] = value;
						}
						else
						{
							floatDictionary.Value.Add(array[2], value);
						}
						break;
					}
				}
				text = m_streamReader.ReadLine();
			}
			while (text != null && text.StartsWith("#"))
			{
				foreach (KeyValuePair<string, Dictionary<string, bool>> boolDictionary in m_boolDictionaries)
				{
					string[] array2 = text.Split(',');
					if (array2[1].Equals(boolDictionary.Key))
					{
						bool value2 = bool.Parse(array2[3]);
						if (boolDictionary.Value.ContainsKey(array2[2]))
						{
							boolDictionary.Value[array2[2]] = value2;
						}
						else
						{
							boolDictionary.Value.Add(array2[2], value2);
						}
						break;
					}
				}
				text = m_streamReader.ReadLine();
			}
			if (text != null && !text.StartsWith("t"))
			{
			}
		}

		private void LateUpdate()
		{
			if (m_enabled && m_streamWriter != null)
			{
				m_streamWriter.WriteLine("t {0}", Time.time);
			}
		}

		private void OnDestroy()
		{
			if (m_streamReader != null)
			{
				m_streamReader.Close();
			}
			if (m_streamWriter != null)
			{
				m_streamWriter.Close();
			}
		}

		public bool GetButton(string buttonName)
		{
			bool button = Input.GetButton(buttonName);
			return ProcessGetButton("Button", buttonName, button, m_lastButtonInputs);
		}

		public bool GetButtonDown(string buttonName)
		{
			bool buttonDown = Input.GetButtonDown(buttonName);
			return ProcessGetButton("ButtonDown", buttonName, buttonDown, m_lastButtonDownInputs);
		}

		public bool GetButtonUp(string buttonName)
		{
			bool buttonUp = Input.GetButtonUp(buttonName);
			return ProcessGetButton("ButtonUp", buttonName, buttonUp, m_lastButtonUpInputs);
		}

		public bool GetMouseButton(int buttonID)
		{
			bool mouseButton = Input.GetMouseButton(buttonID);
			return ProcessGetButton("MouseButton", buttonID.ToString(), mouseButton, m_lastMouseButtonInputs);
		}

		public bool GetMouseButtonDown(int buttonID)
		{
			bool mouseButtonDown = Input.GetMouseButtonDown(buttonID);
			return ProcessGetButton("MouseButtonDown", buttonID.ToString(), mouseButtonDown, m_lastMouseButtonDownInputs);
		}

		public bool GetMouseButtonUp(int buttonID)
		{
			bool mouseButtonUp = Input.GetMouseButtonUp(buttonID);
			return ProcessGetButton("MouseButtonUp", buttonID.ToString(), mouseButtonUp, m_lastMouseButtonUpInputs);
		}

		public float GetAxis(string axisName)
		{
			return ProcessGetAxis("Axis", axisName, m_lastAxisInputs);
		}

		public float GetAxisRaw(string axisName)
		{
			return ProcessGetAxis("AxisRaw", axisName, m_lastAxisRawInputs);
		}

		public bool GetKey(string keyName)
		{
			bool key = Input.GetKey(keyName);
			return ProcessGetButton("Key", keyName, key, m_lastKeyInputs);
		}

		public bool GetKeyUp(string keyName)
		{
			bool keyUp = Input.GetKeyUp(keyName);
			return ProcessGetButton("Key", keyName, keyUp, m_lastKeyUpInputs);
		}

		public bool GetKeyDown(string keyName)
		{
			bool keyDown = Input.GetKeyDown(keyName);
			return ProcessGetButton("Key", keyName, keyDown, m_lastKeyDownInputs);
		}

		private bool ProcessGetButton(string buttonType, string buttonName, bool inputValue, Dictionary<string, bool> buttonDictionary)
		{
			bool value = false;
			switch (m_inputMode)
			{
			case eInputMode.PASSTHRU:
				value = inputValue;
				break;
			case eInputMode.RECORD:
				value = inputValue;
				RecordBoolValue(buttonType, buttonName, value, buttonDictionary);
				break;
			case eInputMode.PLAY:
				buttonDictionary.TryGetValue(buttonName, out value);
				break;
			}
			return value;
		}

		private void RecordBoolValue(string boolType, string boolName, bool currentValue, Dictionary<string, bool> boolDictionary)
		{
			if (m_streamWriter == null)
			{
				return;
			}
			bool value = false;
			if (boolDictionary.TryGetValue(boolName, out value))
			{
				if (value == currentValue)
				{
					return;
				}
				boolDictionary[boolName] = currentValue;
			}
			else
			{
				boolDictionary.Add(boolName, currentValue);
			}
			m_streamWriter.WriteLine("#,{0},{1},{2},{3}", boolType, boolName, currentValue, Time.time);
		}

		private float ProcessGetAxis(string axisType, string axisName, Dictionary<string, float> axisDictionary)
		{
			float value = 0f;
			switch (m_inputMode)
			{
			case eInputMode.PASSTHRU:
				value = Input.GetAxis(axisName);
				break;
			case eInputMode.RECORD:
				value = Input.GetAxis(axisName);
				RecordFloatValue(axisType, axisName, value, axisDictionary);
				break;
			case eInputMode.PLAY:
				axisDictionary.TryGetValue(axisName, out value);
				break;
			}
			return value;
		}

		private Vector3 ProcessGetMousePosition()
		{
			Vector3 lastMousePosition = m_lastMousePosition;
			switch (m_inputMode)
			{
			case eInputMode.PASSTHRU:
				lastMousePosition = Input.mousePosition;
				break;
			case eInputMode.RECORD:
				lastMousePosition = Input.mousePosition;
				m_lastMousePosition = Input.mousePosition;
				RecordFloatValue("MousePosition", "X", lastMousePosition.x, m_lastMousePositionInputs);
				RecordFloatValue("MousePosition", "Y", lastMousePosition.y, m_lastMousePositionInputs);
				break;
			case eInputMode.PLAY:
				m_lastMousePositionInputs.TryGetValue("X", out lastMousePosition.x);
				m_lastMousePositionInputs.TryGetValue("Y", out lastMousePosition.y);
				m_lastMousePosition = lastMousePosition;
				break;
			}
			return lastMousePosition;
		}

		private void RecordFloatValue(string floatType, string floatName, float currentValue, Dictionary<string, float> floatDictionary)
		{
			if (m_streamWriter == null)
			{
				return;
			}
			float value = 0f;
			if (floatDictionary.TryGetValue(floatName, out value))
			{
				if (value == currentValue)
				{
					return;
				}
				floatDictionary[floatName] = currentValue;
			}
			else
			{
				floatDictionary.Add(floatName, currentValue);
			}
			m_streamWriter.WriteLine("*,{0},{1},{2},{3}", floatType, floatName, currentValue, Time.time);
		}
	}
}
