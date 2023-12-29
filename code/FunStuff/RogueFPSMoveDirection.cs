using Sandbox;

public sealed class RogueFPSMoveDirection : Component
{
	

	protected override void OnUpdate()
	{
		GameObject.Transform.Position += GameObject.Transform.Rotation.Forward * 2.0f;
	}
}
