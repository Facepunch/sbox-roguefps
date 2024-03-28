using Sandbox;

public sealed class CameraShake : Component
{
	[Property] public CameraComponent Camera { get; set; }

	bool isShaking = false;
	[Property] public float intensity { get; set; } = 1.0f;

	TimeSince timeSinceShake = 0;

	protected override void OnUpdate()
	{
		if ( !isShaking ) return;

		if(timeSinceShake > 0.15f)
		{
			isShaking = false;
			return;
		}

		Camera.Transform.Position = Camera.Transform.Position + new Vector3( Random.Shared.Float( -intensity, intensity ), Random.Shared.Float( -intensity, intensity ), Random.Shared.Float( -intensity, intensity ) );
		Log.Info( "Shaking" );
	}

	public void Shake()
	{
		isShaking = true;
		timeSinceShake = 0;
	}
}
