using System.Diagnostics;

namespace RogueFPS;

public abstract partial class StateMachine : Component
{
	[Property] public Agent Agent { get; set; }

	private State currentState;
	[Property] public State CurrentState
	{
		get => currentState;
		set
		{
			if ( currentState == value ) return;

			var previousState = currentState;
			currentState = value;

			OnStateChanged( previousState, currentState );
		}
	}
	public IEnumerable<State> States => Components.GetAll<State>( FindMode.EnabledInSelfAndDescendants );

	public TimeSince TimeInState;

	public abstract void Tick();

	/// <summary>
	/// Set state where type is T
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public void SetState<T>() where T : State
	{
		var state = Components.Get<T>( FindMode.EnabledInSelfAndDescendants );
		if ( state is not null )
		{
			CurrentState = state;
		}
	}

	public void OnStateChanged( State before, State after )
	{
		//Log.Info( $"FSM state changed from {before} to {after}" );

		before?.OnStateExit( after );
		after?.OnStateEnter( before );

		if ( before != after )
		{
			TimeInState = 0;
		}
	}

	public virtual void OnEvent( string eventName, params object[] obj )
	{
		foreach ( var state in States )
		{
			state.OnEvent( eventName, obj );
		}
	}

	internal void InternalTick()
	{
		Tick();
	}

	public void UpdateStateMachine()
	{
		DrawDebug();
		InternalTick();

		foreach ( var state in States.OrderByDescending( x => x.Priority ) )
		{
			state.Agent = Agent;

			var sw = Stopwatch.StartNew();
			bool shouldEnterState = state.ShouldEnterState( this );

			sw.Stop();

			if ( shouldEnterState )
			{
				CurrentState = state;
				break;
			}

		}

		if ( CurrentState is not null )
		{
			if ( CurrentState.CanTick() )
				CurrentState.InternalTick();
		}
	}

	[ConVar( "op_dev_ai_debug" )]
	public static bool DebugEnabled { get; set; } = false;

	private void DrawDebug()
	{
		if ( !DebugEnabled )
			return;

		var distanceToCamera = Scene.Camera.Transform.Position.Distance( Transform.Position );

		if ( distanceToCamera > 300f )
			return;

		var eyePos = Vector3.Up * 64f;
		var lineHeight = 16f;
		var currentLine = 0;

		void DebugText( object obj )
		{
			var transform = GameObject.Transform.World;
			var position = transform.Position + eyePos;

			var screenPos = Scene.Camera.PointToScreenNormal( position );
			var offset = Vector2.Up * lineHeight * currentLine;

			var pos = screenPos * Screen.Size;
			Gizmo.Draw.ScreenText( $"{obj}", pos - offset, "Consolas" );

			currentLine++;
		}

		DebugText( $"Velocity: {Agent.WishVelocity}" );
		DebugText( $"Mechanics: {string.Join( ", ", Agent.Mechanics.Where( x => x.IsActive ) )}" );
		DebugText( $"State: {CurrentState}" );
		DebugText( $"Name: {GameObject.Parent.Name}" );
	}
}
