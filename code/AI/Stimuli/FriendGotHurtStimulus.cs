namespace RogueFPS;

/// <summary>
/// Been shot
/// </summary>
public class FriendGotHurtStimulus : Stimulus
{
	Vector3 Pos;

	public FriendGotHurtStimulus( Vector3 position ) : base( position )
	{
		Pos = position;
	}

	public override bool ShouldReact( Actor actor )
	{
		return actor.Transform.Position.Distance( Pos ) < 256f;
	}
}
