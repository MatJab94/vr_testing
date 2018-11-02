using UnityEngine;

public class InteractibleMove : Interactible
{
	[SerializeField] float speed = 2;
	[SerializeField] Vector3 localPointA;
	[SerializeField] Vector3 localPointB;

	float percentage;
	bool pingPong;

	Vector3 pointA;
	Vector3 pointB;

	private void Start()
	{
		pointA = transform.TransformPoint(localPointA);
		pointB = transform.TransformPoint(localPointB);
	}

	protected override void InteractionStarted()
	{

	}

	protected override void InteractionActive()
	{
		if (percentage > 1) pingPong = true;
		if (percentage < 0) pingPong = false;
		percentage -= pingPong ? Time.deltaTime * speed : Time.deltaTime * -speed;
		transform.position = Vector3.Lerp(pointA, pointB, percentage);
	}

	protected override void InteractionEnded()
	{

	}
}
