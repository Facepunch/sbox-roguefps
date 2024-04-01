using Sandbox;
using System.Numerics;
using System;

[Title( "Item Animator" )]
[Category( "Rogue FPS" )]
[Icon( "swap_horizontal_circle", "red", "white" )]
public sealed class ItemAnimator : Component
{
	private ModelRenderer Model;

	public float BounceHeight = 0.5f;
	public float RotationSpeed = 40f; // Degrees per second
	public float BounceSpeed = 2f;

	private Vector3 initialPosition;
	private float rotationAngle = 0f; // Keep track of the rotation angle

	protected override void OnStart()
	{
		Model = GameObject.Components.Get<ModelRenderer>();
		if ( Model != null )
		{
			initialPosition = Model.Transform.Position;
		}
	}

	protected override void OnUpdate()
	{
		if ( Model != null )
		{
			// Update the rotation angle based on the speed and the time elapsed
			rotationAngle += RotationSpeed * RealTime.Delta;
			rotationAngle %= 360; // Keep the angle within 0-360 degrees

			// Manually create a rotation quaternion
			float radians = rotationAngle * (float)Math.PI / 180f;
			float sin = MathF.Sin( radians / 2 );
			float cos = MathF.Cos( radians / 2 );

			// Assuming Y-axis rotation, change if necessary
			Quaternion rotation = new Quaternion( sin / 4, 0, sin, cos );

			// Apply the rotation
			Model.Transform.Rotation = rotation;

			// Bounce using a sine wave based on the time
			float bounce = MathF.Sin( RealTime.Now * BounceSpeed ) * BounceHeight;
			Model.Transform.Position += new Vector3( 0, 0, bounce / 10 );
		}
	}
}
