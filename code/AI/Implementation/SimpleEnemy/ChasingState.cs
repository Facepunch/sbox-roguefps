namespace RogueFPS;

[Title( "Chase" )]
public partial class ChasingState : StateMachine.State
{
	[Property] public float StoppingDistance { get; set; } = 256f;

	public override bool ShouldEnterState( StateMachine machine )
	{
		if ( machine.CurrentState is AttackingState && machine.TimeInState < 1f ) return false;

		return true;
	}

	protected Actor GetTarget()
	{
		return Agent.GetAllPlayers().OrderBy( x => x.Transform.Position.DistanceSquared( Agent.Transform.Position ) ).FirstOrDefault();
	}

	public override void Tick()
	{
		var targetActor = GetTarget();

		if ( !targetActor.IsValid() ) return;

		var path = Agent.GetPath( targetActor.Transform.Position );
		var targetIndex = 0;

		for ( int i = 0; i < path.Count; ++i )
		{
			var pDist = path[i].Distance( Agent.Transform.Position );
			var pDistRaised = path[i].WithZ( Agent.Transform.Position.z ).Distance( Agent.Transform.Position );
			var zDist = MathF.Abs( path[i].z - Agent.Transform.Position.z );

			if ( pDist > 4f )
			{
				targetIndex = i;
				break;
			}
		}

		for ( int i = targetIndex; i < path.Count - 1; ++i )
		{
			var a = path[i];
			var b = path[i + 1];

			if ( StateMachine.DebugEnabled )
			{
				Gizmo.Draw.IgnoreDepth = true;
				Gizmo.Draw.Line( a, b );
				Gizmo.Draw.LineSphere( a, 4f );
			}
		}

		if ( path.Count > 0 ) // Make sure the path is not empty
		{
			var target = path[targetIndex];
			var globalDirection = (target - Agent.Transform.Position); // Global direction to the target
			var distance = globalDirection.Length;

			if ( distance > StoppingDistance ) // Make sure we're not already at the target
			{
				var localDirection = Agent.Transform.Rotation.Inverse * globalDirection; // Convert to local space
				var direction = localDirection.Normal; // Normalize the direction

				Agent.LookAt( target, 15f ); // Ensure the NPC looks at the target
				Agent.WishMove = direction; // Move towards the target in local space

				if ( StateMachine.DebugEnabled )
				{
					Gizmo.Draw.IgnoreDepth = true;
					Gizmo.Draw.Color = Color.Green;
					Gizmo.Draw.Arrow( Agent.Transform.Position, Agent.Transform.Position + (Agent.Transform.Rotation * localDirection) );
				}
			}
			else
			{
				Agent.LookAt( target, 15f ); // Ensure the NPC looks at the target
				Agent.WishMove = Vector3.Zero; // Stop moving if there's no target/path
			}
		}
		else
		{
			// Optionally handle the case where there is no path or the target cannot be reached
			Agent.WishMove = Vector3.Zero; // Stop moving if there's no target/path
		}
	}
}
