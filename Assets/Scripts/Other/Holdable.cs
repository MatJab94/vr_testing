using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Holdable : Grabbable
{
	[SerializeField] bool snapPosition;

	[SerializeField] bool ignoreRotation;
	[SerializeField] bool ignoreVelocityX;
	[SerializeField] bool ignoreVelocityY;
	[SerializeField] bool ignoreVelocityZ;

	[SerializeField] bool limitPosition;
	[SerializeField] Vector3 minRange;
	[SerializeField] Vector3 maxRange;

	Rigidbody rb;

	public bool IsGrabbed { get { return grabber != null; } }
	public OVRInput.Controller HoldingHand { get { return holdingHand; } }

	Vector3 localPos { get { return transform.localPosition; } }

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.maxAngularVelocity = 100;
	}

	public override void Grab(OVRInput.Controller hand, Transform grabberTransform, Rigidbody playerRigidbody, bool longGrabMode)
	{
		holdingHand = hand;

		grabber = grabberTransform;
		anchor = new GameObject().transform;
		anchor.parent = grabber.transform;

		anchor.position = snapPosition || longGrabMode ? grabber.position : transform.position;
		anchor.rotation = snapPosition || longGrabMode ? grabber.rotation : transform.rotation;
	}

	public override void Release(Transform grabberTransform)
	{
		if (grabber == grabberTransform)
		{
			holdingHand = OVRInput.Controller.None;
			grabber = null;
			Destroy(anchor.gameObject);
			anchor = null;
		}
	}

	void FixedUpdate()
	{
		if (IsGrabbed) PhysicsUpdate();
		else if (limitPosition)
		{
			if (rb.velocity.sqrMagnitude != 0)
				rb.velocity = GetLimitedVelocity(rb.velocity * Time.fixedDeltaTime);

			transform.localPosition = new Vector3(
					Mathf.Clamp(localPos.x, minRange.x, maxRange.x),
					Mathf.Clamp(localPos.y, minRange.y, maxRange.y),
					Mathf.Clamp(localPos.z, minRange.z, maxRange.z));
		}
	}

	void PhysicsUpdate()
	{
		if (!ignoreRotation)
		{
			Quaternion rotationDelta = anchor.rotation * Quaternion.Inverse(rb.transform.rotation);
			float angle; Vector3 axis;
			rotationDelta.ToAngleAxis(out angle, out axis);
			if (angle > 180) angle -= 360;
			if (angle != 0) rb.angularVelocity = angle * axis;
		}

		Vector3 positionDelta = anchor.position - rb.transform.position;
		if (ignoreVelocityX) positionDelta -= UnwantedDelta(transform.right, positionDelta);
		if (ignoreVelocityY) positionDelta -= UnwantedDelta(transform.up, positionDelta);
		if (ignoreVelocityZ) positionDelta -= UnwantedDelta(transform.forward, positionDelta);

		if (limitPosition) rb.velocity = GetLimitedVelocity(positionDelta);
		else rb.velocity = positionDelta / Time.fixedDeltaTime;
	}

	Vector3 GetLimitedVelocity(Vector3 delta)
	{
		float dotX = Vector3.Dot(delta, transform.right);
		float dotY = Vector3.Dot(delta, transform.up);
		float dotZ = Vector3.Dot(delta, transform.forward);

		if (localPos.x > maxRange.x && dotX > 0 || localPos.x < minRange.x && dotX < 0)
			delta -= UnwantedDelta(transform.right, delta);

		if (localPos.y > maxRange.y && dotY > 0 || localPos.y < minRange.y && dotY < 0)
			delta -= UnwantedDelta(transform.up, delta);

		if (localPos.z > maxRange.z && dotZ > 0 || localPos.z < minRange.z && dotZ < 0)
			delta -= UnwantedDelta(transform.forward, delta);

		return delta / Time.fixedDeltaTime;
	}

	Vector3 UnwantedDelta(Vector3 axis, Vector3 delta)
	{
		return axis * Vector3.Dot(delta, axis);
	}
}
