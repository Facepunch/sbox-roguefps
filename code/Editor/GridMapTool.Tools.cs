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
		projectedPoint = ProjectRayOntoGroundPlane( cursorRay.Position, cursorRay.Forward, floors );
		
		if ( projectedPoint.Hit )
		{
			// Snap the projected point to the grid and adjust for floor height
			var snappedPosition = projectedPoint.EndPosition.SnapToGrid( Gizmo.Settings.GridSpacing ).WithZ( floors );

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
		}
	}

	public void HandleMove( Ray cursorRay )
	{
		projectedPoint = ProjectRayOntoGroundPlane( cursorRay.Position, cursorRay.Forward, floors );

		if ( projectedPoint.Hit )
		{
			// Snap the projected point to the grid and adjust for floor height
			var snappedPosition = projectedPoint.EndPosition.SnapToGrid( Gizmo.Settings.GridSpacing ).WithZ( floors );

			SelectedObject.Transform.Position = snappedPosition;
			SelectedObject.Transform.Rotation = Rotation.FromPitch( -90 ) * rotation;
		}
	}

	public void HandleCopy( Ray cursorRay )
	{
		if ( CursorRay( cursorRay ).Hit )
		{
			CopyString = CursorRay( cursorRay ).GameObject.Components.Get<ModelRenderer>().Model.Name;
			Log.Info( $"Copy {CopyString}" );
		}
	}
}
