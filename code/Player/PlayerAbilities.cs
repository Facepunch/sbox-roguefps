public sealed class PlayerAbilities : Component
{	
	[Property ] public List<BaseAbilityItem> Abilities { get; set; } = new();

	protected override void OnEnabled()
	{
		base.OnEnabled();
	}
}
public struct AbilityItem
{
	public BaseAbilityItem Ability;
	public InputType InputType;

}
