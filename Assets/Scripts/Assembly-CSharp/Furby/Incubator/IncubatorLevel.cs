using System;
using System.Collections.Generic;
using UnityEngine;

namespace Furby.Incubator
{
	[Serializable]
	public class IncubatorLevel
	{
		public enum InteractionType
		{
			Cold = 0,
			Scared = 1,
			Dusty = 2,
			Imprinting = 3,
			Hatching = 4
		}

		public class Interaction
		{
			public int ImprintIndex;

			public InteractionType Type;

			public float Time;

			public bool IsAttentionType
			{
				get
				{
					return Type < InteractionType.Hatching;
				}
			}

			public DateTime GetDateTime(float internalTime)
			{
				return DateTime.Now.AddSeconds(Time - internalTime);
			}
		}

		public int LevelIndex;

		public int IncubationTime = 60;

		public float MinAttentionInterval = 10f;

		public float MaxAttentionInterval = 20f;

		public int ImprintCount = 1;

		public IEnumerable<Interaction> GenerateInteractions()
		{
			List<Interaction> list = new List<Interaction>();
			float a = 10f;
			float num = GetImprintCount();
			float num2 = (float)IncubationTime / num;
			float num3 = Mathf.Min(a, num2 * 0.5f);
			float num4 = num2 - num3;
			float attentionDuration = num4 * num;
			foreach (Interaction item in GenerateAttentionPoints(attentionDuration))
			{
				Interaction interaction = item;
				float num5 = 0.5f + Mathf.Floor(item.Time / num4);
				interaction.Time += num5 * num3;
				list.Add(interaction);
			}
			foreach (Interaction item2 in GenerateImprintPoints())
			{
				list.Add(item2);
			}
			list.Sort((Interaction x, Interaction y) => x.Time.CompareTo(y.Time));
			return list;
		}

		public IEnumerable<Interaction> GenerateImprintPoints()
		{
			for (int i = 1; i < GetImprintCount(); i++)
			{
				float imprintSlice = i * IncubationTime;
				yield return new Interaction
				{
					Type = InteractionType.Imprinting,
					Time = imprintSlice / (float)GetImprintCount(),
					ImprintIndex = i
				};
			}
			yield return new Interaction
			{
				Type = InteractionType.Hatching,
				ImprintIndex = GetImprintCount(),
				Time = IncubationTime
			};
		}

		private IEnumerable<Interaction> GenerateAttentionPoints(float attentionDuration)
		{
			float attentionTime = GetRandomAttentionInterval();
			int attentionType = UnityEngine.Random.Range(0, 3);
			while (0f < MaxAttentionInterval && attentionTime < attentionDuration)
			{
				Interaction interaction = new Interaction();
				switch (attentionType % 3)
				{
				case 0:
					interaction.Type = InteractionType.Cold;
					break;
				case 1:
					interaction.Type = InteractionType.Dusty;
					break;
				default:
					interaction.Type = InteractionType.Scared;
					break;
				}
				interaction.ImprintIndex = int.MinValue;
				interaction.Time = attentionTime;
				attentionTime += GetRandomAttentionInterval();
				attentionType += UnityEngine.Random.Range(1, 3);
				yield return interaction;
			}
		}

		private float GetRandomAttentionInterval()
		{
			float min = Mathf.Max(1f, MinAttentionInterval);
			float max = Mathf.Max(1f, MaxAttentionInterval);
			return UnityEngine.Random.Range(min, max);
		}

		public int GetImprintCount()
		{
			return Mathf.Max(1, ImprintCount);
		}
	}
}
