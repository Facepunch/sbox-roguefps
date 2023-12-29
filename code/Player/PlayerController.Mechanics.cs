using System.Collections.Immutable;

namespace RogueFPS;

public partial class PlayerController
{
	/// <summary>
	/// Maintains a list of mechanics that are associated with this player controller.
	/// </summary>
	public IEnumerable<BasePlayerControllerMechanic> Mechanics => Components.GetAll<BasePlayerControllerMechanic>( FindMode.EnabledInSelfAndDescendants ).OrderBy( x => x.Priority );

	float? CurrentSpeedOverride;
	float? CurrentEyeHeightOverride;
	float? CurrentFrictionOverride;
	float? CurrentAccelerationOverride;

	BasePlayerControllerMechanic[] ActiveMechanics;

	/// <summary>
	/// Called on <see cref="OnUpdate"/>.
	/// </summary>
	protected void OnUpdateMechanics()
	{
		var lastUpdate = ActiveMechanics;
		var sortedMechanics = Mechanics.Where( x => x.ShouldBecomeActive() || !x.ShouldBecomeInactive() );

		// Copy the previous update's tags so we can compare / send tag changed events later.
		var previousUpdateTags = tags;

		// Clear the current tags
		var currentTags = new List<string>();

		float? speedOverride = null;
		float? eyeHeightOverride = null;
		float? frictionOverride = null;
		float? accelerationOverride = null;

		foreach ( var mechanic in sortedMechanics )
		{
			mechanic.IsActive = true;
			mechanic.OnActiveUpdate();

			// Add tags where we can
			currentTags.AddRange( mechanic.GetTags() );

			var eyeHeight = mechanic.GetEyeHeight();
			var speed = mechanic.GetSpeed();
			var friction = mechanic.GetGroundFriction();
			var acceleration = mechanic.GetAcceleration();

			mechanic.BuildWishInput( ref WishMove );

			if ( speed is not null ) speedOverride = speed;
			if ( eyeHeight is not null ) eyeHeightOverride = eyeHeight;
			if ( friction is not null ) frictionOverride = friction;
			if ( acceleration is not null ) accelerationOverride = acceleration;
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

		tags = currentTags.ToImmutableArray();
	}
}
