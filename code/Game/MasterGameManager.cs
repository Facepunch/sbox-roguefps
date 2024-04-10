using Sandbox;

public sealed class MasterGameManager : Component
{
	private static DateTimeOffset startTime = DateTimeOffset.UtcNow;
	static string FileName = $"{Game.ActiveScene.Title}_currentSession.json";
	public SessionProgess Current { get; set; } = new SessionProgess();

	private void Fetch()
	{
		//Current ??= FileSystem.Data.ReadJson<SessionProgess>( FileName, null );
		/*
		if( Current == null )
		{
			//Log.Info( "No session data found, starting new game." );
			//OnNewGame();
		}
		else
		{
			//Log.Info( "Session data found, loading." );
		}
		*/
	}

	protected override void OnStart()
	{
		//if ( IsProxy )
			//return;

		//Fetch();

		//OnStageBegin();
		//if ( Current == null )
		//{
		//	startTime = DateTimeOffset.UtcNow;
		//}

		OnNewGame();
	}

	protected override void OnUpdate()
	{
		Current.CurrentTime = DateTimeOffset.UtcNow - startTime;
		Current.TotalFactor = (float)((Current.PlayerFactor + Current.CurrentTime.TotalMinutes + Current.TimeFactor) * Current.StageFactor);

		Current.EnemyLevel = 1 + (Current.TotalFactor - Current.PlayerFactor) / 0.33f;

		if ( Input.Pressed( "use" ) )
		{
			//Save();
		}
	}

	public void OnStageBegin()
	{
		Current.PlayerFactor = 1.0f + 0.3f * (Scene.GetAllComponents<PlayerController>().Count() - 1);
		Current.TimeFactor = 0.05f * (int)Current.DifficultyLevel * Scene.GetAllComponents<PlayerController>().Count() * 0.2f;
		Current.StageFactor = 1.15f * Current.StagesCompleted;
	}

	public void Save()
	{
		// If we didn't load data yet (somehow)
		if ( Current == null )
		{
			Fetch();
		}

		FileSystem.Data.WriteJson( FileName, Current );
	}

	public void OnNewGame()
	{
		Current = new SessionProgess();
		startTime = DateTimeOffset.UtcNow;
		OnStageBegin();
	}
}

public class SessionProgess
{
	public TimeSpan CurrentTime { get; set; }
	public Difficulty.DifficultyLevel DifficultyLevel { get; set; } = Difficulty.DifficultyLevel.Normal;
	public float PlayerFactor { get; set; } = 1.0f;
	public float TimeFactor { get; set; } = 1.0f;
	public float StageFactor { get; set; } = 1.0f;
	public int StagesCompleted { get; set; } = 1;
	public float TotalFactor { get; set; } = 1.0f;
	public float EnemyLevel { get; set; } = 1;
}
