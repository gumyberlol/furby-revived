using System;

namespace Furby.Scripts.FurMail
{
	[Serializable]
	public class TargetGroupInformation
	{
		public string Language;

		public int NumberOfUniqueBabies;

		public int NumberOfTribesCompleted;

		public bool HasGoldenBaby;

		public bool IsFurbyUser;

		public bool IsNonFurbyUser;

		public override string ToString()
		{
			return string.Format("Language={0},NumberOfUniqueBabies={1},NumberOfTribesCompleted={2},HasGoldenBaby={3},IsFurbyUser={4},IsNonFurbyUser={5}", Language, NumberOfUniqueBabies, NumberOfTribesCompleted, HasGoldenBaby, IsFurbyUser, IsNonFurbyUser);
		}
	}
}
