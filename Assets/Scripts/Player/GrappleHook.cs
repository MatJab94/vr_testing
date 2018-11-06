using UnityEngine;

public class GrappleHook : MonoBehaviour, IToggleable
{
	[Header("Controls")]
	[SerializeField] OVRInput.Controller hand;
	[SerializeField] KeyCode grappleButton;
	[SerializeField] OVRInput.RawButton grappleButtonVR;
	[SerializeField] OVRInput.RawButton stopButtonVR;
	[SerializeField] KeyCode longerRopeButton;
	[SerializeField] KeyCode shorterRopeButton;
	[SerializeField] OVRInput.RawAxis1D ropeAxisVR;

	[Header("Settings")]
	[SerializeField] float reelSpeed = 1;
	[SerializeField] float minDistance = 0.5f;
	[SerializeField] float maxDistance = 10f;
	[SerializeField] LayerMask ignoreLayers;
	[SerializeField] Rigidbody player;
	[SerializeField] FallingPhysics fallingPhysics;
	[SerializeField] LineRenderer lineRenderer;
	[SerializeField] GrappleHook otherHandGrapple;

	Rigidbody connectedBody;
	ConfigurableJoint joint;
	Rigidbody jointRB;
	RaycastHit hitInfo;
	bool bounce;
	bool showLine;
	bool isActive;

	void Update()
	{
		if (!isActive) return;

		if (Application.isEditor && Input.GetKeyDown(grappleButton) || OVRInput.GetDown(grappleButtonVR, hand))
			CreateJoint();

		if (Application.isEditor && Input.GetKeyUp(grappleButton) || OVRInput.GetUp(grappleButtonVR, hand))
			BreakJoint();

		if (joint != null)
		{
			if (Application.isEditor)
			{
				if (Input.GetKey(longerRopeButton)) ReelRope(reelSpeed);
				if (Input.GetKey(shorterRopeButton)) ReelRope(-reelSpeed);
			}
			else
			{
				float ropeAxis = OVRInput.Get(ropeAxisVR, hand);
				if (ropeAxis >= 0.7f && !OVRInput.Get(stopButtonVR, hand))
					ReelRope(reelSpeed * ropeAxis * ropeAxis);
			}

			lineRenderer.positionCount = 2;
			lineRenderer.SetPositions(new Vector3[] { transform.position, connectedBody.position });
		}
		else
		{
			showLine = Physics.Raycast(transform.position, transform.forward, out hitInfo, maxDistance, ~ignoreLayers);
			if (showLine)
			{
				lineRenderer.positionCount = 2;
				lineRenderer.SetPositions(new Vector3[] { transform.position, hitInfo.point });
			}
			else lineRenderer.positionCount = 0;
		}
	}

	void FixedUpdate()
	{
		if (joint != null)
		{
			Vector3 positionDelta = joint.transform.position - transform.position;
			if (fallingPhysics.IsColliding && !bounce)
			{
				jointRB.velocity = (-positionDelta * 0.5f) / Time.fixedDeltaTime;
				bounce = true;
			}
			else
			{
				player.velocity = positionDelta / Time.fixedDeltaTime;
				bounce = false;
			}
		}
	}

	void ReelRope(float speed)
	{
		Vector3 newAnchor = joint.anchor - joint.anchor.normalized * speed * Time.deltaTime;
		if (newAnchor.magnitude < minDistance || newAnchor.magnitude > maxDistance) return;
		joint.anchor = newAnchor;
	}

	void CreateJoint()
	{
		// break previous joints
		BreakJoint();
		otherHandGrapple.BreakJoint();

		//check if we can hook to something
		if (Physics.Raycast(transform.position, transform.forward, out hitInfo, maxDistance, ~ignoreLayers))
		{
			// configure hook point
			connectedBody = new GameObject("Connected Body").AddComponent<Rigidbody>();
			connectedBody.isKinematic = true;
			connectedBody.transform.position = hitInfo.point;
			connectedBody.transform.forward = hitInfo.normal;

			// configure new joint
			joint = new GameObject("Joint Object").AddComponent<ConfigurableJoint>();
			jointRB = joint.gameObject.GetComponent<Rigidbody>();
			joint.transform.position = transform.position;
			joint.autoConfigureConnectedAnchor = false;
			joint.xMotion = joint.yMotion = joint.zMotion = ConfigurableJointMotion.Limited;
			joint.connectedBody = connectedBody;
			joint.anchor = connectedBody.position - transform.position;
			joint.connectedAnchor = Vector3.zero;
			jointRB.velocity += player.velocity;

			// configure player
			player.isKinematic = false;
			fallingPhysics.keepNonKinematic = true;

			showLine = true;
		}
	}

	public void BreakJoint()
	{
		if (joint != null)
		{
			Destroy(joint.gameObject);
			joint = null;
			jointRB = null;
			Destroy(connectedBody.gameObject);
			connectedBody = null;
			fallingPhysics.keepNonKinematic = false;
		}
	}

	public void Deactivate()
	{
		isActive = false;
		if (joint != null) BreakJoint();
		lineRenderer.enabled = false;
	}

	public void Activate()
	{
		isActive = true;
		lineRenderer.enabled = true;
	}

	void OnDrawGizmos()
	{
		if (joint == null) return;
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(joint.transform.position, 0.1f);
		Gizmos.DrawLine(joint.transform.position, connectedBody.transform.position);
	}
}
