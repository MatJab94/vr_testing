using UnityEngine;

[RequireComponent(typeof(Light))]
public class InteractibleLight : Interactible
{
	Light lightComponent;

	void Start()
	{
		lightComponent = GetComponent<Light>();
	}

	protected override void InteractionStarted()
	{
		lightComponent.enabled = true;
	}

	protected override void InteractionActive()
	{

	}

	protected override void InteractionEnded()
	{
		lightComponent.enabled = false;
	}
}
