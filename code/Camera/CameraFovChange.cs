using Sandbox;

[Title( "FOV Speed" )]
[Category( "Rogue FPS Camera" )]
[Icon( "camera", "red", "white" )]
public sealed class CameraFovChange : Component
{
	[Property]
	public Controller PlayerController { get; set; }

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
		var playerSpeed = PlayerController.Rigidbody.Velocity.Length;

		camera.FieldOfView = camera.FieldOfView.LerpTo( _fov + playerSpeed * 0.05f, Time.Delta * 10f ).Clamp( _fov, _fov + 40f );
		
	}
}
