using System.Runtime.Intrinsics.X86;

namespace RogueFPS;

/// <summary>
/// A health component for any kind of GameObject.
/// </summary>
public partial class HealthComponent : Component
{
	private float health = 100f;

	/// <summary>
	/// An action (mainly for ActionGraphs) to respond to when a GameObject's health changes.
	/// </summary>
	[Property] public Action<float, float> OnHealthChanged { get; set; }

	/// <summary>
	/// What health is this GameObject?
	/// </summary>
	[Property] public float Health
	{ 
		get => health; 

		set
		{
			var currentHealth = health;
			health = value;
			HealthChanged( currentHealth, health );
		}
	}

	/// <summary>
	/// Called when Health is changed.
	/// </summary>
	/// <param name="oldValue"></param>
	/// <param name="newValue"></param>
	protected void HealthChanged( float oldValue, float newValue )
	{
		OnHealthChanged?.Invoke( oldValue, newValue );
	}

	/// <summary>
	/// Called when this GameObject is damaged by something/someone.
	/// </summary>
	/// <param name="info"></param>
	public virtual void OnTakeDamage( ref DamageInfo info )
	{
		// Make sure the victim is THIS GameObject, always.
		info.Victim = GameObject;
	}
}
