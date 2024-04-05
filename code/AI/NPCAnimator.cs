using Sandbox;

namespace Opium.AI;

[Title( "NPC Animator" )]
public partial class NPCAnimator : Component
{
	[Property] public Agent Agent { get; set; }
	[Property] public string AnimationState { get; set; } = "idle";

	protected static int StateFromName( string name )
	{
		return name switch
		{
			"alert" => 1,
			"chase" => 2,
			"attack" => 3,
			"block_stun" => 4,
			"attack_stun" => 5,
			"blocking" => 6,
			_ => 0
		};
	}

	protected override void OnUpdate()
	{
		var stateMachine = Agent?.StateMachine;

		if ( stateMachine.CurrentState is null )
		{
			Agent?.Model?.Set( "state", 0 );
			return;
		}

		Agent?.Model?.Set( "state", 0 );

		Agent?.Model?.Set( "move_x", Agent.WishMove.y );
		Agent?.Model?.Set( "move_y", Agent.WishMove.x );
	}
}
