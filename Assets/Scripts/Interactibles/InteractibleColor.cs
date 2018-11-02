using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class InteractibleColor : Interactible
{
	[SerializeField] Color interacted = Color.red;

	Color normal;
	MeshRenderer meshRenderer;

	void Start()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		normal = meshRenderer.material.color;
	}

	void ChangeColor(Color color)
	{
		meshRenderer.material.color = color;
	}

	protected override void InteractionStarted()
	{
		ChangeColor(interacted);
	}

	protected override void InteractionActive()
	{

	}

	protected override void InteractionEnded()
	{
		ChangeColor(normal);
	}
}
