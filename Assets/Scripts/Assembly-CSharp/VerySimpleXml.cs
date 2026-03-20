public class VerySimpleXml
{
	public static string Indent(int num)
	{
		string text = "\t";
		for (int i = 0; i < num - 1; i++)
		{
			text += text;
		}
		return text;
	}

	public static string StartNode(string nodeName)
	{
		return '<' + nodeName + '>';
	}

	public static string StartNode(string nodeName, int indent)
	{
		return Indent(indent) + StartNode(nodeName);
	}

	public static string EndNode(string nodeName)
	{
		return "</" + nodeName + '>';
	}

	public static string EndNode(string nodeName, int indent)
	{
		return Indent(indent) + EndNode(nodeName);
	}

	public static string NodeValue(string line, string nodeName)
	{
		string text = StartNode(nodeName);
		string value = EndNode(nodeName);
		int num = line.IndexOf(text) + text.Length;
		return line.Substring(num, line.IndexOf(value) - num);
	}
}
