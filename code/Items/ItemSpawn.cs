using Sandbox;

public sealed class ItemSpawn : Component
{

	TimeSince timeSinceSpawned;

	protected override void OnValidate()
	{
		base.OnValidate();

		var _collider = Components.Get<Collider>(FindMode.EnabledInSelf);
		_collider.Enabled = false;

		timeSinceSpawned = 0;
	}

	protected override void OnUpdate()
	{
		if(timeSinceSpawned > 2)
		{
			var _collider = Components.Get<Collider>( FindMode.DisabledInSelf );
			_collider.Enabled = true;
		}
	}
}
