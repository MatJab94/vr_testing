using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
	[SerializeField] KeyCode toggleButton;
	[SerializeField] OVRInput.RawButton toggleButtonVR;
	[SerializeField] OVRInput.Controller hand;
	[SerializeField] KeyCode grabButton;
	[SerializeField] OVRInput.RawButton grabButtonVR;
	[SerializeField] Rigidbody playerRigidbody;
	[SerializeField] LayerMask ignoreLayers;
	[SerializeField] LineRenderer longGrabLine;

	List<Grabbable> grabbables = new List<Grabbable>();
	Grabbable distanceGrabbable;
	Grabbable grabbed;

	bool longGrabMode;
	RaycastHit hitInfo;

	void Update()
	{
		if (Application.isEditor && Input.GetKeyDown(toggleButton) ||
			OVRInput.GetDown(toggleButtonVR, hand))
			ToggleGrabMode();

		if (longGrabMode)
		{
			if (grabbed == null && Physics.Raycast(transform.position, transform.forward, out hitInfo, 100, ~ignoreLayers))
			{
				Debug.DrawLine(transform.position, hitInfo.point, Color.red);
				longGrabLine.SetPositions(new Vector3[] { transform.position, hitInfo.point });
				distanceGrabbable = hitInfo.collider.GetComponent<Grabbable>();
			}
			else
			{
				longGrabLine.SetPositions(new Vector3[] { transform.position, transform.position + transform.forward * 0.25f });
			}
		}

		if (Application.isEditor && Input.GetKeyDown(grabButton) || OVRInput.GetDown(grabButtonVR, hand))
		{
			grabbed = longGrabMode ? distanceGrabbable : GetBestGrabbable();
			if (grabbed != null) grabbed.Grab(hand, transform, playerRigidbody, longGrabMode);
		}

		if (Application.isEditor && Input.GetKeyUp(grabButton) || OVRInput.GetUp(grabButtonVR, hand))
		{
			if (grabbed != null)
			{
				grabbed.Release(transform);
				grabbed = null;
			}
		}
	}

	void ToggleGrabMode()
	{
		longGrabLine.SetPositions(new Vector3[] { transform.position, transform.position });
		hitInfo = new RaycastHit();
		longGrabMode = !longGrabMode;
	}

	Grabbable GetBestGrabbable()
	{
		if (grabbables.Count > 0)
		{
			Grabbable bestGrabbable = grabbables[0];
			Vector3 minDistance = grabbables[0].transform.position - transform.position;
			for (int i = 1; i < grabbables.Count; i++)
			{
				Vector3 distance = grabbables[i].transform.position - transform.position;
				if (distance.sqrMagnitude < minDistance.sqrMagnitude)
				{
					minDistance = distance;
					bestGrabbable = grabbables[i];
				}
			}
			return bestGrabbable;
		}
		return null;
	}

	void OnTriggerEnter(Collider other)
	{
		Grabbable grabbable = other.GetComponent<Grabbable>();
		if (grabbable != null) grabbables.Add(grabbable);
	}

	void OnTriggerExit(Collider other)
	{
		int index = grabbables.IndexOf(other.GetComponent<Grabbable>());
		if (index >= 0) grabbables.RemoveAt(index);
	}
}
