using UnityEngine;

public class PhysicalButton : MonoBehaviour
{
	[SerializeField] Interactible connectedInteractible;
	[SerializeField] Vector3 unpressedLocalPosition;
	[SerializeField] Vector3 pressedLocalPosition;
	[SerializeField] float pressSpeed;

	Vector3 unpressedPosition;
	Vector3 pressedPosition;

	bool beingPressed, pressedDown;

	void Start()
	{
		unpressedPosition = transform.TransformPoint(unpressedLocalPosition);
		pressedPosition = transform.TransformPoint(pressedLocalPosition);
	}

	void Update()
	{
		if (beingPressed) transform.position = Vector3.MoveTowards(transform.position, pressedPosition, pressSpeed);
		else transform.position = Vector3.MoveTowards(transform.position, unpressedPosition, pressSpeed);

		if (!pressedDown && (transform.position - pressedPosition).magnitude < 0.01)
		{
			pressedDown = true;
			connectedInteractible.InteractStart();
		}

		if (pressedDown && (transform.position - unpressedPosition).magnitude < 0.01)
		{
			pressedDown = false;
			connectedInteractible.InteractEnd();
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		beingPressed = true;
	}

	void OnCollisionExit(Collision collision)
	{
		beingPressed = false;
	}
}
