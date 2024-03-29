namespace RogueFPS;

public sealed class CameraController : Component
{
	/// <summary>
	/// A reference to the camera component we're going to be doing stuff with.
	/// </summary>
	[Property] public CameraComponent Camera { get; set; }

	/// <summary>
	/// Hide the local player's body?
	/// </summary>
	[Property] public bool HideBody { get; set; } = true;

	[Property] public Controller PlayerController { get; set; }

	/// <summary>
	/// Constructs a ray using the camera's GameObject
	/// </summary>
	public Ray AimRay => new Ray( Camera.Transform.Position + Camera.Transform.Rotation.Forward * 25f, Camera.Transform.Rotation.Forward );

	protected override void OnStart()
	{
		// Make sure the camera is disabled if we're not actively in charge of it.
		// Note: let's figure out spectator stuff in a nice way
		Camera.Enabled = !IsProxy;

		// If the camera is enabled, let's get rid of the player's body, otherwise it's gonna be in the way.
		if ( Camera.Enabled && HideBody )
		{
			var playerController = PlayerController;
			//if ( playerController == null ) throw new ComponentNotFoundException( "CameraController - couldn't find PlayerController component." );

			// Disable the player's body so it doesn't render.
			var skinnedModels = playerController.Components.GetAll<SkinnedModelRenderer>( FindMode.EverythingInSelfAndChildren );

			foreach ( var skinnedModel in skinnedModels )
			{
				skinnedModel.RenderType = ModelRenderer.ShadowRenderType.ShadowsOnly;
			}
		}
	}

	protected override void OnUpdate()
	{
		ApplyRecoil();
	}

	void ApplyRecoil()
	{
		/*
		if ( PlayerController.Weapon.GetFunction<RecoilFunction>() is var fn )
		{
			PlayerController.EyeAngles += fn.Current;
		}
		*/
	}
}
