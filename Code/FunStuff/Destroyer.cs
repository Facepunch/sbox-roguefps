using Sandbox;

public sealed class Destroyer : Component
{
	[Property] public float TimeToDestroy { get; set; } = 5f;
	TimeSince TimeSinceSpawned = 0f;
	protected override void OnUpdate()
	{
		if ( TimeSinceSpawned > TimeToDestroy )
		{
			GameObject.Destroy();
		}
	}
}
