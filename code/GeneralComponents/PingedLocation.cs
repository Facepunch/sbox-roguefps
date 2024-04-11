using Sandbox;

public sealed class PingedLocation : Component
{
	RealTimeSince pingTime = 0;
	float pingDuration = 5.0f;
	protected override void OnUpdate()
	{
		if(pingTime > pingDuration)
		{
			GameObject.Destroy();
		}
	}
}
