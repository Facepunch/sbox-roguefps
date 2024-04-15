using Sandbox;

public sealed class DoorMove : Component
{
	[Property] public Vector3 MoveDir { get; set; } = Vector3.Up;
	[Property] public float MoveDistance { get; set; } = 128.0f;
	[Property] public float Speed { get; set; } = 2.0f;
	Vector3 StartingPos { get; set; }
	public bool IsOpen { get; set; } = false;
	public bool ShouldOpen { get; set; } = false;
	protected override void DrawGizmos()
	{
		base.DrawGizmos();

		Gizmo.Draw.Line( Vector3.Zero, Vector3.Zero + MoveDir * MoveDistance);

		Gizmo.Transform = new Transform(MoveDistance * MoveDir);
		Gizmo.Draw.Color = Color.White;
		Gizmo.Draw.LineBBox(GameObject.GetBounds());
		Gizmo.Draw.Color = Color.Cyan.WithAlpha( 0.5f );
		Gizmo.Draw.SolidBox( GameObject.GetBounds() );
	}

	protected override void OnStart()
	{
		base.OnStart();

		StartingPos = GameObject.Transform.Position;
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( ShouldOpen && !IsOpen )
		{
			//If the door reaches the distance it should stop moving and marked as open
			if ( (GameObject.Transform.Position - StartingPos).Length >= MoveDistance )
			{
				IsOpen = true;
				ShouldOpen = false;
			}
			else
			{
				GameObject.Transform.Position += MoveDir * Speed;
			}

		}
	}

	public void Open()
	{
		ShouldOpen = true;
	}
}
