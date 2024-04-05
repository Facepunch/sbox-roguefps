namespace RogueFPS;

public class StimulusSystem : GameObjectSystem
{
	public static StimulusSystem Instance { get; private set; }

	public StimulusSystem( Scene scene ) : base( scene )
	{	
		Instance = this;
	}

	public static void Broadcast( Scene scene, Stimulus stimulusInfo )
	{
		var actors = scene.GetAllComponents<Actor.IReceptor>();

		foreach ( var actor in actors )
		{
			actor.OnStimulusReceived( stimulusInfo );
		}
	}
}

public static class SceneExtensions
{
	public static void BroadcastStimulus( this Scene scene, Stimulus stimulusInfo )
	{
		StimulusSystem.Broadcast( scene, stimulusInfo );
	}
}
