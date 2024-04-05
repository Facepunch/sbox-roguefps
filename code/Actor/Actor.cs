namespace RogueFPS;

/// <summary>
/// An actor. This could be an AI actor, or a player.
/// </summary>
public partial class Actor : Component, Component.IDamageable
{
	/// <summary>
	/// How much health does this actor have?
	/// </summary>
	[Property] public float Health { get; set; } = 100f;

	/// <summary>
	/// Is this actor alive?
	/// </summary>
	[Property] public bool IsAlive { get; set; } = true;

	/// <summary>
	/// The GameObject for this actor's Camera (if any)
	/// This can also be known as the actor's eyes. So where they're looking from.
	/// </summary>
	public virtual GameObject CameraObject { get; }

	/// <summary>
	/// The last damage info this actor took.
	/// </summary>
	public Sandbox.DamageInfo LastDamage { get; set; }

	/// <summary>
	/// When did this actor take damage last?
	/// </summary>
	public TimeSince TimeSinceLastDamage { get; set; } = 100f;

	/// <summary>
	/// Called every mechanics update.
	/// </summary>
	protected virtual void OnMechanicsUpdate()
	{
	}

	/// <summary>
	/// Should we actually inflict damage?
	/// </summary>
	/// <param name="damage"></param>
	/// <returns></returns>
	public virtual bool ShouldDamage( in Sandbox.DamageInfo damage )
	{
		return true;
	}

	/// <summary>
	/// Called when the actor is damaged.
	/// </summary>
	public virtual void OnDamage( in Sandbox.DamageInfo damage )
	{
		if ( !ShouldDamage( damage ) )
		{
			// Make sure damgeinfo knows
			damage.Damage = 0;
			return;
		}

		TimeSinceLastDamage = 0;
		LastDamage = damage;

		Health -= damage.Damage;

		TriggerEvent( "damage", damage );

		TimeSinceLastDamage = 0;

		if ( Health <= 0 )
		{
			OnKilled( damage );
		}
	}

	/// <summary>
	/// Called when the actor is killed.
	/// </summary>
	/// <param name="damageInfo"></param>
	public virtual void OnKilled( in Sandbox.DamageInfo damageInfo )
	{
		IsAlive = false;
	}

	/// <summary>
	/// Called every Scene update
	/// </summary>
	protected override void OnUpdate()
	{
		OnMechanicsUpdate();
	}

	public interface IEventListener
	{
		public void OnEvent( Actor actor, string eventName, params object[] obj );
	}

	/// <summary>
	/// Triggers an event on this actor. This is pushed down to mechanics, or any state machines.
	/// </summary>
	/// <param name="eventName"></param>
	/// <param name="obj"></param>
	public void TriggerEvent( string eventName, params object[] obj )
	{
		foreach ( var eventListener in Scene.GetAllComponents<IEventListener>() )
		{
			eventListener.OnEvent( this, eventName, obj );
		}

		foreach ( var mechanic in Mechanics )
		{
			mechanic.OnEvent( eventName, obj );
		}

		OnEvent( eventName, obj );
	}

	/// <summary>
	/// Called when an event is triggered by <see cref="TriggerEvent(string, object[])"/>
	/// </summary>
	/// <param name="eventName"></param>
	/// <param name="obj"></param>
	public virtual void OnEvent( string eventName, params object[] obj )
	{
	}

	// TODO: move this
	protected readonly float DefaultDetectionRange = 600f;

	/// <summary>
	/// How detected are we in relation to a specific actor?
	/// Maybe this belongs in its own component. Idk.
	/// </summary>
	/// <param name="actor"></param>
	/// <returns></returns>
	public virtual float GetDetectionFactor( Actor actor )
	{
		return DefaultDetectionRange - Transform.Position.Distance( actor.Transform.Position );
	}

	protected override void OnEnabled()
	{
		Tags.Add( "actor" );
	}
}
