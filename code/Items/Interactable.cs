using Sandbox;

public class Interactable : Component
{

	[Property] public string Name { get; set; } = "Interactable";
	[Property] public bool HasPrice { get; set; } = false;
	[Property] public int Cost { get; set; } = 100;
	[Property] public bool IsOpen { get; set; } = false;

	public GameObject Player { get; set; }

	protected override void OnUpdate()
	{

	}

	public virtual void OnInteract( GameObject player)
	{
		Log.Info( "Interacted with " + player.Name );
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
		Log.Info( "Destroying glow" );

		var glow = GameObject.Components.Get<HighlightOutline>();
		if ( glow != null )
		{
			glow.Enabled = false;
		}
	}
}
