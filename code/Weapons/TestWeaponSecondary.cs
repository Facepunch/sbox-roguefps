using Sandbox;

public class TestWeaponSecondary : BaseWeaponItem
{
	public override int MaxUseCount { get; set; } = 2;
	public override bool ReloadAfterUse { get; set; } = true;
	protected override void OnUpdate()
	{
		base.OnUpdate();

	}
}
