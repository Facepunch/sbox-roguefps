using Sandbox;
using System;

[Title( "Camera Bob" )]
[Category( "Rogue FPS Camera" )]
[Icon( "camera", "red", "white" )]
public sealed class CameraBob : Component
{
	[Property]
	public CharacterController PlayerController { get; set; }

	private float bobFrequency = 5.0f; // Speed of the bob
	private float bobHorizontalAmplitude = 10.1f; // Horizontal shift amount
	private float bobVerticalAmplitude = 10.1f; // Vertical shift amount

	private float bobTimer = 0.0f;
	private bool shouldBob = true;

	protected override void OnUpdate()
	{
		if ( !PlayerController.IsOnGround ) return;

		var camera = GameObject.Components.Get<CameraComponent>();
		var playerSpeed = PlayerController.Velocity.Length;

		shouldBob = playerSpeed > 0.1f; 

		if ( shouldBob )
		{
			bobTimer += Time.Delta * bobFrequency;
			ApplyCameraBob( camera, bobTimer );
		}
		else
		{
			bobTimer = 0.0f;
		}
	}

	private void ApplyCameraBob( CameraComponent camera, float timer )
	{
		float horizontalOffset = MathF.Sin( timer ) * bobHorizontalAmplitude;
		float verticalOffset = MathF.Abs( MathF.Cos( timer ) * bobVerticalAmplitude );

		Vector3 bobPosition = new Vector3( horizontalOffset, 0, verticalOffset );

		camera.Transform.LocalPosition += bobPosition;
	}
}
