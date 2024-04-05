namespace RogueFPS;

/// <summary>
/// We've seen the player
/// </summary>
public class EnemySpottedStimulus : Stimulus
{
	public Actor Enemy;

	public EnemySpottedStimulus( Actor enemy ) : base( enemy.Transform.Position )
	{
		Enemy = enemy;
	}

	public override bool ShouldReact( Actor actor )
	{
		float distance = Position.Distance( actor.Transform.Position );
		float maxRange = 1000f;

		if ( !Enemy.IsAlive )
		{
			return false;
		}

		return distance < maxRange;
	}
}
