namespace RogueFPS;

public abstract partial class Agent : Actor, Actor.IReceptor
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

		Scene.BroadcastStimulus( new FriendGotHurtStimulus( Transform.Position ) );
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
		if ( eventName == "damage" )
		{
			var damageInfo = (DamageInfo)obj[0];
			LastStimulus = new HurtStimulus( Transform.Position );
		}

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
		FindStimuli();
		
		// Kill off stimuli if it's based on old information
		if ( LastStimulus is not null && !LastStimulus.ShouldReact( this ) )
		{		
			LastStimulus = null;
		}
	}

	/// <summary>
	/// The last stimulus info this actor took.
	/// </summary>
	public Stimulus LastStimulus { get; set; }

	public void OnStimulusReceived( Stimulus stimulusInfo )
	{
		if ( stimulusInfo.HasExpired )
			return;


		if ( stimulusInfo.ShouldReact( this ) )
		{
			LastStimulus = stimulusInfo;
		}
	}

	public virtual bool Hates( Actor other )
	{
		return true;
	}
}
