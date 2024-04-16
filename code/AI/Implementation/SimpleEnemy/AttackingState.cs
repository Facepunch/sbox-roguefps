namespace RogueFPS;

public partial class AttackingState : ChasingState
{
	[Property] public float AttackRange { get; set; } = 256f;

	public BaseWeaponItem GetWeapon()
	{
		var wpn = GameObject.Root.Components.GetAll<BaseWeaponItem>( FindMode.EverythingInSelfAndDescendants ).FirstOrDefault();
		return wpn;
	}

	public override bool ShouldEnterState( StateMachine machine )
	{
		return GetTarget().Transform.Position.Distance( Agent.Transform.Position ) < AttackRange;
	}

	public override void Tick()
	{
		base.Tick();

		var wpn = GetWeapon();
		if ( wpn.IsValid() )
		{
			wpn.DoFire();
		}
	}
}
