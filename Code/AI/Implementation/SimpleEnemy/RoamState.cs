using Sandbox.Navigation;

namespace RogueFPS;
[Title( "Roam" )]
public partial class RoamState : ChasingState
{
	[Property] public float AttackRange { get; set; } = 256f;
	[Property] TimeSince timeSinceLastRoam;
	Vector3? lastRoamPoint;

	public BaseWeaponItem GetWeapon()
	{
		var wpn = GameObject.Root.Components.GetAll<BaseWeaponItem>( FindMode.EverythingInSelfAndDescendants ).FirstOrDefault();
		return wpn;
	}

	public override bool ShouldEnterState( StateMachine machine )
	{
		//Get the target actor from the chasing state
		return machine.Components.Get<ChasingState>(FindMode.EverythingInDescendants).targetActor == null;
	}

	public override void Tick()
	{
		if( timeSinceLastRoam > Random.Shared.Float( 10f, 20f ) || !lastRoamPoint.HasValue )
		{
			lastRoamPoint = Agent.GetRandomPoint();
			timeSinceLastRoam = 0;
		}

		var path = Agent.GetPath( lastRoamPoint.Value );
		var targetIndex = 0;

		Gizmo.Draw.Color = Color.Red;
		Gizmo.Draw.SolidSphere( lastRoamPoint.Value, 10f);

		//Get a new randompoint to roam to if the agent is close to the last roam point
		if( targetIndex == path.Count - 1 )
		{
			lastRoamPoint = Agent.GetRandomPoint();
			//timeSinceLastRoam = 0;
			Log.Info( "Getting new random point" );
		}

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
			else
			{
				Agent.LookAt( target, 15f ); // Ensure the NPC looks at the target
				Agent.WishMove = Vector3.Zero; // Stop moving if there's no target/path

				lastRoamPoint = Agent.GetRandomPoint();
			}
		}
		else
		{
			// Optionally handle the case where there is no path or the target cannot be reached
			Agent.WishMove = Vector3.Zero; // Stop moving if there's no target/path
		}

	}
}
