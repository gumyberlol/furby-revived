using System.Text;

namespace com.google.zxing.common
{
	public sealed class ByteMatrix
	{
		private sbyte[][] bytes;

		private int width;

		private int height;

		public int Height
		{
			get
			{
				return height;
			}
		}

		public int Width
		{
			get
			{
				return width;
			}
		}

		public sbyte[][] Array
		{
			get
			{
				return bytes;
			}
		}

		public ByteMatrix(int width, int height)
		{
			bytes = new sbyte[height][];
			for (int i = 0; i < height; i++)
			{
				bytes[i] = new sbyte[width];
			}
			this.width = width;
			this.height = height;
		}

		public sbyte get_Renamed(int x, int y)
		{
			return bytes[y][x];
		}

		public void set_Renamed(int x, int y, sbyte value_Renamed)
		{
			bytes[y][x] = value_Renamed;
		}

		public void set_Renamed(int x, int y, int value_Renamed)
		{
			bytes[y][x] = (sbyte)value_Renamed;
		}

		public void clear(sbyte value_Renamed)
		{
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					bytes[i][j] = value_Renamed;
				}
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(2 * width * height + 2);
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					switch (bytes[i][j])
					{
					case 0:
						stringBuilder.Append(" 0");
						break;
					case 1:
						stringBuilder.Append(" 1");
						break;
					default:
						stringBuilder.Append("  ");
						break;
					}
				}
				stringBuilder.Append('\n');
			}
			return stringBuilder.ToString();
		}
	}
}
