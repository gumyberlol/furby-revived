using System.Xml;
using System.Xml.Schema;
using UnityEngine;

namespace Relentless
{
	public class ScribeNamedTextReader
	{
		public TextAsset m_namedTextFile;

		public bool m_loadOnStart = true;

		private string m_currentValue;

		private string m_currentKey;

		public void ReadNamedText(string path, ref NamedTextTable namedText)
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
			xmlReaderSettings.IgnoreWhitespace = true;
			xmlReaderSettings.IgnoreComments = true;
			xmlReaderSettings.CheckCharacters = false;
			xmlReaderSettings.ValidationFlags = XmlSchemaValidationFlags.None;
			xmlReaderSettings.ValidationType = ValidationType.None;
			XmlReader xmlReader = XmlReader.Create(path, xmlReaderSettings);
			m_currentValue = string.Empty;
			m_currentKey = string.Empty;
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
				case XmlNodeType.Element:
					StartElement(xmlReader, namedText);
					break;
				case XmlNodeType.EndElement:
					EndElement(xmlReader, namedText);
					break;
				case XmlNodeType.Text:
					if (m_currentKey != string.Empty && m_currentValue == string.Empty)
					{
						m_currentValue = xmlReader.Value;
					}
					break;
				}
			}
		}

		private void StartElement(XmlReader reader, NamedTextTable namedText)
		{
			switch (reader.Name)
			{
			case "NamedTextItem":
				m_currentKey = reader.GetAttribute("TextName");
				break;
			}
		}

		private void EndElement(XmlReader reader, NamedTextTable namedText)
		{
			switch (reader.Name)
			{
			case "NamedTextItem":
				namedText[m_currentKey] = m_currentValue;
				m_currentValue = string.Empty;
				m_currentKey = string.Empty;
				break;
			}
		}
	}
}
