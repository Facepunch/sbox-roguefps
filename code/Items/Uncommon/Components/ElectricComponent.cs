public sealed class ElectricComponent : EffectBaseComponent
{
	public override DamageTypes DamageType { get; set; } = DamageTypes.Electric;
	public int Length { get; set; } = 1;
	public TimeSince timeSinceSpawned { get; set; } = 0;
	public PrefabScene fireParticle { get; set; } = SceneUtility.GetPrefabScene( ResourceLibrary.Get<PrefabFile>( "prefab/particles/items/uncommon/fireeffect.prefab" ) );
	GameObject fire;
	bool spawned = false;

	public float Damage { get; set; } = 0.1f;
	protected override void OnStart()
	{
		base.OnStart();
		timeSinceSpawned = 0;
	}

	public void Hit()
	{
		timeSinceSpawned = 0;
	}

	protected override void OnUpdate()
	{
		var npc = GameObject.Components.Get<Npcbase>();

		if( npc.Health == 0 && fire != null)
		{
			fire.Destroy();
		}

		if ( timeSinceSpawned < Length )
		{
			if ( npc.Health > 0 )
			{
				npc.OnDamage( 0, DamageType );
				var model = Components.Get<ModelRenderer>();
				model.Tint = Color.Blue;
				if(!spawned)
				{
					spawned = true;
					fire = fireParticle.Clone();
					fire.Transform.Position = GameObject.Transform.Position + Vector3.Up * 50;
					fire.Transform.Rotation = Rotation.From( new Angles( 0, 0, 0 ) );
				}
			}
		}
		else
		{
			Destroy();
			fire.Destroy();
			var model = Components.Get<ModelRenderer>();
			model.Tint = Color.White;
		}

		if(fire != null )
		{
			fire.Transform.Position = GameObject.Transform.Position + Vector3.Up * 50;
		}
	}
}
