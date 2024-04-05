namespace RogueFPS;

public partial class Actor
{
	/// <summary>
	/// Maintains a list of mechanics that are associated with this player controller.
	/// </summary>
	public IEnumerable<ActorMechanic> Mechanics => Components.GetAll<ActorMechanic>( FindMode.EnabledInSelfAndDescendants ).OrderBy( x => x.Priority );

	protected float? CurrentSpeedOverride;
	protected float? CurrentEyeHeightOverride;
	protected float? CurrentFrictionOverride;
	protected float? CurrentAccelerationOverride;
	protected bool LockMovementOverride;
	protected bool LockMouseMovementOverride;

	ActorMechanic[] ActiveMechanics = { };

	public ITagSet MechanicTags { get; protected set; } = new TagSet();

	/// <summary>
	/// Is a mechanic active?
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public bool IsMechanicActive<T>() where T : ActorMechanic
	{
		return ActiveMechanics.OfType<T>()
			.Any( x => x.Active );
	}

	/// <summary>
	/// Gets a mechanic of the specified type.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public T GetMechanic<T>() where T : ActorMechanic
	{
		return Mechanics.OfType<T>().FirstOrDefault();
	}

	/// <summary>
	/// Called on <see cref="OnUpdate"/>.
	/// </summary>
	protected virtual void DoMechanicsUpdate()
	{
		var lastUpdate = ActiveMechanics;
		var sortedMechanics = Mechanics.Where( x => x.ShouldBecomeActive() || !x.ShouldBecomeInactive() );

		// Copy the previous update's tags so we can compare / send tag changed events later.
		var previousUpdateTags = MechanicTags;

		// Clear the current tags
		var currentTags = new TagSet();

		float? speedOverride = null;
		float? eyeHeightOverride = null;
		float? frictionOverride = null;
		float? accelerationOverride = null;
		bool lockMovementOverride = false;
		bool lockMouseOverride = false;

		foreach ( var mechanic in sortedMechanics )
		{
			mechanic.IsActive = true;
			mechanic.OnActiveUpdate();

			// Add tags where we can
			mechanic.GetTags()
				.ToList()
				.ForEach( currentTags.Add );

			var eyeHeight = mechanic.GetEyeHeight();
			var speed = mechanic.GetSpeed();
			var friction = mechanic.GetGroundFriction();
			var acceleration = mechanic.GetAcceleration();
			var lockMovement = mechanic.LockMovement;
			var lockMouse = mechanic.LockMouseMovement;

			mechanic.BuildWishInput( ref WishMove );
			if ( speed is not null ) speedOverride = speed;
			if ( eyeHeight is not null ) eyeHeightOverride = eyeHeight;
			if ( friction is not null ) frictionOverride = friction;
			if ( acceleration is not null ) accelerationOverride = acceleration;

			if ( lockMovement ) lockMovementOverride = true;
			if ( lockMouse ) lockMouseOverride = true;
		}

		ActiveMechanics = sortedMechanics.ToArray();

		if ( lastUpdate is not null )
		{
			foreach ( var mechanic in lastUpdate?.Except( sortedMechanics ) )
			{
				// This mechanic shouldn't be active anymore
				mechanic.IsActive = false;
			}
		}

		CurrentSpeedOverride = speedOverride;
		CurrentEyeHeightOverride = eyeHeightOverride;
		CurrentFrictionOverride = frictionOverride;
		CurrentAccelerationOverride = accelerationOverride;
		LockMovementOverride = lockMovementOverride;
		LockMouseMovementOverride = lockMouseOverride;

		MechanicTags.SetFrom( currentTags );
	}
}
