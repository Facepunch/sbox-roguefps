using Sandbox;

namespace RogueFPS;

[GameResource( "Grid Tiles", "gridtile", "A collection holding tiles.", Icon = "brush", IconBgColor = "#ffbb00" )]
public class GridMapEditorResource : GameResource
{
	public List<Model> TileModels { get; set; } = new();

	public List<PrefabFile> TilePrefab { get; set; } = new();

}
