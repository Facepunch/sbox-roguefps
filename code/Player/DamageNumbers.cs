using Sandbox;
using Sandbox.Utility;
using static System.Net.Mime.MediaTypeNames;

namespace RogueFPS;

public partial class DamageNumbers : Component
{
	[RequireComponent]
	public CameraComponent Camera { get; set; }

	IDisposable renderHook;

	protected override void OnEnabled()
	{
		renderHook?.Dispose();

		renderHook = Camera.AddHookAfterUI( "DamageNumbers", 700, RenderEffect );
	}

	protected override void OnDisabled()
	{
		renderHook?.Dispose();
		renderHook = null;
	}

	struct DamageNumber
	{
		public int Amount;
		public Vector3 WorldPosition;
		public RealTimeSince TimeSince;
	}

	static List<DamageNumber> Numbers = new();

	public static void Add( int amount, Vector3 position )
	{
		Numbers.Add( new DamageNumber() { Amount = amount, WorldPosition = position, TimeSince = 0 } );
	}

	protected override void OnUpdate()
	{
		for ( int i = Numbers.Count - 1; i >= 0; i-- )
		{
			if ( Numbers[i].TimeSince >= TimeToLive )
			{
				Numbers.RemoveAt( i );
			}
		}
	}

	public float TimeToLive => 2.0f;
	public float RiseAmount => 64.0f;

	public void RenderEffect( SceneCamera camera )
	{
		foreach ( var number in Numbers )
		{
			var screenPos = camera.ToScreen( number.WorldPosition );

			// Piss around with different Easing methods
			var ease = Easing.ExpoOut( number.TimeSince / TimeToLive );
			screenPos -= Vector2.Up * ease * 64;

			var color = Color.White.WithAlpha( 1.0f - Easing.EaseOut( number.TimeSince / TimeToLive ) );

			Graphics.DrawText( screenPos, number.Amount.ToString(), color, "Quantico", 32, 800 );
		}
	}
}
