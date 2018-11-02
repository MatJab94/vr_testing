using UnityEngine;

public class Lever : MonoBehaviour
{
	[SerializeField] Transform leverTransform;
	[SerializeField] Holdable leverHoldable;
	[SerializeField] Vector3[] points;
	[SerializeField] Interactible[] interactibles;
	[SerializeField] float activateDistance;

	Interactible current;
	Vector3[] worldPoints;

	void Start()
	{
		if (points.Length != interactibles.Length)
			Debug.LogError("Points and Interactibles must have the same length.", this);

		worldPoints = new Vector3[points.Length];
		for (int i = 0; i < points.Length; i++) worldPoints[i] = transform.TransformPoint(points[i]);
	}

	void Update()
	{
		for (int i = 0; i < points.Length; i++)
		{
			if ((leverTransform.position - worldPoints[i]).magnitude <= activateDistance)
			{
				if (current != interactibles[i])
				{
					if (!leverHoldable.IsGrabbed) SnapToPosition(worldPoints[i]);
					current = interactibles[i];
					current.InteractStart();
				}
			}
			else
			{
				if (current == interactibles[i])
				{
					current.InteractEnd();
					current = null;
				}
			}
		}
	}

	void SnapToPosition(Vector3 position)
	{
		leverTransform.position = position;
		leverTransform.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
	}

	private void OnDrawGizmos()
	{
		worldPoints = new Vector3[points.Length];
		for (int i = 0; i < points.Length; i++) worldPoints[i] = transform.TransformPoint(points[i]);

		Gizmos.color = Color.red;
		foreach (var point in worldPoints) Gizmos.DrawWireSphere(point, activateDistance);
	}
}
