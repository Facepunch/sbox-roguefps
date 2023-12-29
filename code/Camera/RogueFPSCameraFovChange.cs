using Sandbox;

[Title( "Rogue FPS FOV Speed" )]
[Category( "Rogue FPS Camera" )]
[Icon( "camera", "red", "white" )]
public sealed class RogueFPSCameraFovChange : Component
{
	[Property]
	public RogueFPSCharacterController PlayerController { get; set; }

	private float _fov;

	protected override void OnStart()
	{
		base.OnStart();

		var camera = GameObject.Components.Get<CameraComponent>();
		_fov = camera.FieldOfView;
	}

	protected override void OnUpdate()
	{
		var camera = GameObject.Components.Get<CameraComponent>();
		var playerSpeed = PlayerController.Velocity.Length;

		camera.FieldOfView = camera.FieldOfView.LerpTo( _fov + playerSpeed * 0.05f, Time.Delta * 10f ).Clamp( _fov, _fov + 40f );
		
	}
}
