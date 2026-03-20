using System;

namespace com.google.zxing.common
{
	public sealed class DefaultGridSampler : GridSampler
	{
		public override BitMatrix sampleGrid(BitMatrix image, int dimension, float p1ToX, float p1ToY, float p2ToX, float p2ToY, float p3ToX, float p3ToY, float p4ToX, float p4ToY, float p1FromX, float p1FromY, float p2FromX, float p2FromY, float p3FromX, float p3FromY, float p4FromX, float p4FromY)
		{
			PerspectiveTransform transform = PerspectiveTransform.quadrilateralToQuadrilateral(p1ToX, p1ToY, p2ToX, p2ToY, p3ToX, p3ToY, p4ToX, p4ToY, p1FromX, p1FromY, p2FromX, p2FromY, p3FromX, p3FromY, p4FromX, p4FromY);
			return sampleGrid(image, dimension, transform);
		}

		public override BitMatrix sampleGrid(BitMatrix image, int dimension, PerspectiveTransform transform)
		{
			BitMatrix bitMatrix = new BitMatrix(dimension);
			float[] array = new float[dimension << 1];
			for (int i = 0; i < dimension; i++)
			{
				int num = array.Length;
				float num2 = (float)i + 0.5f;
				for (int j = 0; j < num; j += 2)
				{
					array[j] = (float)(j >> 1) + 0.5f;
					array[j + 1] = num2;
				}
				transform.transformPoints(array);
				GridSampler.checkAndNudgePoints(image, array);
				try
				{
					for (int k = 0; k < num; k += 2)
					{
						if (image.get_Renamed((int)array[k], (int)array[k + 1]))
						{
							bitMatrix.set_Renamed(k >> 1, i);
						}
					}
				}
				catch (IndexOutOfRangeException)
				{
					throw ReaderException.Instance;
				}
			}
			return bitMatrix;
		}
	}
}
