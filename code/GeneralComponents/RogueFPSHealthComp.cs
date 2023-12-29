using Sandbox;
using System.Threading.Tasks;

public sealed class RogueFPSHealthComp : Component
{
	[Property]
	public float Health { get; set; } = 100f;

	[Property]
	public int DestructionDelayMilliseconds { get; set; } = 1000; // Delay in milliseconds before destruction

	[Property]
	public GameObject BreakParticle { get; set; }

	public void Damaged( float damage )
	{
		Health -= damage;
		if ( Health <= 0f )
		{
			if ( BreakParticle != null )
			{
				SceneUtility.Instantiate( BreakParticle, GameObject.Transform.Position, GameObject.Transform.Rotation );
			}

			// Call the method that starts the destruction process with a delay
			StartDestructionSequence();
		}
	}

	private async void StartDestructionSequence()
	{
		// Wait for the specified delay
		await Task.Delay( DestructionDelayMilliseconds );
		// After the delay, destroy the game object
		GameObject.Destroy();
	}

	protected override void OnUpdate()
	{
		if( Health <= 0f )
		{
			StartDestructionSequence();
		}
		
	}
}
