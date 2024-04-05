namespace RogueFPS;

[Title( "Chase" )]
public partial class ChasingState : StateMachine.State
{
	/// <summary>
	/// Should we be forcing into this state? Not super happy with this but it's event based.
	/// </summary>
	private bool forceIntoState = false;

	public override bool ShouldEnterState( StateMachine machine )
	{
		// Forcing into state bypasses every other check
		if ( forceIntoState )
			return true;

		// Only if we've been in another state for a second
		if ( machine.TimeInState < 1f )
			return false;

		return true;
	}

	public override void Tick()
	{
		var path = Agent.GetPath( Agent.LastStimulus.Position );
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

			if ( distance > 0 ) // Make sure we're not already at the target
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
		}
		else
		{
			// Optionally handle the case where there is no path or the target cannot be reached
			Agent.WishMove = Vector3.Zero; // Stop moving if there's no target/path
		}
	}

	public override void OnStateEnter( StateMachine.State prev )
	{
		base.OnStateEnter( prev );
		forceIntoState = false;
	}

	public override void OnEvent( string eventName, params object[] obj )
	{
		if ( eventName == "damage" )
		{
			forceIntoState = true;
		}
	}
}
