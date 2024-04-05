namespace RogueFPS;

public partial class CrouchMechanic : PlayerMechanic
{
	public override bool ShouldBecomeActive()
	{
		return Input.Down( "Duck" ) && !HasAnyTag( "sprint", "slide" );
	}

	public override IEnumerable<string> GetTags()
	{
		yield return "crouch";
	}

	public override float? GetEyeHeight()
	{
		return -32.0f;
	}

	public override float? GetSpeed()
	{
		return 65.0f;
	}
}
