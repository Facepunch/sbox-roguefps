using Sandbox;

public sealed class CameraShake : Component
{
	[Property] public CameraComponent Camera { get; set; }

	private bool isShaking = false;
	private TimeSince timeSinceShakeStarted;
	public float Intensity { get; set; } = 1.0f;
	public float DecayRate { get; set; } = 6.5f;
	public float Frequency { get; set; } = 10.0f;

	protected override void OnUpdate()
	{
		if ( !isShaking ) return;

		var elapsed = timeSinceShakeStarted;
		if ( elapsed > 1f )
		{
			isShaking = false;
			return;
		}

		var currentIntensity = Intensity * MathF.Exp( -DecayRate * elapsed );

		var shakePattern = MathF.Sin( Frequency * elapsed );

		Camera.Transform.Position += new Vector3(
			shakePattern * Random.Shared.Float( -currentIntensity, currentIntensity ),
			shakePattern * Random.Shared.Float( -currentIntensity, currentIntensity ),
			Random.Shared.Float( -currentIntensity, currentIntensity )
		);
	}

	public void Shake()
	{
		isShaking = true;
		timeSinceShakeStarted = 0;
	}
}
