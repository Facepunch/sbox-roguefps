namespace RogueFPS;

public partial class StateMachine
{
	public abstract partial class State : Component
	{
		[Property, Category( "Performance" )] public float TickFrequency { get; set; }

		[Property, ReadOnly, Category( "Data" )] public TimeUntil NextTick { get; set; }
		[Property, ReadOnly, Category( "Data" )] public Agent Agent { get; set; }

		[Property] public int Priority { get; set; }

		/// <summary>
		/// Should we be entering this state?
		/// </summary>
		[Property, Category( "Actions" )] public Func<bool> ShouldEnterStateAction { get; set; }

		protected float TimeInState => (Agent.StateMachine.CurrentState != this ? 0f : Agent.StateMachine.TimeInState);

		[Property, Category( "Audio" )] public string OnStateEnterSound { get; set; }

		SoundHandle SoundHandle;

		public virtual void OnEvent( string eventName, params object[] obj )
		{
		}

		/// <summary>
		/// Should we be entering this state?
		/// </summary>
		/// <param name="machine"></param>
		/// <returns></returns>
		public virtual bool ShouldEnterState( StateMachine machine )
		{
			if ( ShouldEnterStateAction is not null ) return ShouldEnterStateAction.Invoke();

			return false;
		}

		public virtual void OnStateEnter( State prev )
		{
		}

		public virtual void OnStateExit( State next )
		{
			if ( SoundHandle is not null )
			{
				SoundHandle?.Stop( 0.25f );
			}
		}

		public virtual bool CanTick()
		{
			if ( NextTick ) return true;
			if ( TickFrequency <= 0 ) return true;
			return false;
		}

		[ConVar( "ai_disabled" )] public static bool DisableAI { get; set; } = false;

		internal void InternalTick()
		{
			if ( DisableAI || !Agent.IsAlive ) return;

			NextTick = TickFrequency;
			Tick();
		}

		public virtual void Tick()
		{
		}
	}
}
