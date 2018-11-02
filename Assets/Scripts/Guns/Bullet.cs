using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
	[SerializeField] GameObject BulletHolePrefab;
	[SerializeField] LayerMask ignoreLayers;

	public float velocity = 10;
	public float shareScale = 0.5f;
	public float lifespan = 5;
	Rigidbody rb;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		rb.velocity = transform.forward * velocity;
		StartCoroutine(AutoDestruct());
	}

	IEnumerator AutoDestruct()
	{
		yield return new WaitForSeconds(lifespan);
		Destroy(gameObject);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<Grabber>()) return;

		var rigidbody = other.GetComponent<Rigidbody>();
		if (rigidbody != null) rigidbody.velocity += (rb.velocity * shareScale) / rigidbody.mass;
		RaycastHit hitInfo;
		Vector3 rayOrigin = transform.position - rb.velocity * Time.fixedDeltaTime * 2;
		if (Physics.Raycast(rayOrigin, transform.forward, out hitInfo, 2f, ~ignoreLayers))
		{
			GameObject hole = Instantiate(BulletHolePrefab, hitInfo.point, Quaternion.identity);
			hole.transform.forward = hitInfo.normal;
			hole.transform.parent = hitInfo.collider.transform;
		}
		Destroy(gameObject);
	}
}
