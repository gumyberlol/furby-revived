using UnityEngine;

namespace Relentless
{
	public static class RelentlessGUIStyles
	{
		private static GUIStyle s_Style_Header;

		private static GUIStyle s_Style_HeaderBold;

		private static GUIStyle s_Style_Column;

		private static GUIStyle s_Style_ColumnCentred;

		private static GUIStyle s_Style_Normal;

		private static GUIStyle s_Style_NormalWrapped;

		private static GUIStyle s_Style_Note;

		private static GUIStyle s_Style_Warning;

		private static GUIStyle s_Style_Danger;

		private static GUIStyle s_Style_UnityException;

		private static GUIStyle s_Style_UnityLog;

		private static GUIStyle s_Style_UnityWarning;

		private static GUIStyle s_Style_Description;

		private static GUIStyle s_Style_Emphasis;

		private static GUIStyle s_Style_RsGreen;

		private static GUIStyle s_Style_RsRed;

		public static GUIStyle Style_Header
		{
			get
			{
				if (s_Style_Header == null)
				{
					s_Style_Header = new GUIStyle();
					s_Style_Header.normal.textColor = Color.cyan;
					s_Style_Header.alignment = TextAnchor.MiddleLeft;
				}
				return s_Style_Header;
			}
		}

		public static GUIStyle Style_HeaderBold
		{
			get
			{
				if (s_Style_HeaderBold == null)
				{
					s_Style_HeaderBold = new GUIStyle();
					s_Style_HeaderBold.fontStyle = FontStyle.Bold;
					s_Style_HeaderBold.normal.textColor = Color.cyan;
					s_Style_HeaderBold.alignment = TextAnchor.MiddleLeft;
				}
				return s_Style_HeaderBold;
			}
		}

		public static GUIStyle Style_Column
		{
			get
			{
				if (s_Style_Column == null)
				{
					s_Style_Column = new GUIStyle();
					s_Style_Column.normal.textColor = Color.yellow;
					s_Style_Column.alignment = TextAnchor.MiddleLeft;
				}
				return s_Style_Column;
			}
		}

		public static GUIStyle Style_ColumnCentred
		{
			get
			{
				if (s_Style_ColumnCentred == null)
				{
					s_Style_ColumnCentred = new GUIStyle();
					s_Style_ColumnCentred.normal.textColor = Color.yellow;
					s_Style_ColumnCentred.alignment = TextAnchor.MiddleCenter;
				}
				return s_Style_ColumnCentred;
			}
		}

		public static GUIStyle Style_Normal
		{
			get
			{
				if (s_Style_Normal == null)
				{
					s_Style_Normal = new GUIStyle();
					s_Style_Normal.normal.textColor = Color.white;
					s_Style_Normal.alignment = TextAnchor.MiddleLeft;
				}
				return s_Style_Normal;
			}
		}

		public static GUIStyle Style_NormalWrapped
		{
			get
			{
				if (s_Style_NormalWrapped == null)
				{
					s_Style_NormalWrapped = new GUIStyle();
					s_Style_NormalWrapped.normal.textColor = Color.white;
					s_Style_NormalWrapped.alignment = TextAnchor.MiddleLeft;
					s_Style_NormalWrapped.wordWrap = true;
				}
				return s_Style_NormalWrapped;
			}
		}

		public static GUIStyle Style_Note
		{
			get
			{
				if (s_Style_Note == null)
				{
					s_Style_Note = new GUIStyle();
					s_Style_Note.normal.textColor = Color.gray;
					s_Style_Note.alignment = TextAnchor.MiddleLeft;
				}
				return s_Style_Note;
			}
		}

		public static GUIStyle Style_Warning
		{
			get
			{
				if (s_Style_Warning == null)
				{
					s_Style_Warning = new GUIStyle();
					s_Style_Warning.normal.textColor = Color.red;
					s_Style_Warning.alignment = TextAnchor.MiddleLeft;
				}
				return s_Style_Warning;
			}
		}

		public static GUIStyle Style_Danger
		{
			get
			{
				if (s_Style_Danger == null)
				{
					s_Style_Danger = new GUIStyle();
					s_Style_Danger.fontStyle = FontStyle.Bold;
					s_Style_Danger.normal.textColor = Color.red;
				}
				return s_Style_Danger;
			}
		}

		public static GUIStyle Style_UnityException
		{
			get
			{
				if (s_Style_UnityException == null)
				{
					s_Style_UnityException = new GUIStyle();
					s_Style_UnityException.normal.textColor = new Color(1f, 0.39f, 0.28f);
					s_Style_UnityException.alignment = TextAnchor.MiddleLeft;
					s_Style_UnityException.wordWrap = false;
				}
				return s_Style_UnityException;
			}
		}

		public static GUIStyle Style_UnityLog
		{
			get
			{
				if (s_Style_UnityLog == null)
				{
					s_Style_UnityLog = new GUIStyle();
					s_Style_UnityLog.normal.textColor = new Color(0.83f, 0.83f, 0.83f);
					s_Style_UnityLog.wordWrap = false;
				}
				return s_Style_UnityLog;
			}
		}

		public static GUIStyle Style_UnityWarning
		{
			get
			{
				if (s_Style_UnityWarning == null)
				{
					s_Style_UnityWarning = new GUIStyle();
					s_Style_UnityWarning.normal.textColor = new Color(1f, 0.65f, 0f);
					s_Style_UnityWarning.wordWrap = false;
				}
				return s_Style_UnityWarning;
			}
		}

		public static GUIStyle Style_Description
		{
			get
			{
				if (s_Style_Description == null)
				{
					s_Style_Description = new GUIStyle();
					s_Style_Description.fontStyle = FontStyle.Normal;
					s_Style_Description.normal.textColor = Color.cyan;
				}
				return s_Style_Description;
			}
		}

		public static GUIStyle Style_RsGreen
		{
			get
			{
				if (s_Style_RsGreen == null)
				{
					s_Style_RsGreen = new GUIStyle();
					s_Style_RsGreen.fontStyle = FontStyle.Normal;
					s_Style_RsGreen.normal.textColor = new Color(0.25f, 0.88f, 0.25f);
				}
				return s_Style_RsGreen;
			}
		}

		public static GUIStyle Style_RsRed
		{
			get
			{
				if (s_Style_RsRed == null)
				{
					s_Style_RsRed = new GUIStyle();
					s_Style_RsRed.fontStyle = FontStyle.Normal;
					s_Style_RsRed.normal.textColor = new Color(0.88f, 0.25f, 0.25f);
				}
				return s_Style_RsRed;
			}
		}
	}
}
