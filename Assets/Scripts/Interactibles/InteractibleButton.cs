using System.Collections;
using UnityEngine;

public class InteractibleButton : Interactible
{
	[SerializeField] Interactible[] connectedInteractibles;
	[SerializeField] Transform button;
	[SerializeField] float speed = 0.1f;
	[SerializeField] float animateTime = 0.2f;

	bool toggle;

	protected override void InteractionStarted()
	{
		toggle = !toggle;
		foreach (var interactible in connectedInteractibles)
		{
			if (toggle) interactible.InteractStart();
			else interactible.InteractEnd();
		}
		StartCoroutine(AnimateButtonPush());
	}

	protected override void InteractionActive() { }

	protected override void InteractionEnded() { }

	IEnumerator AnimateButtonPush()
	{
		float time = 0;
		while (time < animateTime)
		{
			button.position -= UpdatedPosition();
			time += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		time = 0;
		while (time < animateTime)
		{
			button.position += UpdatedPosition();
			time += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
	}

	Vector3 UpdatedPosition()
	{
		return button.up * speed * Time.deltaTime;
	}
}
