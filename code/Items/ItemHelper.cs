using Sandbox;

public sealed class ItemHelper : Component
{
	[Property]
	public ItemDef Item { get; set; }

	protected override void OnPreRender()
	{
		base.OnPreRender();
	
		//Cba to do this properly
		var glow = GameObject.Components.Get<HighlightOutline>(FindMode.EverythingInSelf);
		glow.Color = Item.ItemColor;
	}
}
