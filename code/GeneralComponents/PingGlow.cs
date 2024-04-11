using Sandbox;

public sealed class PingGlow : Component
{
	RealTimeSince glowTimer;
	float glowDuration = 5.0f;

	HighlightOutline _highLight;
	Color lastColor;
	protected override void OnStart()
	{
		base.OnStart();

		_highLight = Components.Get<HighlightOutline>(FindMode.EverythingInSelfAndChildren);
		if ( _highLight != null )
		{
			_highLight.Enabled = true;
			lastColor = _highLight.Color;
		}
		else
		{
			_highLight = Components.Create<HighlightOutline>();
			_highLight.Enabled = true;
		}

		glowTimer = 0;
	}

	protected override void OnUpdate()
	{
		if(glowTimer > glowDuration)
		{
			_highLight.Color = lastColor;
			glowTimer = 0;
			_highLight.Enabled = false;
			Destroy();
		}
		else
		{
			_highLight.Color = Color.Orange;
		}
	}
}
