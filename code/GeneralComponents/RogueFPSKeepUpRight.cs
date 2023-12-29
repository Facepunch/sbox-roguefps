using Sandbox;
using System.Numerics;

public sealed class RogueFPSKeepUpRight : Component
{
	protected override void OnUpdate()
	{
		base.OnUpdate();

		// Apply the new rotation to the GameObject
		GameObject.Transform.Rotation = Rotation.Identity;
	}
}
