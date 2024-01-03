using Editor.Widgets;
using RogueFPS;
using Sandbox;
using Sandbox.UI;
using System;
namespace Editor;

/// <summary>
/// An editor tool for placing models on a grid.
/// </summary>
[EditorTool]
[Title( "Grid Map Tool" )]
[Icon( "🏗️" )]
[Group( "Grid Map Editor" )]
[Order( 0 )]
public class GridMapTool : EditorTool
{
	public GameObject SelectedObject { get; set; }

	public string CopyString { get; set; }

	public GridMapEditorResource resource { get; set; }

	public enum ModelPrefabSelection
	{
		Model,
		Prefab
	}
	public ModelPrefabSelection CurrentSelection { get; set; } = ModelPrefabSelection.Model;


	public ListView modellistView { get; set; } = new();
	public ListView prefablistView { get; set; } = new();

	public List<PrefabFile> prefabList { get; set; } = new();
	public List<ModelList> modelList { get; set; } = new();

	public struct ModelList
	{
		public string name;
		public string path;
		public Pixmap icon;
	}

	public PrefabFile SelectedPrefab { get; set; }
	public string SelectedModel { get; set; }

	public Rotation rotation = Rotation.From( 90, 0, 0 );

	public float floors = 0.0f;

	public enum RotationSnap
	{
		None = 0,
		Five = 5,
		Fifteen = 15,
		FortyFive = 45,
		Ninety = 90
	}
	public RotationSnap CurrentRotationSnap { get; set; } = RotationSnap.Ninety;

	public enum PaintMode
	{
		Place = 0,
		Remove = 1,
		Move = 2,
		Copy = 3,
	}

	public PaintMode CurrentPaintMode { get; set; } = PaintMode.Place;

	public override void OnEnabled()
	{
		var so = EditorTypeLibrary.GetSerializedObject( this );
		AllowGameObjectSelection = false;

		{
			var window = new TileWindow( SceneOverlay, Scene );
			window.MaximumWidth = 300;

			var rotshort = new Shortcut( SceneOverlay, "Rotate", "p", () => Log.Info( "Buttes" ) );
			var row = Layout.Column();
			var cs = new ControlSheet();
			var paintmode = row.Add( new SegmentedControl() );
			paintmode.AddOption( "Place", "brush" );
			paintmode.AddOption( "Remove", "delete" );
			paintmode.AddOption( "Move", "open_with" );
			paintmode.AddOption( "Copy", "content_copy" );


			paintmode.OnSelectedChanged += ( s ) =>
			{
				CurrentPaintMode = Enum.Parse<PaintMode>( s );
			};

			cs.AddRow( so.GetProperty( "resource" ) );

			var modelprefab = row.Add( new SegmentedControl() );
			modelprefab.AddOption( "Model", "brush" );
			modelprefab.AddOption( "Prefab", "delete" );
			modelprefab.OnSelectedChanged += ( s ) =>
			{
				CurrentSelection = Enum.Parse<ModelPrefabSelection>( s, true );
				UpdateListViewVisibility();
			};

			prefablistView.ItemSize = 64;
			prefablistView.ItemAlign = Sandbox.UI.Align.SpaceBetween;
			prefablistView.OnPaintOverride += () => PaintListBackground( modellistView );
			prefablistView.ItemPaint = PaintBrushPrefab;
			prefablistView.ItemSelected = ( item ) =>
			{
				if ( item is PrefabFile data )
				{
					SelectedPrefab = data;
				}
			};

			modellistView.ItemSize = new Vector2( 64, 64 );
			modellistView.ItemAlign = Sandbox.UI.Align.SpaceBetween;
			modellistView.ItemSelected = ( item ) =>
			{
				if ( item is ModelList data )
				{
					SelectedModel = data.path;
				}
			};

			modellistView.OnPaintOverride += () => PaintListBackground( modellistView );
			modellistView.ItemPaint = PaintBrushItem;
			row.Add( cs );
			row.Add( modellistView );
			row.Add( prefablistView );

			window.Layout = row;
			window.MinimumSize = new Vector2( 300, 500 );

			AddOverlay( window, TextFlag.RightBottom, 10 );
		}
		UpdateListViewVisibility();
		{
			var window = new WidgetWindow( SceneOverlay, "Grid Map Controls" );
			window.MaximumWidth = 300;
			var cs = new ControlSheet();
			cs.AddRow( so.GetProperty( "CurrentRotationSnap" ) );
			var Rotbuttons = Layout.Row();
			Rotbuttons.Spacing = 8;
			Rotbuttons.Add( new Label( "Rotation:" ) );
			Rotbuttons.AddStretchCell( 1 );
			Rotbuttons.Add( new Button( "", "arrow_back" ) { Clicked = DoRotation( true ) } );
			Rotbuttons.Add( new Button( "", "arrow_forward" ) { Clicked = DoRotation( false ) } );

			var Floorbuttons = Layout.Row();
			Floorbuttons.Spacing = 8;
			Floorbuttons.Add( new Label( "Floors:" ) );
			Floorbuttons.AddStretchCell( 1 );
			Floorbuttons.Add( new Button( "", "arrow_upward" ) { Clicked = DoFloors( Gizmo.Settings.GridSpacing ) } );
			Floorbuttons.Add( new Button( "", "arrow_downward" ) { Clicked = DoFloors( -Gizmo.Settings.GridSpacing ) } );

			cs.AddLayout( Rotbuttons );
			cs.AddLayout( Floorbuttons );

			//cs.Add( new Button( "Clear", "clear" ) { Clicked = ClearAll } );
			window.Layout = cs;


			AddOverlay( window, TextFlag.RightTop, 10 );
		}

	}

	private void UpdateListViewVisibility()
	{
		modellistView.Visible = CurrentSelection == ModelPrefabSelection.Model;
		prefablistView.Visible = CurrentSelection == ModelPrefabSelection.Prefab;
	}


	public override void OnDisabled()
	{
		base.OnDisabled();
	}

	public override void OnUpdate()
	{

		if ( resource != null )
		{
			foreach ( var model in resource.TileModels )
			{
				if ( !modelList.Any( x => x.name == model.ResourceName ) )
				{
					modelList.Add( new ModelList()
					{
						name = model.ResourceName,
						path = model.ResourcePath,
						icon = AssetSystem.FindByPath( model.ResourcePath ).GetAssetThumb()
					} );

					Log.Info( model.Name );
				}
			}

			foreach ( var prefab in resource.TilePrefab )
			{
				if ( !prefabList.Any( x => x.ResourceName == prefab.ResourceName ) )
				{
					prefabList.Add( prefab );

					Log.Info( prefab.ResourceName );
				}
			}

			modellistView.SetItems( modelList.Cast<object>() );

			prefablistView.SetItems( prefabList.Cast<object>() );
		}

		modellistView.ItemsSelected = SetSelection;

		// Do gizmos and stuff
		var cursorRay = Gizmo.CurrentRay;

		var tr = Scene.Trace.Ray( cursorRay, 5000 )
						.UseRenderMeshes( true )
						.UsePhysicsWorld( false )
						.WithoutTags( "sprinkled" )
						.Run();

		if ( resource is null )
			return;

		if ( Gizmo.IsCtrlPressed && Gizmo.WasLeftMousePressed )
		{
			rotation *= Rotation.FromYaw( (float)CurrentRotationSnap );
		}

		if ( tr.Hit )
		{
			if ( CurrentSelection == ModelPrefabSelection.Model )
			{
				PaintModelGizmos( tr );
				if ( previewPrefab is not null )
				{
					previewPrefab.Destroy();
					previewPrefab = null;
				}
			}
			else if ( CurrentSelection == ModelPrefabSelection.Prefab )
			{
				PaintPrefabGizmos( tr );
			}
		}

		if ( !Gizmo.IsCtrlPressed && CurrentSelection == ModelPrefabSelection.Model )
		{
			if ( Gizmo.WasLeftMousePressed && CurrentPaintMode == PaintMode.Place )
			{
				var go = new GameObject( true, "GridTile" );
				go.Components.Create<ModelRenderer>().Model = Model.Load( SelectedModel );
				go.Components.Create<ModelCollider>().Model = Model.Load( SelectedModel );
				go.Transform.Position = tr.HitPosition.SnapToGrid( Gizmo.Settings.GridSpacing ).WithZ( floors );
				go.Transform.Rotation = Rotation.LookAt( tr.Normal ) * rotation;
				go.Tags.Add( "sprinkled" );
			}
			else if ( Gizmo.WasLeftMousePressed && CurrentPaintMode == PaintMode.Remove )
			{
				var tr2 = Scene.Trace.Ray( cursorRay, 5000 )
								.UseRenderMeshes( true )
								.UsePhysicsWorld( false )
								.WithTag( "sprinkled" )
								.Run();
				if ( tr2.Hit )
				{
					tr2.GameObject.Destroy();
				}
			}

			if ( Gizmo.WasLeftMousePressed && CurrentPaintMode == PaintMode.Move )
			{
				var tr2 = Scene.Trace.Ray( cursorRay, 5000 )
								.UseRenderMeshes( true )
								.UsePhysicsWorld( false )
								.WithTag( "sprinkled" )
								.Run();
				if ( tr2.Hit )
				{
					SelectedObject = tr2.GameObject;
				}
			}
			else if ( Gizmo.WasLeftMouseReleased && CurrentPaintMode == PaintMode.Move && SelectedObject is not null )
			{
				SelectedObject = null;
			}
			if ( Gizmo.IsLeftMouseDown && CurrentPaintMode == PaintMode.Move && SelectedObject is not null )
			{
				SelectedObject.Transform.Position = tr.HitPosition.SnapToGrid( Gizmo.Settings.GridSpacing ).WithZ( floors );
				SelectedObject.Transform.Rotation = Rotation.LookAt( tr.Normal ) * rotation;
			}

			if ( Gizmo.WasLeftMouseReleased && CurrentPaintMode == PaintMode.Copy && CopyString == null )
			{
				var tr2 = Scene.Trace.Ray( cursorRay, 5000 )
								.UseRenderMeshes( true )
								.UsePhysicsWorld( false )
								.WithTag( "sprinkled" )
								.Run();
				if ( tr2.Hit )
				{
					CopyString = tr2.GameObject.Components.Get<ModelRenderer>().Model.Name;
				}
			}

			if ( Gizmo.WasLeftMousePressed && CurrentPaintMode == PaintMode.Copy && CopyString != null )
			{
				var go = new GameObject( true, "GridTile" );
				go.Components.Create<ModelRenderer>().Model = Model.Load( CopyString );
				go.Components.Create<ModelCollider>().Model = Model.Load( CopyString );
				go.Transform.Position = tr.HitPosition.SnapToGrid( Gizmo.Settings.GridSpacing ).WithZ( floors );
				go.Transform.Rotation = Rotation.LookAt( tr.Normal ) * rotation;
				go.Tags.Add( "sprinkled" );
			}

			if ( Application.IsKeyDown( KeyCode.Escape ) && CopyString != null )
			{
				CopyString = null;
			}
		}
		else if ( !Gizmo.IsCtrlPressed && CurrentSelection == ModelPrefabSelection.Prefab)
		{
			if ( Gizmo.WasLeftMousePressed && CurrentPaintMode == PaintMode.Place && previewPrefab != null )
			{
				var go = SceneUtility.Instantiate( SceneUtility.GetPrefabScene( SelectedPrefab ) );
				go.Transform.Position = tr.HitPosition.SnapToGrid( Gizmo.Settings.GridSpacing ).WithZ( floors );
				go.Transform.Rotation = Rotation.LookAt( tr.Normal ) * rotation;
				go.Tags.Add( "sprinkled" );
				Log.Info( go.Name );
			}
			else if ( Gizmo.WasLeftMousePressed && CurrentPaintMode == PaintMode.Remove )
			{
				var tr2 = Scene.Trace.Ray( cursorRay, 5000 )
								.UseRenderMeshes( true )
								.UsePhysicsWorld( false )
								.WithTag( "sprinkled" )
								.Run();
				if ( tr2.Hit )
				{
					tr2.GameObject.Destroy();
				}
			}
		}


		if ( !Gizmo.IsLeftMouseDown )
			return;
	}

	GameObject previewPrefab;

	public void PaintPrefabGizmos( SceneTraceResult tr )
	{
		var cursorRay = Gizmo.CurrentRay;

		if ( CurrentPaintMode == PaintMode.Place && SelectedPrefab is not null )
		{
			if ( previewPrefab is null )
			{
				previewPrefab = SceneUtility.Instantiate( SceneUtility.GetPrefabScene( SelectedPrefab ) );
				previewPrefab.Tags.Add( "sprinkled" );
			}

			if ( previewPrefab != SceneUtility.GetPrefabScene( SelectedPrefab ) )
			{
				previewPrefab.Destroy();

				previewPrefab = SceneUtility.Instantiate( SceneUtility.GetPrefabScene( SelectedPrefab ) );
				previewPrefab.Tags.Add( "sprinkled" );
			}

			if ( previewPrefab is not null )
			{
				using ( Gizmo.Scope( "preview" ) )
				{
					Gizmo.Transform = new Transform( tr.HitPosition.SnapToGrid( Gizmo.Settings.GridSpacing ).WithZ( floors ), Rotation.LookAt( tr.Normal ) * rotation );
					previewPrefab.Transform.Position = tr.HitPosition.SnapToGrid( Gizmo.Settings.GridSpacing ).WithZ( floors );
					previewPrefab.Transform.Rotation = Rotation.LookAt( tr.Normal ) * rotation;
				}
			}
		}
		else if( CurrentPaintMode != PaintMode.Place)
		{
			if ( previewPrefab != null )
			{
				previewPrefab.Destroy();
				previewPrefab = null;
			}
		}

		if ( CurrentPaintMode == PaintMode.Remove )
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

	public void PaintModelGizmos( SceneTraceResult tr )
	{
		// Do gizmos and stuff
		var cursorRay = Gizmo.CurrentRay;

		if ( CurrentPaintMode == PaintMode.Place && SelectedModel is not null )
		{
			using ( Gizmo.Scope( "preview" ) )
			{

				Gizmo.Transform = new Transform( tr.HitPosition.SnapToGrid( Gizmo.Settings.GridSpacing ), Rotation.LookAt( tr.Normal ) * rotation );
				Gizmo.Draw.Color = Gizmo.Colors.Green.WithAlpha( 0.35f );
				Gizmo.Draw.SolidBox( Model.Load( SelectedModel ).Bounds );
				Gizmo.Draw.LineBBox( Model.Load( SelectedModel ).Bounds );
				Gizmo.Transform = new Transform( tr.HitPosition.SnapToGrid( Gizmo.Settings.GridSpacing ).WithZ( floors ), Rotation.LookAt( tr.Normal ) * rotation );

				//var trans = new Transform( tr.HitPosition, Rotation.LookAt( tr.Normal ) );
				Gizmo.Draw.Color = Color.White;
				Gizmo.Draw.Model( Model.Load( SelectedModel ) );
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
				if ( tr.Hit && CopyString != null )
				{

					Gizmo.Transform = new Transform( tr.HitPosition.SnapToGrid( Gizmo.Settings.GridSpacing ).WithZ( floors ), Rotation.LookAt( tr.Normal ) * rotation );
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

	private static bool PaintListBackground( Widget widget )
	{
		Paint.ClearPen();
		Paint.SetBrush( Theme.ControlBackground.WithAlpha( 0.5f ) );
		Paint.DrawRect( widget.LocalRect );

		return false;
	}

	private void PaintBrushItem( VirtualWidget widget )
	{
		var brush = (ModelList)widget.Object;

		Paint.Antialiasing = true;
		Paint.TextAntialiasing = true;


		if ( brush.path == SelectedModel )
		{
			Paint.ClearPen();
			Paint.SetBrush( Theme.Green.WithAlpha( 0.10f ) );
			Paint.SetPen( Theme.Green.WithAlpha( 0.50f ) );
			Paint.DrawRect( widget.Rect.Grow( 0 ), 3 );
		}

		Paint.ClearPen();
		Paint.SetBrush( Theme.White.WithAlpha( 0.01f ) );
		Paint.SetPen( Theme.White.WithAlpha( 0.05f ) );
		Paint.DrawRect( widget.Rect.Shrink( 2 ), 3 );

		Paint.Draw( widget.Rect.Shrink( 1 ), brush.icon );

		var rect = widget.Rect;

		var textRect = rect.Shrink( 2 );
		textRect.Top = textRect.Top + 20;
		textRect.Top = textRect.Top + 25;

		Paint.ClearPen();
		Paint.SetBrush( Theme.Black.WithAlpha( 0.5f ) );
		Paint.DrawRect( textRect, 0.0f );

		Paint.Antialiasing = true;

		Paint.SetPen( Theme.Blue, 2.0f );
		Paint.ClearBrush();
		Paint.SetFont( "Poppins", 6, 700 );
		Paint.DrawText( textRect, brush.name );
	}


	private void PaintBrushPrefab( VirtualWidget widget )
	{
		var brush = (PrefabFile)widget.Object;

		Paint.Antialiasing = true;
		Paint.TextAntialiasing = true;


		if ( brush == SelectedPrefab )
		{
			Paint.ClearPen();
			Paint.SetBrush( Theme.Green.WithAlpha( 0.10f ) );
			Paint.SetPen( Theme.Green.WithAlpha( 0.50f ) );
			Paint.DrawRect( widget.Rect.Grow( 0 ), 3 );
		}

		Paint.ClearPen();
		Paint.SetBrush( Theme.White.WithAlpha( 0.01f ) );
		Paint.SetPen( Theme.White.WithAlpha( 0.05f ) );
		Paint.DrawRect( widget.Rect.Shrink( 2 ), 3 );

		Paint.Draw( widget.Rect.Shrink( 1 ), AssetSystem.FindByPath( brush.ResourcePath ).GetAssetThumb() );

		var rect = widget.Rect;

		var textRect = rect.Shrink( 2 );
		textRect.Top = textRect.Top + 20;
		textRect.Top = textRect.Top + 25;

		Paint.ClearPen();
		Paint.SetBrush( Theme.Black.WithAlpha( 0.5f ) );
		Paint.DrawRect( textRect, 0.0f );

		Paint.Antialiasing = true;

		Paint.SetPen( Theme.Blue, 2.0f );
		Paint.ClearBrush();
		Paint.SetFont( "Poppins", 6, 700 );
		Paint.DrawText( textRect, brush.ResourceName );
	}


	void SetSelection( object o )
	{
		if ( o is string s )
		{
			SelectedModel = s;

			Log.Info( $"Selected {s}" );
		}
	}

	private Action DoRotation( bool leftright )
	{

		Log.Info( "Rotate" );

		return () =>
		{
			if ( leftright )
			{
				rotation *= Rotation.FromYaw( (float)CurrentRotationSnap );
			}
			else
			{
				rotation *= Rotation.FromYaw( -(float)CurrentRotationSnap );
			}
		};

	}

	private Action DoFloors( float i )
	{
		return () => { floors += i; };
	}


	void ClearAll()
	{
		foreach ( var obj in Scene.GetAllObjects( false ).Where( x => x.Tags.Has( "sprinkled" ) ).ToArray() )
		{
			obj.Destroy();
		}
	}

}

file class TileWindow : WidgetWindow
{
	Scene scene;
	public TileWindow( Widget parent, Scene scene ) : base( parent, "Grid Map Editor" )
	{

		this.scene = scene;
	}

	protected override void OnPaint()
	{
		base.OnPaint();

	}
}
