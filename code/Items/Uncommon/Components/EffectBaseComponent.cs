using Sandbox;

public class EffectBaseComponent : Component
{
	public virtual DamageTypes DamageType { get; set; } = DamageTypes.None;
}
