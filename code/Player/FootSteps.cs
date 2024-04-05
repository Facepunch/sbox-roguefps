public sealed class FootSteps : Component
{
	[Property] SkinnedModelRenderer Source { get; set; }

	protected override void OnEnabled()
	{
		if ( Source is null )
			return;

		Source.OnFootstepEvent += OnEvent;
	}

	protected override void OnDisabled()
	{
		if ( Source is null )
			return;

		Source.OnFootstepEvent -= OnEvent;
	}

	TimeSince timeSinceStep;

	private void OnEvent( SceneModel.FootstepEvent e )
	{
		if ( timeSinceStep < 0.5f )
			return;

		var tr = Scene.Trace
			.Ray( e.Transform.Position + Vector3.Up * 20, e.Transform.Position + Vector3.Up * -20 )
			.WithoutTags("player")
			.Run();

		if ( !tr.Hit )
			return;

		if ( tr.Surface is null )
			return;

		timeSinceStep = 0;

		var sound = tr.Surface.Sounds.FootLeft;
		if ( sound is null ) return;

		var vol = Components.Get<PlayerController>( FindMode.EverythingInSelfAndAncestors ).WishVelocity.Length.LerpInverse( 50, 150, true );

		var handle = Sound.Play( sound, tr.HitPosition + tr.Normal * 5 );
		handle.Volume = vol * e.Volume;

		Scene.BroadcastStimulus( FootstepStimulus.From( e ) );
	}
}
