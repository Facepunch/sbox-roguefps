using Sandbox;

public sealed class TracerBulletParticle : Component
{
	[Property]
	[Group( "Tracer" )]
	public GameObject Start { get; set; }

	[Property]
	[Group( "Tracer" )]
	public GameObject End { get; set; }

	[Property]
	[Group( "Tracer" )]
	public LineRenderer LineRenderer { get; set; }
	public TimeUntil TimeUntilDestroy { get; set; } = 0.5f;

	protected override void OnStart()
	{
		base.OnStart();

		LineRenderer = GameObject.Components.Get<LineRenderer>();
	}
	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( TimeUntilDestroy > 0 )
		{
			var curve = new Curve(new Curve.Frame(0,0), new Curve.Frame( 1, TimeUntilDestroy.Relative * 5 ) );
			LineRenderer.Width = curve;
		}
		else
		{
			LineRenderer.Width = 0;
			GameObject.Destroy();
		}
	}
}
