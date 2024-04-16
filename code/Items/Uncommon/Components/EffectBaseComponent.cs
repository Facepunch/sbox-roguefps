using Sandbox;

public class EffectBaseComponent : Component
{
	[Property] public virtual string Icon { get; set; } = "ui/ping/ping.png";
	[Property] public virtual string Name { get; set; } = "Effect";
	[Property] public virtual string Description { get; set; } = "This is an effect.";
	public int StackCount { get; set; } = 1;
	public virtual DamageTypes DamageType { get; set; } = DamageTypes.None;
}
