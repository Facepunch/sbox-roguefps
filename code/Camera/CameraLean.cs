using Sandbox;
using System.Numerics;
using System;

[Title( "Camera Lean" )]
[Category( "Player Camera" )]
[Icon( "camera_roll" )]
public class CameraLean : Component
{
	[Property]
	public CharacterController PlayerController { get; set; }
	public float StrafeTiltAmount { get; set; } = 5.0f;
	private float currentTilt = 0.0f;

	protected override void OnUpdate()
	{

		var strafeSpeed = PlayerController.Velocity.Length;
		if ( strafeSpeed <= 20 ) return;

		float targetTilt = 0.0f;
		if ( Input.Down( "Left" ) ) targetTilt -= StrafeTiltAmount;
		if ( Input.Down( "Right" ) ) targetTilt += StrafeTiltAmount;

		currentTilt = MathX.Lerp( currentTilt, targetTilt, Time.Delta * 5.0f );

		ApplyCameraLean( currentTilt );
	}

	private void ApplyCameraLean( float angle )
	{
		var camera = GameObject.Components.Get<CameraComponent>();
		if ( camera != null )
		{
			camera.Transform.Rotation = camera.Transform.Rotation * Rotation.From( new Angles( 0, 0, angle ) );
		}
	}
}
