using UnityEngine;

public class Climbable : Grabbable
{
	ClimbingPhysics climbingPhysics;

	public override void Grab(OVRInput.Controller hand, Transform grabberTransform, Rigidbody playerRigidbody, bool longGrabMode)
	{
		if (longGrabMode) return;

		player = playerRigidbody;
		player.isKinematic = false;

		climbingPhysics = player.gameObject.GetComponent<ClimbingPhysics>();
		climbingPhysics.isClimbing = true;

		grabber = grabberTransform;
		anchor = new GameObject().transform;
		anchor.parent = transform;
		anchor.position = grabber.position;
		anchor.rotation = grabber.rotation;
	}

	public override void Release(Transform grabberTransform)
	{
		if (grabber == grabberTransform)
		{
			climbingPhysics.isClimbing = false;
			grabber = null;
			Destroy(anchor.gameObject);
			anchor = null;
		}
	}

	void FixedUpdate()
	{
		if (grabber != null)
		{
			Vector3 positionDelta = anchor.position - grabber.position;
			player.velocity = positionDelta / Time.fixedDeltaTime;
		}
	}
}
