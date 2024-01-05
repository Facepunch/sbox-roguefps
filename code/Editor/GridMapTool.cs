
using RogueFPS;
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
public partial class GridMapTool : EditorTool
{
	public GameObject SelectedObject { get; set; }

	public string CopyString { get; set; }

	public GridMapEditorResource resource { get; set; }
	//public PrefabFile resource { get; set; }

	public enum ModelPrefabSelection
	{
		Model,
		Prefab
	}
	public ModelPrefabSelection CurrentSelection { get; set; } = ModelPrefabSelection.Model;

	public string SearchString { get; set; } = "";

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

	/*
	public List<TileList> tileList { get; set; } = new();

	public struct TileList
	{
		public GameObject gameObject;
		public Pixmap icon;
	}
	*/
	public PrefabFile SelectedPrefab { get; set; }
	public string SelectedModel { get; set; }

	public float floors = 0.0f;
	public int floorCount = 0;

	public Rotation rotation = Rotation.From( 90, 0, 0 );
	float rotationSnap = 90.0f;
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

	public GameObject CurrentGameObjectCollection;
	public List<GameObject> GameObjectCollection { get; set; } = new();

	public ComboBox collectionDropDown { get; set; } = new();

	TimeSince timeSinceChangedCollection = 0;

	public override void OnEnabled()
	{
		var so = EditorTypeLibrary.GetSerializedObject( this );
		AllowGameObjectSelection = false;

		foreach ( var objectinscene in Scene.GetAllObjects( true ) )
		{
			if ( objectinscene.Tags.Has( "Collection" ) && (objectinscene.Parent is Scene) )
			{
				collectionDropDown.AddItem( objectinscene.Name );
				GameObjectCollection.Add( objectinscene );
			}
		}

		MainWindow( so );

		UpdateListViewVisibility();
		UpdateListViewItems();

		ToolWindow( so );
	}

	private void OnSearchTextChanged( string searchText )
	{
		SearchString = searchText.ToLower();
		UpdateListViewItems();
	}


	private void UpdateListViewItems()
	{
		if ( CurrentSelection == ModelPrefabSelection.Model )
		{
			/*
			var filteredModelList = Model
				.Where( model => model.gameObject.Name.ToLower().Contains( SearchString ) )
				.ToList();
			*/
			var filteredModelList = modelList
				.Where( model => model.name.ToLower().Contains( SearchString ) )
				.ToList();
			modellistView.SetItems( filteredModelList.Cast<object>() );
			modellistView.Update(); // Refresh ListView
		}
		else if ( CurrentSelection == ModelPrefabSelection.Prefab )
		{
			var filteredPrefabList = prefabList
				.Where( prefab => prefab.ResourceName.ToLower().Contains( SearchString ) )
				.ToList();
			prefablistView.SetItems( filteredPrefabList.Cast<object>() );
			prefablistView.Update(); // Refresh ListView
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

	private Vector3 startSelectionPoint;
	private Vector3 endSelectionPoint;
	private bool isSelecting = false;

	private void FillSelectionWithTiles( Vector3 start, Vector3 end )
	{
		float gridSpacing = Gizmo.Settings.GridSpacing;
		Vector3 lowerCorner = new Vector3( Math.Min( start.x, end.x ), Math.Min( start.y, end.y ), start.z ); // Use start.z as it's the ground level
		Vector3 upperCorner = new Vector3( Math.Max( start.x, end.x ), Math.Max( start.y, end.y ), start.z );

		for ( float x = lowerCorner.x; x <= upperCorner.x; x += gridSpacing )
		{
			for ( float y = lowerCorner.y; y <= upperCorner.y; y += gridSpacing )
			{
				PlaceTileAtPosition( new Vector3( x, y, lowerCorner.z ) );
			}
		}
	}

	private void PlaceTileAtPosition( Vector3 position )
	{
		if ( SelectedModel != null )
		{
			var go = new GameObject( true, "GridTile" );
			go.Components.Create<ModelRenderer>().Model = Model.Load( SelectedModel );
			go.Components.Create<ModelCollider>().Model = Model.Load( SelectedModel );
			go.Transform.Position = position;
			go.Transform.Rotation = Rotation.FromPitch( -90 ) * rotation;
			go.Tags.Add( "sprinkled" );
		}
	}

	//GPT Moment
	private SceneTraceResult ProjectRayOntoGroundPlane( Vector3 rayOrigin, Vector3 rayDirection, float groundHeight )
	{
		Vector3 planeNormal;
		Vector3 planePoint;
		GroundAxis axis = Axis;

		switch ( axis )
		{
			case GroundAxis.X:
				planeNormal = Vector3.Forward; // Normal perpendicular to X-axis
				planePoint = new Vector3( groundHeight, 0, 0 ); // Point on the X-axis plane
				break;
			case GroundAxis.Y:
				planeNormal = Vector3.Left; // Normal perpendicular to Y-axis
				planePoint = new Vector3( 0, groundHeight, 0 ); // Point on the Y-axis plane
				break;
			default: // Z-axis
				planeNormal = Vector3.Up; // Normal perpendicular to Z-axisZ
				planePoint = new Vector3( 0, 0, groundHeight ); // Point on the Z-axis plane
				break;
		}

		float denom = Vector3.Dot( planeNormal, rayDirection );
		if ( Math.Abs( denom ) > 0.0001f ) // Ensure not parallel
		{
			float num = Vector3.Dot( planeNormal, planePoint - rayOrigin );
			float distance = num / denom;

			if ( distance >= 0 )
			{
				Vector3 hitPoint = rayOrigin + rayDirection * distance;
				return new SceneTraceResult { Hit = true, EndPosition = hitPoint };
			}
		}

		return new SceneTraceResult { Hit = false };
	}

	List<GameObject> SelectedGroupObjects { get; set; } = new List<GameObject>();

	private void SelectObjectsInBox( BBox selectionBox )
	{
		foreach ( var obj in Scene.GetAllObjects( false ) )
		{
			if ( selectionBox.Contains( obj.Transform.Position ) && obj.Tags.Has( "sprinkled" ) )
			{
				SelectedGroupObjects.Add( obj );
			}
		}
	}

	public override void OnUpdate()
	{
		
		UpdateWidgetValues();

		if ( GameObjectCollection is not null && resource is not null )
		{
			CurrentGameObjectCollection = GameObjectCollection.FirstOrDefault( x => x.Name == collectionDropDown.CurrentText );

			collectionDropDown.ItemChanged += () =>
			{
				timeSinceChangedCollection = 0;
			};
		}

		//&& resource.ResourceName.Contains( "_tileset" )

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
			UpdateListViewItems();
			//modellistView.SetItems( modelList.Cast<object>() );

			//prefablistView.SetItems( prefabList.Cast<object>() );
		}

		modellistView.ItemsSelected = SetSelection;

		// Do gizmos and stuff
		var cursorRay = Gizmo.CurrentRay;

		var tr = Scene.Trace.Ray( cursorRay, 5000 )
						.UseRenderMeshes( true )
						.UsePhysicsWorld( false )
						.WithoutTags( "sprinkled" )
						.Run();

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
		
		GroundGizmo( cursorRay );

		HandleRotation();

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

		if ( Gizmo.IsShiftPressed && SelectedObject is null )
		{
			projectedPoint = ProjectRayOntoGroundPlane( cursorRay.Position, cursorRay.Forward, floors );
			if ( Gizmo.WasLeftMousePressed )
			{
				var snappedPosition = projectedPoint.EndPosition.SnapToGrid( Gizmo.Settings.GridSpacing );
				
				startSelectionPoint = snappedPosition;
				
				isSelecting = true;
			}
			else if ( isSelecting )
			{
				var snappedPosition = projectedPoint.EndPosition.SnapToGrid( Gizmo.Settings.GridSpacing );

				endSelectionPoint = snappedPosition;
				
				switch ( Axis )
				{
					case GroundAxis.X:
						endSelectionPoint.x += FloorHeight;
						break;
					case GroundAxis.Y:
						endSelectionPoint.y += FloorHeight;
						break;
					default:
						endSelectionPoint.z += FloorHeight;
						break;
				}

				if ( Gizmo.WasLeftMouseReleased )
				{
					var bbox = new BBox( startSelectionPoint, endSelectionPoint );
					SelectObjectsInBox( bbox );
					isSelecting = false;
				}
			}

			if ( Application.IsKeyDown( KeyCode.F ) )
			{
				FillSelectionWithTiles( startSelectionPoint, endSelectionPoint );
				isSelecting = false;
			}

			if ( Application.IsKeyDown( KeyCode.R ) )
			{
				foreach ( var obj in SelectedGroupObjects )
				{
					obj.Destroy();
				}
			}

			if ( Application.IsKeyDown( KeyCode.Z ) )
			{
				Axis = GroundAxis.Z;
				currentaxisLabel.Text = Axis.ToString();
			}
			else if ( Application.IsKeyDown( KeyCode.C ) )
			{
				Axis = GroundAxis.X;
				currentaxisLabel.Text = Axis.ToString();
			}
			else if ( Application.IsKeyDown( KeyCode.X ) )
			{
				Axis = GroundAxis.Y;
				currentaxisLabel.Text = Axis.ToString();
			}

			FloorHeightShortCut();

			if ( SelectedGroupObjects is not null )
			{
				using ( Gizmo.Scope( "Selection" ) )
				{
					foreach ( var obj in SelectedGroupObjects )
					{
						Gizmo.Draw.Color = Color.Red;
						Gizmo.Draw.LineBBox( obj.GetBounds() );
					}
				}
			}
			using ( Gizmo.Scope( "selection_box" ) )
			{
				var rect = new BBox( startSelectionPoint, endSelectionPoint );
				Gizmo.Draw.Color = Color.Blue.WithAlpha( 0.25f );
				Gizmo.Draw.SolidBox( rect );
				Gizmo.Draw.Color = Color.Blue;
				Gizmo.Draw.LineBBox( rect );
			}

			return;
		}
		else if ( isSelecting )
		{
			isSelecting = false;
		}
		else
		{
			if ( SelectedGroupObjects != null )
			{
				SelectedGroupObjects.Clear();
			}
			startSelectionPoint = Vector3.Zero;
			endSelectionPoint = Vector3.Zero;
		}

		if ( !Gizmo.IsCtrlPressed && CurrentSelection == ModelPrefabSelection.Model )
		{
			if ( Gizmo.WasLeftMousePressed && CurrentPaintMode == PaintMode.Place )
			{
				HandlePlacement( tr, cursorRay );
			}
			else if ( Gizmo.WasLeftMousePressed && CurrentPaintMode == PaintMode.Remove )
			{
				HandleRemove( cursorRay );
			}

			if ( Gizmo.WasLeftMousePressed && CurrentPaintMode == PaintMode.Move )
			{
				HandleGetMove( cursorRay );
			}
			else if ( Gizmo.WasLeftMouseReleased && CurrentPaintMode == PaintMode.Move && SelectedObject is not null )
			{
				SelectedObject = null;
			}
			
			if ( Gizmo.IsLeftMouseDown && CurrentPaintMode == PaintMode.Move && SelectedObject is not null )
			{
				HandleMove( cursorRay );
			}

			if ( Gizmo.WasLeftMouseReleased && CurrentPaintMode == PaintMode.Copy && CopyString == null )
			{
				HandleCopy( cursorRay );
			}
			
			if ( Gizmo.WasLeftMousePressed && CurrentPaintMode == PaintMode.Copy && CopyString != null )
			{
				var go = new GameObject( true, "GridTile" );
				go.Parent = CurrentGameObjectCollection;
				go.Components.Create<ModelRenderer>().Model = Model.Load( CopyString );
				go.Components.Create<ModelCollider>().Model = Model.Load( CopyString );
				go.Transform.Position = boxtr.EndPosition.SnapToGrid( Gizmo.Settings.GridSpacing ).WithZ( floors );
				if ( beenRotated )
				{
					go.Transform.Rotation = Rotation.FromPitch( -90 ) * rotation;
				}
				else
				{
					go.Transform.Rotation = lastRot;
				}
				go.Tags.Add( "sprinkled" );
			}

			if ( Application.IsKeyDown( KeyCode.Escape ) )
			{
				CopyString = null;
				SelectedModel = null;
			}
		}
		else if ( !Gizmo.IsCtrlPressed && CurrentSelection == ModelPrefabSelection.Prefab )
		{
			if ( Gizmo.WasLeftMousePressed && CurrentPaintMode == PaintMode.Place && previewPrefab != null )
			{
				var go = SceneUtility.Instantiate( SceneUtility.GetPrefabScene( SelectedPrefab ) );
				go.Transform.Position = boxtr.EndPosition.SnapToGrid( Gizmo.Settings.GridSpacing ).WithZ( floors );
				go.Transform.Rotation = Rotation.FromPitch( -90 ) * rotation;
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
		}

		UpdateRotationSnapWithKeybind();

		if ( CurrentGameObjectCollection is not null )
		{
			CollectionGroupHighLight();
		}

		if ( !Gizmo.IsLeftMouseDown )
			return;
	}

	GameObject previewPrefab;

	public void PaintPrefabGizmos( SceneTraceResult tr )
	{
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
					Gizmo.Transform = new Transform( boxtr.EndPosition.SnapToGrid( Gizmo.Settings.GridSpacing ), Rotation.FromPitch( -90 ) * rotation );
					Gizmo.Draw.Color = Gizmo.Colors.Green.WithAlpha( 0.35f );
					Gizmo.Draw.SolidBox( previewPrefab.GetBounds() );
					Gizmo.Draw.LineBBox( previewPrefab.GetBounds() );

					Gizmo.Transform = new Transform( boxtr.EndPosition.SnapToGrid( Gizmo.Settings.GridSpacing ), Rotation.FromPitch( -90 ) * rotation );
					previewPrefab.Transform.Position = boxtr.EndPosition.SnapToGrid( Gizmo.Settings.GridSpacing ).WithZ( floors );
					previewPrefab.Transform.Rotation = Rotation.FromPitch( -90 ) * rotation;
				}
			}
		}
		else if ( CurrentPaintMode != PaintMode.Place )
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

	void SetSelection( object o )
	{
		if ( o is string s )
		{
			SelectedModel = s;

			Log.Info( $"Selected {s}" );
		}
	}
	private Action DoRotation( bool leftright, GroundAxis axis )
	{
		beenRotated = true;	
		
		float rotationIncrement = leftright ? (float)CurrentRotationSnap : -(float)CurrentRotationSnap;
		return () =>
		{

			switch ( axis )
			{
				case GroundAxis.X:
					rotation *= Rotation.FromAxis( Vector3.Right, rotationIncrement );
					break;
				case GroundAxis.Y:
					rotation *= Rotation.FromAxis( Vector3.Forward, rotationIncrement );
					break;
				case GroundAxis.Z:
					rotation *= Rotation.FromAxis( Vector3.Up, rotationIncrement );
					break;
			}

		};
	}

	void SnapToClosest( GroundAxis axis )
	{
		var a = rotation.Angles();

		switch (axis)
		{
			case GroundAxis.X:
				a.pitch = a.pitch.SnapToGrid( (float)CurrentRotationSnap );
				break;
			case GroundAxis.Y:
				a.yaw = a.yaw.SnapToGrid( (float)CurrentRotationSnap );
				break;
			case GroundAxis.Z:
				a.roll = a.roll.SnapToGrid( (float)CurrentRotationSnap );
				break;
		}
		rotation = Rotation.From( a );
	}


	
private Action DoFloors( int i )
	{
		return () =>
		{
			floors += i;
			if ( i > 0 )
			{
				floorCount++;
			}
			else if ( i < 0 )
			{
				floorCount--;
			}
		};
	}

	public void CreateCollection( string name )
	{
		using var scope = SceneEditorSession.Scope();
		var go = new GameObject( true, name );
		go.Transform.Position = Vector3.Zero;
		go.Tags.Add( "collection" );

		GameObjectCollection.Add( go );

		Log.Info( "Created collection" );
	}

	void ClearAll()
	{
		foreach ( var obj in Scene.GetAllObjects( false ).Where( x => x.Tags.Has( "sprinkled" ) ).ToArray() )
		{
			obj.Destroy();
		}
	}
}
