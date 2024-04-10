public sealed class InteractorUse : Component
{
	GameObject InteractObject { get; set; }
	private ItemPickUp _pickupPanel { get; set; }
	private TimeSince _timeSinceUse { get; set; }

	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();

		var tr = Scene.Trace.Ray( Scene.Camera.Transform.Position, Scene.Camera.Transform.Position + Scene.Camera.Transform.Rotation.Forward * 100 )
			.Radius( 20 )
			.WithTag( "interactable" )
			.Run();

		if ( tr.Hit )
		{
			var interactor = tr.GameObject.Components.Get<Interactable>();
			if ( interactor != null )
			{
				//Wait a moment before interacting again to avoid interacting with two items in the same frame
				if ( _timeSinceUse < 0.1f ) return;

				if ( interactor.IsOpen ) return;

				if ( Input.Pressed( "use" ) )
				{
					interactor.OnInteract( GameObject );
					_timeSinceUse = 0f;
					ClearInteractor();
					return;
				}

				var itemUI = ItemsUI.Instance;

				if ( _pickupPanel != null ) return;

				var item = interactor.Components.Get<ItemHelper>( FindMode.EnabledInSelfAndChildren );
				if ( item != null )
				{
					if ( item.Equipment != null )
					{
						//Log.Info( "Equipment component found" );
						_pickupPanel = new ItemPickUp( $"Get {item.Equipment.Name}", interactor.Cost, false );
						itemUI.Panel.AddChild( _pickupPanel );
					}
					else
					{
						//Log.Info( "Item component found" );
						_pickupPanel = new ItemPickUp( $"Get {item.Item.Name}", interactor.Cost, false );
						itemUI.Panel.AddChild( _pickupPanel );
					}
				}
				else
				{
					Log.Info( "No item component found" );
					_pickupPanel = new ItemPickUp( interactor.Name, interactor.Cost, interactor.HasPrice );
					itemUI.Panel.AddChild( _pickupPanel );
				}

				InteractObject = tr.GameObject;

				interactor.CreateGlow();
			}
		}
		else
		{
			ClearInteractor();
		}
	}

	public void ClearInteractor()
	{
		InteractObject = null;
		DestroyUI();
	}

	public void DestroyUI()
	{
		_pickupPanel?.Delete();
		_pickupPanel = null;

		var interactor = InteractObject?.Components.Get<Interactable>();
		if ( interactor == null ) return;
		interactor.DestroyGlow();
	}
}
