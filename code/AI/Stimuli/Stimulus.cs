namespace RogueFPS;

/// <summary>
/// Generic stimulus - you should derive from this if you want a new stimulus type
/// </summary>
public class Stimulus
{
	public TimeSince LifeTime;
	public Vector3 Position;
	public bool HasExpired => LifeTime > 5f;

	protected Stimulus( Vector3 position )
	{
		LifeTime = 0;
		Position = position;
	}

	public virtual bool ShouldReact( Actor actor )
	{
		return true;
	}
}
