namespace RogueFPS;

public abstract partial class Agent : Actor
{
	/// <summary>
	/// An action called when the NPC is damaged by something.
	/// </summary>
	[Property, Category( "Actions" )] public Action< Sandbox.DamageInfo> OnDamageAction { get; set; }

	/// <summary>
	/// An action called when the NPC is killed by something.
	/// </summary>
	[Property, Category( "Actions" )] public Action<Sandbox.DamageInfo> OnKilledAction { get; set; }

	/// <summary>
	/// The state machine this NPC is using.
	/// </summary>
	[Property] public StateMachine StateMachine { get; set; }

	/// <summary>
	/// Ragdoll
	/// </summary>
	[Property] public ModelPhysics Physics { get; set; }
	[Property] public ModelCollider Collider { get; set; }

	/// <summary>
	/// Model
	/// </summary>
	[Property] public SkinnedModelRenderer Model { get; set; }

	[Property] public GameObject CameraGameObject { get; set; }
	public override GameObject CameraObject => CameraGameObject;

	[Property] public bool NoReactionToSound { get; set; } = false;

	public override void OnDamage( in Sandbox.DamageInfo damage )
	{
		base.OnDamage( damage );

		if ( OnDamageAction != null ) OnDamageAction?.Invoke( damage );

		if ( Model is not null )
		{
			var health = Health.Remap( 0, 100, 1, 0 );
			Model?.SceneObject.Attributes.Set( "bloodamount", health );
		}
	}

	public override void OnKilled( in Sandbox.DamageInfo damage )
	{
		base.OnKilled( damage );

		if ( OnKilledAction != null ) OnKilledAction?.Invoke( damage );

		if ( Collider is not null )
		{
			Collider.Enabled = false;
		}

		GameObject.Destroy();
	}

	public override void OnEvent( string eventName, params object[] obj )
	{
		StateMachine?.OnEvent( eventName, obj );
	}

	protected override void UpdateMovement()
	{
		BuildWishInput();
		DoMechanicsUpdate();
		StateMachine?.UpdateStateMachine();
		BuildWishVelocity();
		Accelerate();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		UpdateMovement();
	}

	public virtual bool Hates( Actor other )
	{
		return true;
	}
}
