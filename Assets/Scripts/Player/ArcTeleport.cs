using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ArcTeleport : MonoBehaviour
{
	[Header("Controls")]
	[SerializeField] KeyCode toggleButton;
	[SerializeField] OVRInput.RawButton toggleButtonVR;
	[SerializeField] KeyCode teleportButton;
	[SerializeField] OVRInput.RawButton teleportButtonVR;
	[SerializeField] OVRInput.Controller hand;

	[Header("Player/Collision Settings")]
	[SerializeField] Transform playerTransform;
	[SerializeField] CapsuleCollider playerCollider;
	[SerializeField, Range(0.01f, 0.1f)] float skin;
	[SerializeField, Range(0, 60)] float maxGroundAngle;
	[SerializeField] LayerMask ignoreLayers;

	[Header("Arc Display Settings")]
	[SerializeField] Transform arcMarker;
	[SerializeField] MeshRenderer arcMarkerRenderer;
	[SerializeField] Color goodTeleport;
	[SerializeField] Color badTeleport;

	[Header("Arc Settings")]
	[SerializeField, Range(1, 10)] float arcMaxDistance;
	[SerializeField, Range(25, 150)] int maxSegmentCount;
	[SerializeField, Range(0.1f, 2f)] float segScale;

	bool isTeleportActive, isTeleportValid;
	RaycastHit hit;
	float dot, minValidDot;
	LineRenderer arcRenderer;
	Vector3[] segments;
	Vector3 segVelocity;
	int finalSegmentCount;

	void Awake()
	{
		arcRenderer = GetComponent<LineRenderer>();
		SetTeleportValidity(false);
		segments = new Vector3[maxSegmentCount];
		minValidDot = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
	}

	void Update()
	{
		if (Application.isEditor && Input.GetKeyDown(toggleButton) ||
			OVRInput.GetDown(toggleButtonVR, hand))
			ToggleActive();
		if (!isTeleportActive) return;

		SimulatePath();

		if (Application.isEditor && Input.GetKeyDown(teleportButton) ||
			OVRInput.GetDown(teleportButtonVR, hand))
			Teleport();
	}

	void ToggleActive()
	{
		if (isTeleportActive)
		{
			arcMarker.gameObject.SetActive(false);
			arcRenderer.enabled = false;
		}
		else
		{
			arcMarker.gameObject.SetActive(true);
			arcRenderer.enabled = true;
		}
		isTeleportActive = !isTeleportActive;
	}

	void SimulatePath()
	{
		segments[0] = transform.position;
		segVelocity = transform.forward * arcMaxDistance;
		hit.point = playerTransform.position;
		arcMarker.gameObject.SetActive(false);
		for (int i = 1; i < maxSegmentCount; i++)
		{
			float segTime = (segVelocity.sqrMagnitude != 0) ? segScale / segVelocity.magnitude : 0;
			segVelocity += Physics.gravity * segTime;
			if (Physics.Raycast(segments[i - 1], segVelocity, out hit, segScale + 0.1f, ~ignoreLayers))
			{
				CheckTeleportValidity();
				finalSegmentCount = i + 1;
				segments[i] = arcMarker.position = hit.point;
				arcMarker.up = hit.normal;
				arcMarker.gameObject.SetActive(true);
				break;
			}
			else segments[i] = segments[i - 1] + segVelocity * segTime;
		}
		arcRenderer.positionCount = finalSegmentCount;
		for (int i = 0; i < finalSegmentCount; i++) arcRenderer.SetPosition(i, segments[i]);
	}

	void CheckTeleportValidity()
	{
		dot = Vector3.Dot(hit.normal, playerTransform.up);
		if (dot >= minValidDot)
		{
			float radius = playerCollider.radius;
			Vector3 start = hit.point + (radius + skin) * playerTransform.up;
			Vector3 end = start + (playerCollider.height - 2 * (radius + skin)) * playerTransform.up;
			if (Physics.CheckCapsule(start, end, radius - skin, ~ignoreLayers)) SetTeleportValidity(false);
			else SetTeleportValidity(true);
		}
		else SetTeleportValidity(false);
	}

	void SetTeleportValidity(bool isValid)
	{
		isTeleportValid = isValid;
		if (isTeleportValid)
		{
			arcRenderer.startColor = arcRenderer.endColor = goodTeleport;
			arcMarkerRenderer.material.color = goodTeleport;
		}
		else
		{
			arcRenderer.startColor = arcRenderer.endColor = badTeleport;
			arcMarkerRenderer.material.color = badTeleport;
		}
	}

	void Teleport()
	{
		if (isTeleportValid) playerTransform.position = hit.point;
	}
}