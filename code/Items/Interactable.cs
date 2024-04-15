using Sandbox;

public class Interactable : Component
{
	[Property, ImageAssetPath] public virtual string PingIcon { get; set; } = "ui/ping/ping.png";
	[Property] public virtual string Name { get; set; } = "Interactable";
	[Property] public bool HasPrice { get; set; } = false;
	[Property] public int Cost { get; set; } = 100;
	[Property] public bool IsOpen { get; set; } = false;
	public GameObject Player { get; set; }
	public virtual string PingString { get; set; } = "Interactable";

	protected override void OnStart()
	{
		base.OnStart();

		var helper = GameObject.Components.Get<ItemHelper>(FindMode.InChildren);
		if ( helper != null )
		{
			PingIcon = "ui/ping/item.png";
		}
	}

	public virtual void OnInteract( GameObject player)
	{
		//Log.Info( "Interacted with " + player.Name );

		Player = player;

		var item = Components.Get<ItemHelper>(FindMode.EverythingInChildren);
		if ( item != null )
		{
			var inventory = player.Components.Get<Stats>().Inventory;
			//inventory.AddItem( item.Item );
			item.OnPickedUp( player );
			//Log.Info( "Added item to inventory" );
		}
		else
		{
			//Log.Info( "No item component found" );
		}
	}

	public void CreateGlow()
	{
		var glow = GameObject.Components.Get<HighlightOutline>(FindMode.EverythingInSelf);
		if ( glow != null )
		{
			glow.Enabled = true;
		}
	}
	public void DestroyGlow()
	{
		//Log.Info( "Destroying glow" );

		var glow = GameObject.Components.Get<HighlightOutline>();
		if ( glow != null )
		{
			glow.Enabled = false;
		}
	}
}
