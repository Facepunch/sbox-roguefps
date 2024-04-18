[GameResource( "RogueFPS Character", "rfpschr", "Character Ref", Icon = "engineering",IconBgColor = "#fcba03" )]
public class CharacterResource : GameResource
{
	public string CharacterName { get; set; }
	public string CharacterDescription { get; set; }
	[Property, ImageAssetPath]public string Icon { get; set; }
	public Model Model { get; set; }
	public GameObject CharacterPrefab { get; set; }
	public bool IsPlayable { get; set; }
}
