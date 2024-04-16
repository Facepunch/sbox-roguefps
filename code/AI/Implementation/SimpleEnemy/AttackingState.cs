namespace RogueFPS;

public partial class AttackingState : ChasingState
{
	[Property] public float AttackRange { get; set; } = 256f;

	public override bool ShouldEnterState( StateMachine machine )
	{
		return GetTarget().Transform.Position.Distance( Agent.Transform.Position ) < AttackRange;
	}
}
