using Sandbox;
namespace RogueFPS;

[Title( "Rogue FPS Player Upgrade Info" )]
[Category( "Player Upgrade" )]
[Icon( "upgrade", "red", "white" )]
[EditorHandle( "materials/gizmo/charactercontroller.png" )]
public sealed class RogueFPSPlayerUpgradeInfo : Component
{
	[Property]
	public string ItemName { get; set; } = "Item Name";

	[Property]
	public string ItemDescription { get; set; } = "Item Description";

	protected override void OnUpdate()
	{
		
	}
}
