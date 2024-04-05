namespace RogueFPS;

/// <summary>
/// Been shot
/// </summary>
public class HurtStimulus : Stimulus
{
	public HurtStimulus( Vector3 position ) : base( position )
	{
	}

	public override bool ShouldReact( Actor actor )
	{
		return true;
	}
}
