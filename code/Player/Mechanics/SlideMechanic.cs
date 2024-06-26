namespace RogueFPS;

/// <summary>
/// A basic sliding mechanic.
/// </summary>
public partial class SlideMechanic : PlayerMechanic
{
	[Property] public float NextSlideCooldown { get; set; } = 0.5f;
	[Property] public float MinimumSlideLength { get; set; } = 1.0f;
	[Property] public float SlideFriction { get; set; } = 0.01f;
	[Property] public float EyeHeight { get; set; } = -20.0f;
	[Property] public float WishDirectionScale { get; set; } = 0.5f;
	[Property] public float SlideSpeed { get; set; } = 300.0f;

	public TimeUntil slideCoolDown = 0;

	public override bool ShouldBecomeActive()
	{
		if ( TimeSinceActiveChanged < NextSlideCooldown ) return false;

		if ( CanSlide && Input.Pressed( "ability" ) )
		{
			slideCoolDown = Player.Stats.UpgradedStats[Stats.PlayerUpgradedStats.SkillOneCoolDown];
			return true;
		}

		return false;
	}
	
	public override void OnActiveUpdate()
	{
		base.OnActiveUpdate();
		isSliding = true;
	}

	bool CanSlide => slideCoolDown <= 0;
	public bool isSliding = false;
	private TimeUntil slideEndTime;
	private int slideCharges = 1; // Number of slide charges
	private float rechargeRate = 5f; // Time in seconds to recharge one slide charge
	private float timeSinceLastSlide = 0f; // Time since last slide
	private void RechargeSlideCharges()
	{
		if ( slideCharges < Player.Stats.UpgradedStats[Stats.PlayerUpgradedStats.SkillOneUses] )
		{
			timeSinceLastSlide += Time.Delta;

			if ( slideCoolDown <= 0 )
			{
	
				slideCharges++;
				timeSinceLastSlide = 0f;

				if ( slideCharges < Player.Stats.UpgradedStats[Stats.PlayerUpgradedStats.SkillOneUses] )
				{
					slideCoolDown = Player.Stats.UpgradedStats[Stats.PlayerUpgradedStats.SkillOneCoolDown];
				}

				//Log.Info( "Slide charge replenished. Current charges: " + slideCharges );
			}
		}
		else
		{
			//Log.Info( "Slide charges full." );
		}
	}

	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();

		RechargeSlideCharges();
	}

	public override bool ShouldBecomeInactive()
	{
		if ( IsActive ) 
		{
			return TimeSinceActiveChanged > MinimumSlideLength;
		}

		return base.ShouldBecomeInactive();
	}

	public override IEnumerable<string> GetTags()
	{
		yield return "slide";
	}

	public override void BuildWishInput( ref Vector3 wish ) => wish.y *= WishDirectionScale;
	public override float? GetSpeed() => SlideSpeed;
	public override float? GetEyeHeight() => EyeHeight;
	public override float? GetGroundFriction() => SlideFriction;
	public override float? GetAcceleration() => 2;
}
