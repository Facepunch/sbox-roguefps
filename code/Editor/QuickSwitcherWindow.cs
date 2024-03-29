using Editor;
using Sandbox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace QuickSwitcher;

public class QuickSwitcherWindow : PopupWidget
{
	public string Filter { get; set; }
	private ListView List { get; set; }

	record class Option( OptionType Type, string Name, string ActionText, Pixmap Icon );

	public static QuickSwitcherWindow Instance { get; set; } 

	public QuickSwitcherWindow( Widget parent = null ) : base( parent )
	{
		Instance = this;

		Layout = Layout.Column();
		Size = new Vector2( 500, 400 );

		Layout.Margin = 8;
		Layout.Spacing = 4;

		var input = Layout.Add( new LineEdit(), 1 );
		input.PlaceholderText = "Search...";
		input.FixedHeight = 32;
		input.Focus();

		input.TextChanged += s => SetListItems();

		Bind( nameof( Filter ) ).ReadOnly().From( input, x => x.Text );

		//var toolbar = Layout.Add( new SegmentedControl( this ) );
		//toolbar.AddOption( "All", "list" );
		//toolbar.AddOption( "File", "description" );
		//toolbar.AddOption( "Action", "directions_run" );

		Layout.Add( BuildOptions(), 1 );
	}

	private void SetListItems()
	{
		List<Option> options = new();

		//
		// Assets
		//
		{
			var assets = AssetSystem.All.OrderBy( x => x.Name ).ToList();
			options.AddRange( assets.Select( x => new Option( OptionType.Asset, x.Name, $"{x.AssetType.FriendlyName} Asset", x.AssetType.Icon64 ) ) );
		}

		//
		// Actions
		////
		//{
		//	foreach ( var assetType in AssetType.All )
		//	{
		//		options.Add( new Option( OptionType.Action, $"Create {assetType.FriendlyName}...", "Action", assetType.Icon64 ) );
		//	}
		//}

		if ( !string.IsNullOrEmpty( Filter ) )
			options = options.Where( x => x.Name.Contains( Filter, StringComparison.OrdinalIgnoreCase ) ).ToList();

		options = options.OrderByDescending( x => x.Type ).ToList();

		List.SetItems( options );
	}

	void DoSelect( Option option )
	{
		var asset = AssetSystem.All.FirstOrDefault( x => x.Name == option.Name );

		if ( asset != null )
		{
			asset.OpenInEditor();
		}
	}

	private Widget BuildOptions()
	{
		var canvas = new Widget( null );
		canvas.Layout = Layout.Row();

		List = new ListView( canvas );
		SetListItems();
		List.Margin = 0;
		List.ItemSize = new Vector2( 0, 24 );
		List.ItemPaint = PaintListMode;
		List.ItemSpacing = 0;

		List.ItemSelected = ( items ) =>
		{
			if ( items is Option option )
				DoSelect( option );
		};

		canvas.Layout.Add( List );

		return canvas;
	}

	private void PaintListMode( VirtualWidget item )
	{
		if ( item.Object is not Option option )
			return;

		var itemRect = item.Rect;

		if ( Paint.HasSelected || Paint.HasPressed )
		{
			Paint.SetPen( Paint.HasPressed ? Theme.Green : Theme.Primary, 2, PenStyle.Dash );
			Paint.ClearBrush();
			Paint.DrawRect( itemRect.Shrink( 1 ), 3 );

			Paint.ClearPen();
			Paint.SetBrush( Paint.HasPressed ? Theme.Green.WithAlpha( 0.4f ) : Theme.Primary.WithAlpha( 0.4f ) );
			Paint.DrawRect( itemRect.Shrink( 0 ), 3 );

			Paint.SetPen( Theme.White );
		}
		else if ( Paint.HasMouseOver )
		{
			Paint.ClearPen();
			Paint.SetBrush( Theme.Blue.Darken( 0.7f ).Desaturate( 0.3f ).WithAlpha( 0.5f ) );
			Paint.DrawRect( itemRect );
			Paint.SetPen( Theme.White );
		}
		else
		{
			Paint.ClearPen();
			Paint.SetBrush( option.Type.GetColor().WithAlpha( 0.05f ) );
			Paint.DrawRect( itemRect );
			Paint.SetPen( Theme.White );
		}

		itemRect = itemRect.Shrink( 0, 0, 16, 0 );

		if ( item.Object is DirectoryInfo dirInfo )
		{
			Paint.SetDefaultFont( 8 );
			Paint.DrawText( itemRect.Shrink( 42, 0 ), dirInfo.Name, TextFlag.LeftCenter | TextFlag.SingleLine );

			Paint.SetPen( Theme.Yellow );
			Paint.DrawIcon( itemRect.Shrink( 15, 3 ), "folder", 18, TextFlag.LeftCenter );
			return;
		}

		var name = option.Name;
		Pixmap icon = option.Icon;
		var actionText = option.ActionText;
		var rect = itemRect;

		{
			Paint.SetDefaultFont( 7 );
			Paint.SetPen( Theme.ControlText.WithAlpha( 0.5f ) );
			Paint.DrawText( rect, actionText, TextFlag.RightCenter | TextFlag.SingleLine );
		}

		Paint.SetPen( Theme.ControlText );
		Paint.SetDefaultFont( 8 );
		Paint.DrawText( rect.Shrink( 24, 0 ), name, TextFlag.LeftCenter | TextFlag.SingleLine );

		if ( icon == null )
			return;

		var ir = itemRect.Shrink( 3 );
		ir.Size = 16;
		Paint.Draw( ir, icon );
	}

	public override void Show()
	{
		base.Show();
		// Show the window in the center of the screen
		Position = ScreenGeometry.Contain( Size, TextFlag.Center ).Position;
		Position -= new Vector2( 0, 200 );
	}

	[Menu( "Editor", "Tools/Open Quick Switcher", "code", Shortcut = "Ctrl+K" )]
	public static void ToggleQuickSwitcher()
	{
		Log.Info( "ToggleQuickSwitcher" );
		if ( Instance != null )
			Instance.Destroy();

		Instance = new QuickSwitcherWindow();
		Instance.Show();
	}
}
