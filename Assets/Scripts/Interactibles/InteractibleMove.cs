using System.Collections;
using UnityEngine;

public class InteractibleMove : Interactible
{
	[SerializeField] float speed = 2;
	[SerializeField] bool continuousMove;
	[SerializeField] bool useWorldCoords;
	[SerializeField] Vector3 pointA;
	[SerializeField] Vector3 pointB;

	float percentage;
	bool pingPong;

	private void Start()
	{
		if (!useWorldCoords)
		{
			pointA = transform.TransformPoint(pointA);
			pointB = transform.TransformPoint(pointB);
		}
	}

	protected override void InteractionStarted()
	{
		if (!continuousMove) StartCoroutine(Move(true));
	}

	protected override void InteractionActive()
	{
		if (continuousMove)
		{
			if (percentage > 1) pingPong = true;
			if (percentage < 0) pingPong = false;
			percentage -= pingPong ? Time.deltaTime * speed : Time.deltaTime * -speed;
			transform.position = Vector3.Lerp(pointA, pointB, percentage);
		}
	}

	protected override void InteractionEnded()
	{
		if (!continuousMove) StartCoroutine(Move(false));
	}

	IEnumerator Move(bool flag)
	{
		percentage = flag ? 0 : 1;
		while (percentage >= 0 && percentage <= 1)
		{
			percentage += flag ? Time.deltaTime * speed : Time.deltaTime * -speed;
			transform.position = Vector3.Lerp(pointA, pointB, percentage);
			yield return new WaitForEndOfFrame();
		}
	}
}
