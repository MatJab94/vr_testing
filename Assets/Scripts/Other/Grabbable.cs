using UnityEngine;

public abstract class Grabbable : MonoBehaviour
{
	protected OVRInput.Controller holdingHand;
	protected Rigidbody player;
	protected Transform grabber;
	protected Transform anchor;

	public abstract void Grab(OVRInput.Controller hand, Transform grabberTransform, Rigidbody playerRigidbody, bool longGrabMode);

	public abstract void Release(Transform grabberTransform);
}
