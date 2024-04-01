using Sandbox;

public sealed class InteractorUse : Component
{
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

				var pickupui = new ItemPickUp( interactor.Cost );

				itemUI.Panel.AddChild( pickupui );
			}
		}
		else
		{
			DestroyUI();
		}
	}


	void DestroyUI()
	{
		var parent = GameObject.Parent;
		var ui = parent.Components.Get<ScreenPanel>( FindMode.EnabledInSelfAndDescendants );
		var itemUI = ui.Components.Get<ItemsUI>( FindMode.EnabledInSelfAndDescendants );

		itemUI.Panel.Children.FirstOrDefault( x => x is ItemPickUp )?.Delete();
	}
}
