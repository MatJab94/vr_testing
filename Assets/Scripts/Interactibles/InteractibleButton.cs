using System.Collections;
using UnityEngine;

public class InteractibleButton : Interactible
{
	[SerializeField] Interactible connectedInteractible;
	[SerializeField] Transform button;
	[SerializeField] float speed = 0.1f;
	[SerializeField] float animateTime = 0.2f;

	bool toggle;

	protected override void InteractionStarted()
	{
		if (connectedInteractible != null)
		{
			toggle = !toggle;
			if (toggle) connectedInteractible.InteractStart();
			else connectedInteractible.InteractEnd();
		}
		StartCoroutine(Animate());
	}

	protected override void InteractionActive() { }

	protected override void InteractionEnded() { }

	IEnumerator Animate()
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
