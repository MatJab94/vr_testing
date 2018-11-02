using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ClimbingPhysics : MonoBehaviour
{
	[HideInInspector] public bool isClimbing;

	Rigidbody rb;
	float minVelocity = 0.01f;
	float time;
	float timeToSetKinematic = 1f;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	void Update()
	{
		if (!isClimbing && rb.velocity.magnitude < minVelocity)
		{
			time += Time.deltaTime;
			if (time >= timeToSetKinematic)
			{
				time = 0;
				rb.isKinematic = true;
			}
		}
	}
}
