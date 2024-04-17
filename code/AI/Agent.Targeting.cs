
namespace RogueFPS;

// Should probably all be in HostileNPC
public partial class Agent
{
	/// <summary>
	/// Does this agent have line of sight to a specified GameObject?
	/// </summary>
	private bool HasLineOfSight( GameObject gameObject )
	{
		// TODO(alex): line of sight cone here

		if ( gameObject == null )
			return false;

		var trace = Scene.Trace
			.Ray( CameraObject.Transform.Position, gameObject.Transform.Position )
			.IgnoreGameObjectHierarchy( GameObject )
			.WithoutTags( "pickup", "agent" )
			.Run();

		if ( !trace.Hit )
			return false;

		return trace.GameObject == gameObject || gameObject.IsDescendant( trace.GameObject );
	}

	/// <summary>
	/// Find ALL players in the scene
	/// </summary>
	public IEnumerable<PlayerController> GetAllPlayers()
	{
		return Scene.GetAllObjects( true )
			.Where( x => x.Components.Get<PlayerController>( FindMode.EverythingInSelfAndAncestors ) != null )
			.Select( x => x.Components.Get<PlayerController>( FindMode.EverythingInSelfAndAncestors ) );
	}

	private IEnumerable<Actor> CachedLOS { get; set; }
	private TimeSince TimeSinceLOSAcquired = 1f;
	private float LOSFrequency => 1f;

	private IEnumerable<Actor> FindActorsInLineOfSight( float range = -1 )
	{
		var forward = CameraObject.Transform.Rotation.Forward;
		var losSphere = new Sphere( CameraObject.Transform.Position + forward * ( range * 2f ), range * 2f );

		//Gizmo.Transform = global::Transform.Zero;
		//Gizmo.Draw.LineSphere( losSphere, 8 );
		//Gizmo.Draw.Line( CameraObject.Transform.Position, CameraObject.Transform.Position + forward * (range / 2f) );

		if ( TimeSinceLOSAcquired < LOSFrequency )
		{
			return CachedLOS;
		}

		TimeSinceLOSAcquired = 0;

		if ( range < 0 )
		{
			range = DefaultDetectionRange;
		}

		CachedLOS = Scene.FindInPhysics( losSphere )
			.Select( x => x.Components.Get<Actor>( FindMode.EverythingInSelfAndAncestors ) )
			.Where( x => HasLineOfSight( x?.GameObject ) )
			.Where( x => x.IsAlive );

		return CachedLOS;
	}

	public IEnumerable<Actor> FindEnemiesInLineOfSight( float range = -1 )
	{
		if ( range < 0 ) range = DefaultDetectionRange;

		return FindActorsInLineOfSight( range ).Where( Hates );
	}

	public Actor FindClosestEnemyInLineOfSight( float range = -1 )
	{
		if ( range < 0 ) range = DefaultDetectionRange;

		return FindActorsInLineOfSight( range ).FirstOrDefault( Hates );
	}

	/// <summary>
	/// Can this agent see a player?
	/// </summary>
	public bool CanSeeAnyPlayer()
	{
		var target = FindActorsInLineOfSight();
		return (target is not null);
	}

	/// <summary>
	/// Make the agent look at someone.
	/// TODO: Make this use proper Actor movement 
	/// </summary>
	public void LookAt( Vector3 position, float smoothing = 5f )
	{
		var lookRot = Rotation.LookAt( position - CameraObject.Transform.Position );
		var lookRotAngles = new Angles( 0, lookRot.Yaw(), 0 );

		Transform.Rotation = Rotation.Lerp( Transform.Rotation, lookRotAngles.ToRotation(), smoothing * Time.Delta );
	}

	/// <summary>
	/// Make the agent look at someone.
	/// TODO: Make this use proper Actor movement 
	/// </summary>
	public void LookAt( Actor target )
	{
		var lookRot = Rotation.LookAt( target.Transform.Position - CameraObject.Transform.Position );
		var lookRotAngles = new Angles( 0, lookRot.Yaw(), 0 );
		Transform.Rotation = lookRotAngles.ToRotation();
	}

	/// <summary>
	/// Gets a path (using the navmesh) between two vectors.
	/// </summary>
	public List<Vector3> GetPath( Vector3 pointA, Vector3 pointB )
	{
		return Scene.NavMesh.GetSimplePath( pointA, pointB );
	}

	/// <inheritdoc cref="GetPath(Vector3, Vector3)"/>
	public List<Vector3> GetPath( Vector3 target )
	{
		return Scene.NavMesh.GetSimplePath( Transform.Position, target );
	}
	public Vector3? GetRandomPoint()
	{
		return Scene.NavMesh.GetRandomPoint();
	}

	[Property] public float LineOfSightRange { get; set; } = -1;
}
