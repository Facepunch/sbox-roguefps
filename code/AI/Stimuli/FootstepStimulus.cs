namespace RogueFPS;

/// <summary>
/// We've heard a footstep
/// </summary>
public class FootstepStimulus : Stimulus
{
	public float Volume;

	public FootstepStimulus( Vector3 position, float volume = 1 ) : base( position )
	{
		Volume = volume;
	}

	public static FootstepStimulus From( SceneModel.FootstepEvent e )
	{
		return new FootstepStimulus( e.Transform.Position, e.Volume );
	}

	public override bool ShouldReact( Actor actor )
	{
		if ( actor is Agent agent && agent.NoReactionToSound ) return false;

		float distance = Position.Distance( actor.Transform.Position );
		distance *= Volume;
		float maxRange = 1024f;

		return distance < maxRange;
	}
}
