using Editor.Widgets;
using RogueFPS;
using Sandbox;
using Sandbox.UI;
using System;
using System.Drawing;
using static Sandbox.HitboxSet;

namespace Editor;

public partial class GridMapTool
{
	void CollectionGroupHighLight()
	{
		if ( timeSinceChangedCollection <= 5 )
		{
			var alpha = 1 - (timeSinceChangedCollection * 4);
			using ( Gizmo.Scope( "Collection" ) )
			{
				Gizmo.Draw.Color = Gizmo.Colors.Pitch.WithAlpha( alpha.LerpTo( 1, 0.0f ) );
				Gizmo.Draw.SolidBox( CurrentGameObjectCollection.GetBounds() );

				Gizmo.Draw.Color = Gizmo.Colors.Pitch.WithAlpha( alpha.LerpTo( 1, 0.0f ) );
				Gizmo.Draw.LineBBox( CurrentGameObjectCollection.GetBounds() );
			}
		}
		/*
		Gizmo.Draw.Color = Gizmo.Colors.Pitch.WithAlpha( 0.15f );
		Gizmo.Draw.SolidBox( CurrentGameObjectCollection.GetBounds() );

		Gizmo.Draw.Color = Gizmo.Colors.Pitch;
		Gizmo.Draw.LineBBox( CurrentGameObjectCollection.GetBounds() );
		*/
	}

	public void GroundGizmo( Ray cursorRay )
	{
		projectedPoint = ProjectRayOntoGroundPlane( cursorRay.Position, cursorRay.Forward, floors );
		
		if ( projectedPoint.Hit )
		{
			// Snap the projected point to the grid and adjust for floor height
			var snappedPosition = projectedPoint.EndPosition;

			Vector3 normal = Vector3.Up; // Default for Z-axis
			switch ( Axis )
			{
				case GroundAxis.X:
					normal = Vector3.Forward; // Normal for X-axis
					break;
				case GroundAxis.Y:
					normal = Vector3.Left; // Normal for Y-axis
					break;
					// Z-axis is already set as default
			}

			using ( Gizmo.Scope( "Ground") )
			{
				var axiscolor = Gizmo.Colors.Up;
				switch ( Axis )
				{
					case GroundAxis.X:
						axiscolor = Gizmo.Colors.Left;
						break;
					case GroundAxis.Y:
						axiscolor = Gizmo.Colors.Forward;
						break;
				}

				Gizmo.Draw.Color = axiscolor.WithAlpha(0.25f);
				//Gizmo.Transform = new Transform( snappedPosition );
				//Gizmo.Draw.Plane( 0, normal );

				// Calculate the half size of the grid spacing
				float halfGridSize = Gizmo.Settings.GridSpacing / 2.0f;

				// Create a flat box based on the grid spacing
				Vector3 minCorner = snappedPosition - new Vector3( halfGridSize, halfGridSize, 0 );
				switch ( Axis )
				{
					case GroundAxis.X:
						minCorner = snappedPosition - new Vector3( 0, halfGridSize, halfGridSize );
						break;
					case GroundAxis.Y:
						minCorner = snappedPosition - new Vector3( halfGridSize, 0, halfGridSize );
						break;
				}
				Vector3 maxCorner = snappedPosition + new Vector3( halfGridSize, halfGridSize, 0 );
				switch ( Axis )
				{
					case GroundAxis.X:
						maxCorner = snappedPosition + new Vector3( 0, halfGridSize, halfGridSize );
						break;
					case GroundAxis.Y:
						maxCorner = snappedPosition + new Vector3( halfGridSize, 0, halfGridSize );
						break;
				}
				var bbox = new BBox( minCorner, maxCorner );
				Gizmo.Draw.LineThickness = 6;
				Gizmo.Draw.Color = axiscolor.WithAlpha( 1f );
				Gizmo.Draw.LineBBox( bbox );
				Gizmo.Transform = new Transform( 0, Rotation.FromAxis( Vector3.Up, 45 ) );
			}
		}
	}

	public void PaintModelGizmos( SceneTraceResult tr )
	{
		// Do gizmos and stuff
		var cursorRay = Gizmo.CurrentRay;

		var boxtr = Scene.Trace.Ray( cursorRay, 5000 )
			.UsePhysicsWorld( true )
			.WithoutTags( "sprinkled" )
			.Run();

		if ( !boxtr.Hit )
		{
			Vector3 rayOrigin = cursorRay.Position;
			Vector3 rayDirection = cursorRay.Forward;

			boxtr = ProjectRayOntoGroundPlane( rayOrigin, rayDirection, 0 );
		}

		if ( CurrentPaintMode == PaintMode.Place && SelectedModel is not null )
		{
			using ( Gizmo.Scope( "preview" ) )
			{
				projectedPoint = ProjectRayOntoGroundPlane( cursorRay.Position, cursorRay.Forward, floors );
				
				if ( projectedPoint.Hit )
				{
					var snappedPosition = projectedPoint.EndPosition;

					Gizmo.Transform = new Transform( snappedPosition, Rotation.FromPitch( -90 ) * rotation );
					Gizmo.Draw.Color = Color.White;
					Gizmo.Draw.Model( Model.Load( SelectedModel ) );
					Gizmo.Draw.Color = Gizmo.Colors.Green.WithAlpha( 0.35f );
					Gizmo.Draw.LineBBox( Model.Load( SelectedModel ).Bounds );
				}
			}
		}

		else if ( CurrentPaintMode == PaintMode.Remove )
		{
			using ( Gizmo.Scope( "preview" ) )
			{
				var tr2 = Scene.Trace.Ray( cursorRay, 5000 )
						.UseRenderMeshes( true )
						.UsePhysicsWorld( false )
						.WithTag( "sprinkled" )
						.Run();

				if ( tr2.Hit )
				{

					Gizmo.Draw.Color = Color.Red.WithAlpha( 0.5f );
					Gizmo.Draw.LineBBox( tr2.GameObject.GetBounds() );
					Gizmo.Draw.SolidBox( tr2.GameObject.GetBounds() );
				}
			}
		}
		else if ( CurrentPaintMode == PaintMode.Move )
		{
			using ( Gizmo.Scope( "preview" ) )
			{
				var tr2 = Scene.Trace.Ray( cursorRay, 5000 )
						.UseRenderMeshes( true )
						.UsePhysicsWorld( false )
						.WithTag( "sprinkled" )
						.Run();
				
				if ( tr2.Hit && SelectedObject is null )
				{
					Gizmo.Draw.Color = Theme.Blue.WithAlpha( 0.5f );
					Gizmo.Draw.LineBBox( tr2.GameObject.GetBounds() );
					Gizmo.Draw.SolidBox( tr2.GameObject.GetBounds() );
				}
				else if ( SelectedObject is not null )
				{
					Gizmo.Draw.Color = Theme.Blue.WithAlpha( 0.5f );
					Gizmo.Draw.LineBBox( SelectedObject.GetBounds() );
					Gizmo.Draw.SolidBox( SelectedObject.GetBounds() );
				}
			}
		}
		else if ( CurrentPaintMode == PaintMode.Copy )
		{
			using ( Gizmo.Scope( "preview" ) )
			{
				var tr2 = Scene.Trace.Ray( cursorRay, 5000 )
						.UseRenderMeshes( true )
						.UsePhysicsWorld( false )
						.WithTag( "sprinkled" )
						.Run();

				if ( CopyString != null )
				{
					Gizmo.Transform = new Transform( boxtr.EndPosition.SnapToGrid( Gizmo.Settings.GridSpacing ), Rotation.FromPitch( -90 ) * rotation );
					Gizmo.Draw.Color = Color.Yellow.WithAlpha( 0.15f );
					Gizmo.Draw.LineBBox( Model.Load( CopyString ).Bounds );
					Gizmo.Draw.SolidBox( Model.Load( CopyString ).Bounds );
					Gizmo.Draw.Color = Color.White;
					Gizmo.Draw.Model( Model.Load( CopyString ) );
				}
				else if ( tr2.Hit && CopyString == null )
				{
					Gizmo.Draw.Color = Theme.Yellow.WithAlpha( 0.5f );
					Gizmo.Draw.LineBBox( tr2.GameObject.GetBounds() );
					Gizmo.Draw.SolidBox( tr2.GameObject.GetBounds() );
				}
			}
		}
	}
}
