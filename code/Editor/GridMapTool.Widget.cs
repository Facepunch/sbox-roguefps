using Editor.Widgets;
using RogueFPS;
using Sandbox;
using Sandbox.UI;
using System;
using static Sandbox.HitboxSet;

namespace Editor;

public partial class GridMapTool
{
	void ToolWindow(SerializedObject so)
	{
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

			var collectionrow = Layout.Row();
			/*var search = new LineEdit();
			search.MinimumHeight = Theme.RowHeight;
			search.PlaceholderText = "Search...";
			search.TextEdited += ( s ) =>
			{
				SearchString = s;
			};
			*/

			var search = new LineEdit();
			search.MinimumHeight = Theme.RowHeight;
			search.PlaceholderText = "Search...";
			search.TextEdited += OnSearchTextChanged;

			var collectionLabel = new Label( "Collection" );
			var collectionCombo = collectionDropDown;
			collectionrow.AddSpacingCell( 5 );

			var collectionButton = new Button( "", "add" );
			collectionButton.ButtonType = "clear";
			collectionButton.Clicked = () =>
			{
				var popup = new NewCollectionObjectWindow();
				popup.Show();
				popup.gridmapToolDockWidget = this;
			};
			collectionButton.MaximumWidth = 32;

			collectionrow.Add( search );
			collectionrow.AddSpacingCell( 5 );
			collectionrow.Add( collectionLabel );
			collectionrow.AddSpacingCell( 5 );
			collectionrow.Add( collectionCombo );
			collectionrow.Add( collectionButton );


			var modelprefab = row.Add( new SegmentedControl() );
			modelprefab.AddOption( "Model", "brush" );
			modelprefab.AddOption( "Prefab", "delete" );
			modelprefab.OnSelectedChanged += ( s ) =>
			{
				CurrentSelection = Enum.Parse<ModelPrefabSelection>( s, true );
				UpdateListViewVisibility();
				UpdateListViewItems();
			};

			prefablistView.ItemSize = 64;
			prefablistView.ItemAlign = Sandbox.UI.Align.SpaceBetween;
			prefablistView.OnPaintOverride += () => PaintListBackground( prefablistView );
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
				if ( item is TileList data )
				{
					SelectedModel = data.gameObject;
				}
			};

			modellistView.OnPaintOverride += () => PaintListBackground( modellistView );
			modellistView.ItemPaint = PaintBrushItem;
			row.Add( collectionrow );
			row.Add( cs );
			row.Add( modellistView );
			row.Add( prefablistView );

			window.Layout = row;
			window.MinimumSize = new Vector2( 300, 600 );

			AddOverlay( window, TextFlag.RightBottom, 10 );
		}
	}

	void MainWindow(SerializedObject so)
	{
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
			Floorbuttons.Add( new Label( "Floor:" ) );
			Floorbuttons.AddStretchCell( 1 );
			floorLabel = new Label( floorCount.ToString() );
			Floorbuttons.Add( floorLabel );
			Floorbuttons.AddStretchCell( 1 );
			Floorbuttons.Add( new Button( "", "arrow_upward" ) { Clicked = () => { DoFloors( 128 )(); floorLabel.Text = floorCount.ToString(); } } );
			Floorbuttons.Add( new Button( "", "arrow_downward" ) { Clicked = () => { DoFloors( -128 )(); floorLabel.Text = floorCount.ToString(); } } );

			cs.AddLayout( Floorbuttons );
			cs.AddLayout( Rotbuttons );
			cs.AddLayout( Floorbuttons );

			//cs.Add( new Button( "Clear", "clear" ) { Clicked = ClearAll } );
			window.Layout = cs;


			AddOverlay( window, TextFlag.RightTop, 10 );
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
