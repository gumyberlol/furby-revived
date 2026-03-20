using UnityEngine;

public class UIFilledSpriteGrow : UIFilledSprite
{
	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		float x = 0f;
		float num = 0f;
		float num2 = 1f;
		float num3 = -1f;
		float num4 = mOuterUV.xMin;
		float num5 = mOuterUV.yMin;
		float num6 = mOuterUV.xMax;
		float num7 = mOuterUV.yMax;
		if (base.fillDirection == FillDirection.Horizontal || base.fillDirection == FillDirection.Vertical)
		{
			float num8 = (num6 - num4) * base.fillAmount;
			float num9 = (num7 - num5) * base.fillAmount;
			if (base.fillDirection == FillDirection.Horizontal)
			{
				if (base.invert)
				{
					x = 1f - base.fillAmount;
					num4 = num6 - num8;
				}
				else
				{
					num2 *= base.fillAmount;
					num6 = num4 + num8;
				}
			}
			else if (base.fillDirection == FillDirection.Vertical)
			{
				if (base.invert)
				{
					float num10 = num3;
					num3 = num;
					num = num10;
					num *= base.fillAmount;
					num5 = num7 - num9;
				}
				else
				{
					num = 0f - (1f - base.fillAmount);
					num7 = num5 + num9;
				}
			}
		}
		Vector2[] array = new Vector2[4];
		Vector2[] array2 = new Vector2[4];
		array[0] = new Vector2(num2, num);
		array[1] = new Vector2(num2, num3);
		array[2] = new Vector2(x, num3);
		array[3] = new Vector2(x, num);
		array2[0] = new Vector2(num6, num7);
		array2[1] = new Vector2(num6, num5);
		array2[2] = new Vector2(num4, num5);
		array2[3] = new Vector2(num4, num7);
		Color32 item = base.color;
		if (base.fillDirection == FillDirection.Radial90)
		{
			if (!AdjustRadial(array, array2, base.fillAmount, base.invert))
			{
				return;
			}
		}
		else
		{
			if (base.fillDirection == FillDirection.Radial180)
			{
				Vector2[] array3 = new Vector2[4];
				Vector2[] array4 = new Vector2[4];
				for (int i = 0; i < 2; i++)
				{
					array3[0] = new Vector2(0f, 0f);
					array3[1] = new Vector2(0f, 1f);
					array3[2] = new Vector2(1f, 1f);
					array3[3] = new Vector2(1f, 0f);
					array4[0] = new Vector2(0f, 0f);
					array4[1] = new Vector2(0f, 1f);
					array4[2] = new Vector2(1f, 1f);
					array4[3] = new Vector2(1f, 0f);
					if (base.invert)
					{
						if (i > 0)
						{
							Rotate(array3, i);
							Rotate(array4, i);
						}
					}
					else if (i < 1)
					{
						Rotate(array3, 1 - i);
						Rotate(array4, 1 - i);
					}
					float num11;
					float to;
					if (i == 1)
					{
						num11 = ((!base.invert) ? 1f : 0.5f);
						to = ((!base.invert) ? 0.5f : 1f);
					}
					else
					{
						num11 = ((!base.invert) ? 0.5f : 1f);
						to = ((!base.invert) ? 1f : 0.5f);
					}
					array3[1].y = Mathf.Lerp(num11, to, array3[1].y);
					array3[2].y = Mathf.Lerp(num11, to, array3[2].y);
					array4[1].y = Mathf.Lerp(num11, to, array4[1].y);
					array4[2].y = Mathf.Lerp(num11, to, array4[2].y);
					float fill = base.fillAmount * 2f - (float)i;
					bool flag = i % 2 == 1;
					if (!AdjustRadial(array3, array4, fill, !flag))
					{
						continue;
					}
					if (base.invert)
					{
						flag = !flag;
					}
					if (flag)
					{
						for (int j = 0; j < 4; j++)
						{
							num11 = Mathf.Lerp(array[0].x, array[2].x, array3[j].x);
							to = Mathf.Lerp(array[0].y, array[2].y, array3[j].y);
							float x2 = Mathf.Lerp(array2[0].x, array2[2].x, array4[j].x);
							float y = Mathf.Lerp(array2[0].y, array2[2].y, array4[j].y);
							verts.Add(new Vector3(num11, to, 0f));
							uvs.Add(new Vector2(x2, y));
							cols.Add(item);
						}
						continue;
					}
					for (int num12 = 3; num12 > -1; num12--)
					{
						num11 = Mathf.Lerp(array[0].x, array[2].x, array3[num12].x);
						to = Mathf.Lerp(array[0].y, array[2].y, array3[num12].y);
						float x3 = Mathf.Lerp(array2[0].x, array2[2].x, array4[num12].x);
						float y2 = Mathf.Lerp(array2[0].y, array2[2].y, array4[num12].y);
						verts.Add(new Vector3(num11, to, 0f));
						uvs.Add(new Vector2(x3, y2));
						cols.Add(item);
					}
				}
				return;
			}
			if (base.fillDirection == FillDirection.Radial360)
			{
				float[] array5 = new float[16]
				{
					0.5f, 1f, 0f, 0.5f, 0.5f, 1f, 0.5f, 1f, 0f, 0.5f,
					0.5f, 1f, 0f, 0.5f, 0f, 0.5f
				};
				Vector2[] array6 = new Vector2[4];
				Vector2[] array7 = new Vector2[4];
				for (int k = 0; k < 4; k++)
				{
					array6[0] = new Vector2(0f, 0f);
					array6[1] = new Vector2(0f, 1f);
					array6[2] = new Vector2(1f, 1f);
					array6[3] = new Vector2(1f, 0f);
					array7[0] = new Vector2(0f, 0f);
					array7[1] = new Vector2(0f, 1f);
					array7[2] = new Vector2(1f, 1f);
					array7[3] = new Vector2(1f, 0f);
					if (base.invert)
					{
						if (k > 0)
						{
							Rotate(array6, k);
							Rotate(array7, k);
						}
					}
					else if (k < 3)
					{
						Rotate(array6, 3 - k);
						Rotate(array7, 3 - k);
					}
					for (int l = 0; l < 4; l++)
					{
						int num13 = ((!base.invert) ? (k * 4) : ((3 - k) * 4));
						float num14 = array5[num13];
						float to2 = array5[num13 + 1];
						float num15 = array5[num13 + 2];
						float to3 = array5[num13 + 3];
						array6[l].x = Mathf.Lerp(num14, to2, array6[l].x);
						array6[l].y = Mathf.Lerp(num15, to3, array6[l].y);
						array7[l].x = Mathf.Lerp(num14, to2, array7[l].x);
						array7[l].y = Mathf.Lerp(num15, to3, array7[l].y);
					}
					float fill2 = base.fillAmount * 4f - (float)k;
					bool flag2 = k % 2 == 1;
					if (!AdjustRadial(array6, array7, fill2, !flag2))
					{
						continue;
					}
					if (base.invert)
					{
						flag2 = !flag2;
					}
					if (flag2)
					{
						for (int m = 0; m < 4; m++)
						{
							float x4 = Mathf.Lerp(array[0].x, array[2].x, array6[m].x);
							float y3 = Mathf.Lerp(array[0].y, array[2].y, array6[m].y);
							float x5 = Mathf.Lerp(array2[0].x, array2[2].x, array7[m].x);
							float y4 = Mathf.Lerp(array2[0].y, array2[2].y, array7[m].y);
							verts.Add(new Vector3(x4, y3, 0f));
							uvs.Add(new Vector2(x5, y4));
							cols.Add(item);
						}
						continue;
					}
					for (int num16 = 3; num16 > -1; num16--)
					{
						float x6 = Mathf.Lerp(array[0].x, array[2].x, array6[num16].x);
						float y5 = Mathf.Lerp(array[0].y, array[2].y, array6[num16].y);
						float x7 = Mathf.Lerp(array2[0].x, array2[2].x, array7[num16].x);
						float y6 = Mathf.Lerp(array2[0].y, array2[2].y, array7[num16].y);
						verts.Add(new Vector3(x6, y5, 0f));
						uvs.Add(new Vector2(x7, y6));
						cols.Add(item);
					}
				}
				return;
			}
		}
		for (int n = 0; n < 4; n++)
		{
			verts.Add(array[n]);
			uvs.Add(array2[n]);
			cols.Add(item);
		}
	}
}
