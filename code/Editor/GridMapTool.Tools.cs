using Editor.Widgets;
using RogueFPS;
using Sandbox;
using Sandbox.UI;
using System;
using static Sandbox.HitboxSet;

namespace Editor;

public partial class GridMapTool
{
	private SceneTraceResult projectedPoint;
	public int FloorHeight = 128;

	public SceneTraceResult CursorRay (Ray cursorRay )
	{
		var tr = Scene.Trace.Ray( cursorRay, 5000 )
		.UseRenderMeshes( true )
		.UsePhysicsWorld( false )
		.WithTag( "sprinkled" )
		.Run();

		return tr;
	}

	public void HandlePlacement( SceneTraceResult tr, Ray cursorRay )
	{
		if ( SelectedModel is null ) return;
		
		projectedPoint = ProjectRayOntoGroundPlane( cursorRay.Position, cursorRay.Forward, floors );
		
		if ( projectedPoint.Hit )
		{
			// Snap the projected point to the grid and adjust for floor height
			var snappedPosition = projectedPoint.EndPosition.SnapToGrid( Gizmo.Settings.GridSpacing );

			var go = new GameObject( true, "GridTile" );
			go.Parent = CurrentGameObjectCollection;
			go.Components.Create<ModelRenderer>().Model = Model.Load( SelectedModel );
			go.Components.Create<ModelCollider>().Model = Model.Load( SelectedModel );
			go.Transform.Position = snappedPosition;
			go.Transform.Rotation = Rotation.FromPitch( -90 ) * rotation;
			go.Tags.Add( "sprinkled" );

			Log.Info( $"Placed {SelectedModel} at {snappedPosition}" );
		}
	}

	public void HandleRemove( Ray cursorRay )
	{		
		if ( CursorRay(cursorRay).Hit )
		{
			Log.Info( $"Remove {CursorRay( cursorRay ).GameObject.Name}" );
			CursorRay( cursorRay ).GameObject.Destroy();
		}
	}
	public void HandleGetMove( Ray cursorRay )
	{
		if ( CursorRay( cursorRay ).Hit )
		{
			Log.Info( $"Start Moving {CursorRay( cursorRay ).GameObject.Name}" );
			SelectedObject = CursorRay( cursorRay ).GameObject;
			lastRot = SelectedObject.Transform.Rotation;
			beenRotated = false;
		}
	}
	Rotation lastRot;
	bool beenRotated;
	public void HandleMove( Ray cursorRay )
	{
		projectedPoint = ProjectRayOntoGroundPlane( cursorRay.Position, cursorRay.Forward, floors );

		if ( projectedPoint.Hit )
		{
			// Snap the projected point to the grid and adjust for floor height
			var snappedPosition = projectedPoint.EndPosition.SnapToGrid( Gizmo.Settings.GridSpacing ).WithZ( floors );

			SelectedObject.Transform.Position = snappedPosition;

			// Only update rotation if 'shouldRotate' is true
			if ( beenRotated )
			{
				SelectedObject.Transform.Rotation = Rotation.FromPitch( -90 ) * rotation;
			}
			else
			{
				// Keep the last rotation
				SelectedObject.Transform.Rotation = lastRot;
			}
		}
	}

	public void HandleCopy( Ray cursorRay )
	{
		if ( CursorRay( cursorRay ).Hit )
		{
			CopyString = CursorRay( cursorRay ).GameObject.Components.Get<ModelRenderer>().Model.Name;
			Log.Info( $"Copy {CopyString}" );
			beenRotated = false;
		}
	}
	
	bool _prevlessFloor = false;
	bool _prevmoreFloor = false;
	public void FloorHeightShortCut()
	{
		if ( Application.IsKeyDown( KeyCode.Q ) && !_prevlessFloor )
		{
			DoFloors( -FloorHeight )();
			floorLabel.Text = floorCount.ToString();
		}
		else if ( Application.IsKeyDown( KeyCode.E ) && !_prevmoreFloor )
		{
			DoFloors( FloorHeight )();
			floorLabel.Text = floorCount.ToString();
		}

		_prevlessFloor = Application.IsKeyDown( KeyCode.Q );
		_prevmoreFloor = Application.IsKeyDown( KeyCode.E );
	}
	
	//Nasty
	bool _prevlessRotationZ = false;
	bool _prevmoreRotationZ = false;
	bool _prevlessRotationX = false;
	bool _prevmoreRotationX = false;
	bool _prevlessRotationY = false;
	bool _prevmoreRotationY = false;

	public void HandleRotation()
	{

		if ( Application.IsKeyDown( KeyCode.Num1 ) && Gizmo.IsShiftPressed && !_prevlessRotationZ )
		{
			DoRotation( true, GroundAxis.Z )();
			SnapToClosest( GroundAxis.Z );
		}
		else if ( Application.IsKeyDown( KeyCode.Num1 ) && Gizmo.IsAltPressed && !_prevmoreRotationZ )
		{
			DoRotation( false, GroundAxis.Z )();
			SnapToClosest( GroundAxis.Z );
		}

		if ( Application.IsKeyDown( KeyCode.Num2 ) && Gizmo.IsShiftPressed && !_prevlessRotationX )
		{
			DoRotation( true, GroundAxis.X )();
			SnapToClosest( GroundAxis.X );
		}
		else if ( Application.IsKeyDown( KeyCode.Num2 ) && Gizmo.IsAltPressed && !_prevmoreRotationX )
		{
			DoRotation( false, GroundAxis.X )();
			SnapToClosest( GroundAxis.X );
		}

		if ( Application.IsKeyDown( KeyCode.Num3 ) && Gizmo.IsShiftPressed && !_prevlessRotationY )
		{
			DoRotation( true, GroundAxis.Y )();
			SnapToClosest( GroundAxis.Y );
		}
		else if ( Application.IsKeyDown( KeyCode.Num3 ) && Gizmo.IsAltPressed && !_prevmoreRotationY )
		{
			DoRotation( false, GroundAxis.Y )();
			SnapToClosest( GroundAxis.Y );
		}

		_prevlessRotationZ = Application.IsKeyDown( KeyCode.Num1 ) && Gizmo.IsShiftPressed;
		_prevmoreRotationZ = Application.IsKeyDown( KeyCode.Num1 ) && Gizmo.IsAltPressed;
		_prevlessRotationX = Application.IsKeyDown( KeyCode.Num2 ) && Gizmo.IsShiftPressed;
		_prevmoreRotationX = Application.IsKeyDown( KeyCode.Num2 ) && Gizmo.IsAltPressed;
		_prevlessRotationY = Application.IsKeyDown( KeyCode.Num3 ) && Gizmo.IsShiftPressed;
		_prevmoreRotationY = Application.IsKeyDown( KeyCode.Num3 ) && Gizmo.IsAltPressed;
	}

	bool _prevlessRotationSnap = false;
	bool _prevmoreRotationSnap = false;

	public void UpdateRotationSnapWithKeybind()
	{
		if ( Gizmo.IsCtrlPressed && Application.IsKeyDown( KeyCode.BraceLeft ) && !_prevlessRotationSnap )
		{
			CycleRotationSnap( -1 );
		}
		else if ( Gizmo.IsCtrlPressed && Application.IsKeyDown( KeyCode.BraceRight ) && !_prevmoreRotationSnap )
		{
			CycleRotationSnap( 1 );
			Log.Info( $"Rotation Snap: {rotationSnap}" );
		}

		_prevlessRotationSnap = Gizmo.IsCtrlPressed && Application.IsKeyDown( KeyCode.BraceLeft );
		_prevmoreRotationSnap = Gizmo.IsCtrlPressed && Application.IsKeyDown( KeyCode.BraceRight );
	}

	private void CycleRotationSnap( int direction )
	{
		// Get all values of the RotationSnap enum
		var values = Enum.GetValues( typeof( RotationSnap ) ).Cast<RotationSnap>().ToList();

		// Find the current index of the enum
		int currentIndex = values.IndexOf( CurrentRotationSnap );

		// Calculate the new index
		int newIndex = (currentIndex + direction + values.Count) % values.Count;

		// Update the CurrentRotationSnap to the new value
		CurrentRotationSnap = values[newIndex];
	}
}
