using Sandbox;
using Sandbox.Services;
using System.Collections.Generic;

namespace RogueFPS;
public sealed class RogueFPSTestGun : Component
{
	[Property] public GameObject GunObject1 { get; set; }
	[Property] public GameObject GunObject2 { get; set; }
	[Property] public GameObject template { get; set; }
	[Property] public GameObject explosionRadius { get; set; }

	private bool lastGunFired = false;

	private bool isSpinning = false;
	private float spinAmount = 0f;
	private float spinSpeed = 360f; // Degrees per second
	private float spinDuration = 1f; // Duration of spin in seconds
	private float bobPhase = 0f; // Phase of the bob sine wave
	private Vector3 initialPosition; // To store the initial position of the gun
	private GameObject gunToAnimate;

	// Bobbing parameters
	private const float bobAmount = 0.05f; // The amplitude of the bob
	private const float bobSpeed = 10f; // Speed of the bob

	[Property] public GameObject Muzzle1 { get; set; }
	[Property] public GameObject Muzzle2 { get; set; }

	[Property] public GameObject Bullet { get; set; }

	public GameObject muzzleToUse;

	private bool canRJ = true;

	//private NeonPickUp HeldObject;

	private Vector3 GunTilt = new Vector3( 0, 0, 0 );
	
	private float lastFireTime = 0f; // Timestamp of the last fire

	public PlayerStats Stats;

	////
	private bool isBurstFiring = false;
	private int burstCount = 10; // Number of shots in a burst
	public  float lastBurstTime = 0f; // Timestamp of the last burst
	private Queue<float> burstQueue = new Queue<float>(); // Queue to manage burst shots
	public TimeUntil burstCoolDown = 0;
	////

	protected override void OnStart()
	{
		base.OnStart();
		Stats = GameObject.Components.Get<PlayerStats>();
		
		spinDuration = Stats.UpgradedStats[PlayerStats.PlayerUpgradedStats.AttackSpeed];
		spinSpeed = Stats.UpgradedStats[PlayerStats.PlayerUpgradedStats.AttackSpeed] * 360;
		
		//HeldObject = GameObject.GetComponent<NeonPickUp>();

		burstCoolDown = 0;
	}

	protected override void OnUpdate()
	{
		var characterController = GameObject.Components.Get<RogueFPSCharacterController>();
		var pc = GameObject.Components.Get<RogueFPSPlayerController>();
		/*
		if ( HeldObject != null && HeldObject.PickedUpObject != null )
		{
			Log.Info( "Holding object" );
			GunObject.Enabled = false;
			return;
		}
		else
		{
			GunObject.Enabled = true;
		}
		
		// Reset the canShoot flag when the player is grounded
		if ( characterController != null && characterController.IsOnGround )
		{
			canRJ = true;
		}
		*/

		spinDuration = 1f / Stats.UpgradedStats[PlayerStats.PlayerUpgradedStats.AttackSpeed]; // AttackSpeed acts as "shots per second"
		spinSpeed = 360f / spinDuration; // Ensure the spin completes within the duration of one shot

		// If the attack button is pressed and enough time has elapsed since the last shot
		if ( Input.Down( "attack1" ) && (Time.Now - lastFireTime) >= spinDuration )
		{
			// Choose the gun based on lastGunFired flag
			gunToAnimate = lastGunFired ? GunObject2 : GunObject1;
			muzzleToUse = lastGunFired ? Muzzle2 : Muzzle1;

			// Start spinning the gun
			isSpinning = true;
			spinAmount = 0f; // Reset spin amount
			lastFireTime = Time.Now; // Update the last fire time

			Shoot( muzzleToUse );

			// Toggle the lastGunFired for the next shot
			lastGunFired = !lastGunFired;
		}

		// Adjustments for burst firing
		float burstSpinSpeed = spinSpeed * 2;  // Double the spin speed for burst fire
		float burstSpinDuration = spinDuration / 2;  // Half the duration for double speed

		if ( Input.Down( "attack2" ) && !isBurstFiring && 0 >= burstCoolDown)
		{
			isBurstFiring = true;

			burstCoolDown = Stats.UpgradedStats[PlayerStats.PlayerUpgradedStats.SecondaryAttackCoolDown];
			Log.Info( "Burst cooldown: " + burstCoolDown );
			// Populate the burst queue with timestamps for each shot
			for ( int i = 0; i < burstCount; i++ )
			{
				burstQueue.Enqueue( Time.Now + i * (spinDuration / 2) );
			}
		}
		// Handle the burst firing logic
		if ( isBurstFiring && burstQueue.Count > 0 )
		{

			if ( Time.Now >= burstQueue.Peek() )
			{
				burstQueue.Dequeue();

				// Choose the gun based on lastGunFired flag
				gunToAnimate = lastGunFired ? GunObject2 : GunObject1;
				muzzleToUse = lastGunFired ? Muzzle2 : Muzzle1;

				// Start spinning the gun
				isSpinning = true;
				spinAmount = 0f;
				lastFireTime = Time.Now;
				
				spinAmount += burstSpinSpeed * Time.Delta;

				Shoot( muzzleToUse );

				lastGunFired = !lastGunFired;

				if ( burstQueue.Count == 0 )
				{
					isBurstFiring = false;
				}
			}
		}

		// Spinning logic
		if ( isSpinning )
		{
			// Calculate how much the gun should spin this frame
			float spinThisFrame = spinSpeed * Time.Delta;

			// Increment the total spin amount
			spinAmount += spinThisFrame;

			gunToAnimate.Transform.Rotation *= Rotation.FromAxis( Vector3.Right, spinThisFrame );
			
			// If the gun has completed its spin, stop spinning and reset rotation
			if ( spinAmount >= 360f )
			{
				isSpinning = false;
				gunToAnimate.Transform.LocalRotation = Rotation.Identity;		
			}
		}

		// Bobbing effect
		if ( !isSpinning && !pc.CantUseInputMove || (isSpinning && spinAmount < spinSpeed * spinDuration) && !pc.CantUseInputMove )
		{
			var cc = GameObject.Components.Get<RogueFPSCharacterController>();

			if ( cc.Velocity.Normal.Length >= 1 && cc.IsOnGround )
			{
				// Apply bobbing effect
				bobPhase += Time.Delta * bobSpeed;
				float bobOffset = MathF.Sin( bobPhase ) * bobAmount;
			//	GunObject1.Transform.LocalPosition += new Vector3( 0, 0, bobOffset ); // Adjust Y or Z based on your coordinate system
			//	GunObject2.Transform.LocalPosition += new Vector3( 0, 0, bobOffset ); // Adjust Y or Z based on your coordinate system
			}
		}
	}

	public void Shoot( GameObject gunObject )
	{
		//DebugOverlay.Line( Muzzle.Transform.Position, Muzzle.Transform.Position + Muzzle.Transform.Rotation.Forward * 1000, Color.Red, 10, false );
		Sound.Play( "rust_smg.shoot", Transform.Position );

		Gizmo.Draw.Line( muzzleToUse.Transform.Position, muzzleToUse.Transform.Position + muzzleToUse.Transform.Rotation.Forward * 1000 );
		Gizmo.Draw.LineThickness = 10;

		var tr = Scene.PhysicsWorld.Trace.Ray( muzzleToUse.Transform.Position, muzzleToUse.Transform.Position + muzzleToUse.Transform.Rotation.Forward * 1000 )
			.Run();

		//SceneUtility.Instantiate( Bullet, muzzleToUse.Transform.Position, Rotation.LookAt( muzzleToUse.Transform.Rotation.Forward * 1000 ) );

		//Gizmo.Draw.Color = Color.Green;
		//Gizmo.Draw.Line( tr.EndPosition, tr.EndPosition + tr.Normal * 2.0f );

		//Gizmo.Draw.Color = Color.White;
		//Gizmo.Draw.Text( $"Normal: {tr.Normal}\nFraction: {tr.Fraction}", new Transform( tr.EndPosition + Vector3.Down * 1 ) );

		//SceneUtility.Instantiate( template, new Transform( tr.EndPosition, Rotation.LookAt( -tr.Normal + Vector3.Up * 8 ) ) );

		if ( tr.Hit )
		{
			// Apply a force to the hit object
			Vector3 forceDirection = muzzleToUse.Transform.Rotation.Forward; // The direction to apply the force
			float forceMagnitude = 100000f; // The magnitude of the force

			if ( tr.Body.IsValid() )
			{

				//// If the hit object has a NeonFlyEnemy component, call its Hit method
				//var enemy = tr.Body.GameObject as GameObject;
				//var enemyComponent = enemy.Components.Get<NeonFlyEnemy>();

				//if ( enemyComponent != null )
				//{
				//	enemyComponent.Hit( forceDirection * forceMagnitude );

				//}
				//else
				//{
				//	SceneUtility.Instantiate( explosionRadius, new Transform( tr.EndPosition, Rotation.LookAt( -tr.Normal + Vector3.Up * 8 ) ) );
				//	//tr.Body.ApplyImpulseAt( tr.EndPosition, forceDirection * forceMagnitude * 100 );
				//}

				//var healthComponent = enemy.Components.Get<RogueFPSHealthComp>();
				//if ( healthComponent != null )
				//{
				//	healthComponent.Damaged( 10f );
				//}

				//var barrelComponent = enemy.Components.Get<NeonBarrel>();
				//if ( barrelComponent != null )
				//{
				//	barrelComponent.Damaged( 10f );
				//}

				//var buttonComponent = enemy.Components.Get<NeonShootButton>();
				//if ( buttonComponent != null )
				//{
				//	buttonComponent.Open();
				//}
			}

			// Check if the ground was hit to perform a rocket jump
			//if ( IsGround( tr.Normal ) && canRJ )
			//{
			//	var characterController = GameObject.Components.Get<NeonCharacterController>();
			//	if ( characterController != null )
			//	{
			//		characterController.IsOnGround = false;
			//		// This will be your "rocket jump" force
			//		//Vector3 jumpImpulse = Vector3.Up * 500f; // Adjust this value to set the jump strength

			//		// Apply the impulse to the character controller
			//		Log.Info( "Rocket jump!" );
			//		//characterController.Velocity += jumpImpulse;

			//		canRJ = false;
			//	}
			//}
		}

	}

	private bool IsGround( Vector3 normal )
	{
		// This method determines if the surface normal is pointing up enough to be considered "ground"
		// Adjust the dot product threshold to be stricter or more lenient on what is considered ground
		return Vector3.Dot( normal, Vector3.Up ) > 0.7f;
	}
}
