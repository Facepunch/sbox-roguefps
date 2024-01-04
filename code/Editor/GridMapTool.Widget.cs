using Editor.Widgets;
using RogueFPS;
using Sandbox;
using Sandbox.UI;
using System;
using System.ComponentModel.DataAnnotations;
using static Sandbox.HitboxSet;

namespace Editor;

public partial class GridMapTool
{
	private Label floorLabel;
	private Label currentaxisLabel;

	public GroundAxis Axis { get; set; } = GroundAxis.Z;
	public enum GroundAxis
	{

		X,
		Y,
		Z
	}
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
				if ( item is ModelList data )
				{
					SelectedModel = data.path;
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

			var pop = Layout.Row();
			pop.Add( new Label( "Current Axis: " ) );
			currentaxisLabel = new Label( Axis.ToString() );
			pop.Add( currentaxisLabel );
			pop.AddStretchCell( 1 );
			var popbutton = pop.Add( new Button( "Options", "more_horiz" ) { Clicked = () => { OpenDropdown( window ); } } );
			popbutton.ButtonType = "clear";

			cs.AddLayout( Floorbuttons );
			cs.AddLayout( Rotbuttons );
			cs.AddLayout( pop );

			//cs.Add( new Button( "Clear", "clear" ) { Clicked = ClearAll } );
			window.Layout = cs;
			AddOverlay( window, TextFlag.RightTop, 10 );
		}
	}

	void OpenDropdown( Widget window )
	{
		var popup = new PopupWidget( window );
		popup.IsPopup = true;
		popup.Position = Application.CursorPosition;

		popup.Layout = Layout.Column();
		popup.Layout.Margin = 16;

		var ps = new PropertySheet( popup );

		ps.AddSectionHeader( "Ground Axis" );
		{
			var w = ps.AddRow( "X", new Checkbox( "Key: C" ) );
			w.Bind( "Value" ).From( () => Axis == GroundAxis.X, x => { if ( x ) Axis = GroundAxis.X; currentaxisLabel.Text = Axis.ToString(); } );
		}
		{
			var w = ps.AddRow( "Y", new Checkbox( "Key: X" ) );
			w.Bind( "Value" ).From( () => Axis == GroundAxis.Y, x => { if ( x ) Axis = GroundAxis.Y; currentaxisLabel.Text = Axis.ToString(); } );
		}
		{
			var w = ps.AddRow( "Z", new Checkbox("Key: Z" ) );
			w.Bind( "Value" ).From( () => Axis == GroundAxis.Z, x => { if ( x ) Axis = GroundAxis.Z; currentaxisLabel.Text = Axis.ToString(); } );
		}

		popup.Layout.Add( ps );
		popup.Show();
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

public class NewCollectionObjectWindow : BaseWindow
{

	Widget container;
	public GridMapTool gridmapToolDockWidget;

	public NewCollectionObjectWindow()
	{
		Size = new Vector2( 350, 150 );
		MinimumSize = Size;
		MaximumSize = Size;
		TranslucentBackground = true;
		NoSystemBackground = true;

		WindowTitle = "Add New Collection Object";
		SetWindowIcon( "rocket_launch" );

		Layout = Layout.Column();
		Layout.Margin = 4;
		Layout.Spacing = 4;

		container = new Widget( this );

		var properties = new PropertySheet( this );

		var nameLabel = new Label.Subtitle( "Add New Collection Object" );
		nameLabel.Margin = 16;

		var nameEdit = properties.AddLineEdit( "Collection Object Name", "" );
		var addbutton = new Button.Primary( "Add Collection", "add_circle" );
		addbutton.MaximumWidth = 100;
		addbutton.Clicked = () =>
		{
			gridmapToolDockWidget.CreateCollection( nameEdit.Text );
			/*
			var gameobject = new GameObject(true, nameEdit.Text );
			gameobject.Transform.Position = Vector3.Zero;
			gameobject.Tags.Add( "sprinkled" );
			gameobject.Tags.Add( "collection" );
			*/
			gridmapToolDockWidget.collectionDropDown.AddItem( nameEdit.Text );
			Close();
		};
		Layout.Add( nameLabel );
		Layout.Add( properties );
		Layout.Add( addbutton );
		Layout.Add( container );

		Show();
	}
	public string SetButtonIcon = "settings";

	public Button buttonIcon;
}
