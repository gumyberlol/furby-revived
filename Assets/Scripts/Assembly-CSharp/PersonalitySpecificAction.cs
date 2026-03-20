using Furby;
using HutongGames.PlayMaker;
using Relentless;

[Tooltip("Instructs Furby to perform a personality-specific action")]
[ActionCategory(ActionCategory.Audio)]
public class PersonalitySpecificAction : FsmStateAction
{
	[Tooltip("Gobbler specific action")]
	public FurbyAction m_GobblerPersonality;

	[Tooltip("Kooky specific action")]
	public FurbyAction m_KookyPersonality;

	[Tooltip("RockStar specific action")]
	public FurbyAction m_RockStarPersonality;

	[Tooltip("SweetBelle specific action")]
	public FurbyAction m_SweetBellePersonality;

	[Tooltip("ToughGirl specific action")]
	public FurbyAction m_ToughGirlPersonality;

	public override void Reset()
	{
		m_GobblerPersonality = FurbyAction.Purr;
		m_KookyPersonality = FurbyAction.Purr;
		m_RockStarPersonality = FurbyAction.Purr;
		m_SweetBellePersonality = FurbyAction.Purr;
		m_ToughGirlPersonality = FurbyAction.Purr;
	}

	public override void OnEnter()
	{
		switch (Singleton<FurbyDataChannel>.Instance.FurbyStatus.Personality)
		{
		case FurbyPersonality.Gobbler:
			Singleton<FurbyDataChannel>.Instance.PostAction(m_GobblerPersonality, null);
			break;
		case FurbyPersonality.Kooky:
			Singleton<FurbyDataChannel>.Instance.PostAction(m_KookyPersonality, null);
			break;
		case FurbyPersonality.Base:
			Singleton<FurbyDataChannel>.Instance.PostAction(m_RockStarPersonality, null);
			break;
		case FurbyPersonality.SweetBelle:
			Singleton<FurbyDataChannel>.Instance.PostAction(m_SweetBellePersonality, null);
			break;
		case FurbyPersonality.ToughGirl:
			Singleton<FurbyDataChannel>.Instance.PostAction(m_ToughGirlPersonality, null);
			break;
		}
		Finish();
	}
}
