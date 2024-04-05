namespace RogueFPS;

public abstract class ActorMechanic : Component
{
	[Property, Group( "Base" )] public Actor Actor { get; set; }
	[Property, Group( "Base" )] public int Priority { get; set; } = 0;
	[Property, Group( "Base" ), ReadOnly] public TimeSince TimeSinceActiveChanged { get; set; } = 1f;

	private bool isActive;
	/// <summary>
	/// Is this mechanic active?
	/// </summary>
	[Property, Group( "Base" ), ReadOnly]
	public bool IsActive
	{
		get => isActive;
		set
		{
			var before = isActive;
			isActive = value;

			if ( isActive != before )
			{
				TimeSinceActiveChanged = 0;
				OnActiveChanged( before, isActive );
			}
		}
	}

	/// <summary>
	/// Try to forcefully activate the mechanic
	/// </summary>
	public virtual void Activate()
	{	
		IsActive = true;
	}

	/// <summary>
	/// Try to forcefully deactivate the mechanic
	/// </summary>
	public virtual void Deactivate()
	{
		IsActive = false;
	}

	protected override void OnAwake()
	{
		// If we don't have the player controller defined, let's have a look for it
		if ( !Actor.IsValid() )
		{
			Actor = Components.Get<Actor>( FindMode.EverythingInSelfAndAncestors );
		}
	}

	/// <summary>
	/// Return a list of tags to be used by the player controller / other mechanics.
	/// </summary>
	/// <returns></returns>
	public virtual IEnumerable<string> GetTags()
	{
		return Enumerable.Empty<string>();
	}

	/// <summary>
	/// An accessor to see if the player controller has a tag.
	/// </summary>
	/// <param name="tag"></param>
	/// <returns></returns>
	public bool HasTag( string tag ) => Actor.MechanicTags.Has( tag );

	/// <summary>
	/// An accessor to see if the player controller has all matched tags.
	/// </summary>
	/// <param name="tags"></param>
	/// <returns></returns>
	public bool HasAllTags( ITagSet tags ) => Actor.MechanicTags.HasAll( tags );

	/// <inheritdoc cref="HasAllTags(ITagSet)"/>
	public bool HasAllTags( params string[] tags )
	{
		var set = new TagSet();
		foreach ( var tag in tags )
		{
			set.Add( tag );
		}
		return HasAllTags( set );
	}

	/// <summary>
	/// An accessor to see if the player controller has any tag.
	/// </summary>
	/// <param name="tags"></param>
	/// <returns></returns>
	public bool HasAnyTag( ITagSet tags ) => Actor.Tags.HasAny( tags );
	
	/// <inheritdoc cref="HasAnyTag(ITagSet)"/>
	public bool HasAnyTag( params string[] tags )
	{
		var set = new TagSet();
		foreach ( var tag in tags )
		{
			set.Add( tag );
		}
		return HasAnyTag( set );
	}

	/// <summary>
	/// Called when <see cref="IsActive"/> changes.
	/// </summary>
	/// <param name="before"></param>
	/// <param name="after"></param>
	protected virtual void OnActiveChanged( bool before, bool after )
	{
		//
	}

	/// <summary>
	/// Called by <see cref="PlayerController"/>, treat this like a Tick/Update while the mechanic is active.
	/// </summary>
	public virtual void OnActiveUpdate()
	{
		//
	}

	/// <summary>
	/// Should we be ticking this mechanic at all?
	/// </summary>
	/// <returns></returns>
	public virtual bool ShouldBecomeActive()
	{
		return false;
	}

	/// <summary>
	/// Should we be inactive?
	/// </summary>
	/// <returns></returns>
	public virtual bool ShouldBecomeInactive()
	{
		return !ShouldBecomeActive();
	}

	/// <summary>
	/// Mechanics can override the player's movement speed.
	/// </summary>
	/// <returns></returns>
	public virtual float? GetSpeed()
	{
		return null;
	}

	/// <summary>
	/// Mechanics can override the player's eye height.
	/// </summary>
	/// <returns></returns>
	public virtual float? GetEyeHeight()
	{
		return null;
	}

	/// <summary>
	/// Mechanics can override the player's ground friction.
	/// </summary>
	public virtual float? GetGroundFriction()
	{
		return null;
	}

	/// <summary>
	/// Mechanics can override the player's acceleration.
	/// </summary>
	/// <returns></returns>
	public virtual float? GetAcceleration()
	{
		return null;
	}

	/// <summary>
	/// Do we lock movement?
	/// </summary>
	public virtual bool LockMovement { get; } = false;


	/// <summary>
	/// Do we lock mouse movement?
	/// </summary>
	public virtual bool LockMouseMovement { get; } = false;

	/// <summary>
	/// Mechanics can override the player's wish input direction.
	/// </summary>
	public virtual void BuildWishInput( ref Vector3 wish )
	{
	}

	public virtual void OnEvent( string eventName, params object[] obj )
	{
	}
}
