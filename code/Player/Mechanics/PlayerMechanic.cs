namespace RogueFPS;

public partial class PlayerMechanic : ActorMechanic
{
	/// <summary>
	/// Gets a reference to the player.
	/// </summary>
	public PlayerController Player => Actor as PlayerController;
	public PlayerController PlayerController => Actor as PlayerController;
}
