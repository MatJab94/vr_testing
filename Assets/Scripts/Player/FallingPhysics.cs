using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingPhysics : MonoBehaviour
{
	[HideInInspector] public bool keepNonKinematic;

	Rigidbody rb;
	float minVelocity = 0.01f;
	float time;
	float timeToSetKinematic = 1f;

	public bool IsColliding { get; private set; }

	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	void Update()
	{
		if (!keepNonKinematic && rb.velocity.magnitude < minVelocity)
		{
			time += Time.deltaTime;
			if (time >= timeToSetKinematic)
			{
				time = 0;
				rb.isKinematic = true;
			}
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		IsColliding = true;
	}

	void OnCollisionStay(Collision collision)
	{
		IsColliding = true;
	}

	private void OnCollisionExit(Collision collision)
	{
		IsColliding = false;
	}
}
