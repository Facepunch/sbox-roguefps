namespace RogueFPS;

[Title( "Idle" )]
public partial class IdleState : StateMachine.State
{
	public override bool ShouldEnterState( StateMachine machine )
	{
		if ( Agent.LastStimulus is null || Agent.LastStimulus.HasExpired )
			return true;

		return false;
	}
}
