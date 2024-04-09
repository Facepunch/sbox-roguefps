using Sandbox;

public sealed class InteractorUse : Component
{
	GameObject InteractObject { get; set; }

	protected override void OnUpdate()
	{

	}

	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();

		var tr = Scene.Trace.Ray(Scene.Camera.Transform.Position, Scene.Camera.Transform.Position + Scene.Camera.Transform.Rotation.Forward * 100)
			.Radius(20)
			.WithTag("interactable")
			.Run();

		if (tr.Hit)
		{
			var interactor = tr.GameObject.Components.Get<Interactable>();
			if (interactor != null)
			{
				if ( interactor.IsOpen ) return;

				if (Input.Pressed("use"))
				{
					interactor.OnInteract(GameObject);
					DestroyUI();
					return;
				}

				var parent = GameObject.Parent;
				var ui = parent.Components.Get<ScreenPanel>( FindMode.EnabledInSelfAndDescendants );
				var itemUI = ui.Components.Get<ItemsUI>( FindMode.EnabledInSelfAndDescendants );

				if(itemUI.Panel.Children.FirstOrDefault( x => x is ItemPickUp ) != null) return;

				var item = interactor.Components.Get<ItemHelper>(FindMode.EnabledInSelfAndChildren);
				if ( item != null )
				{
					if(item.Equipment != null)
					{
						//Log.Info( "Equipment component found" );
						var pickupui = new ItemPickUp( $"Get {item.Equipment.Name}", interactor.Cost, false );
						itemUI.Panel.AddChild( pickupui );
					}
					else
					{
						//Log.Info( "Item component found" );
						var pickupui = new ItemPickUp( $"Get {item.Item.Name}", interactor.Cost, false );
						itemUI.Panel.AddChild( pickupui );
					}

				}
				else
				{
					Log.Info( "No item component found" );
					var pickupui = new ItemPickUp( interactor.Name, interactor.Cost, interactor.HasPrice );
					itemUI.Panel.AddChild( pickupui );
				}

				InteractObject = tr.GameObject;

				interactor.CreateGlow();
			}
		}
		else
		{
			DestroyUI();
		}
	}

	void CreateGlow()
	{
		if(InteractObject == null) return;

		foreach (var child in InteractObject.Children)
		{
			if (child.Components.Get<HighlightOutline>() != null)
			{
				var outline = child.Components.Get<HighlightOutline>();
				outline.Enabled = true;
			}
		}
	}

	void DestroyGlow()
	{
		if(InteractObject == null) return;

		foreach (var child in InteractObject.Children)
		{
			if (child.Components.Get<HighlightOutline>() != null)
			{
				var outline = child.Components.Get<HighlightOutline>();
				outline.Enabled = false;
			}
		}
	}


	public void DestroyUI()
	{
		var interactor = InteractObject?.Components.Get<Interactable>();
		if (interactor == null) return;
		interactor.DestroyGlow();

		var parent = GameObject.Parent;
		var ui = parent.Components.Get<ScreenPanel>( FindMode.EnabledInSelfAndDescendants );
		var itemUI = ui.Components.Get<ItemsUI>( FindMode.EnabledInSelfAndDescendants );

		itemUI.Panel.Children.FirstOrDefault( x => x is ItemPickUp )?.Delete();

		//InteractObject = null;
	}
}
