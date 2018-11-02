using UnityEngine;

public abstract class Interactible : MonoBehaviour
{
	bool interactStart, interactActive, interactEnd;

	void Update()
	{
		InteractionUpdate();
	}

	public void InteractionUpdate()
	{
		if (interactStart)
		{
			interactStart = false;
			interactActive = true;
			InteractionStarted();
		}

		if (interactActive) InteractionActive();

		if (interactEnd)
		{
			interactEnd = false;
			interactActive = false;
			InteractionEnded();
		}
	}

	protected virtual void InteractionStarted() { }

	protected virtual void InteractionActive() { }

	protected virtual void InteractionEnded() { }

	public void InteractStart()
	{
		interactStart = true;
	}

	public void InteractEnd()
	{
		interactEnd = true;
	}
}
