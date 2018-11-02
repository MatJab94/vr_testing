using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider)), RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Holdable))]
public class Magazine : MonoBehaviour
{
	public GunType gunType;

	[SerializeField] InteractibleGun gunInteractible;

	[SerializeField] float ejectVelocity = 2.0f;
	[SerializeField] float insertSpeed = .15f;
	[SerializeField] float insertOffset = 0.075f;

	Collider _collider;
	Rigidbody _rigidbody;

	public int bulletCount = 6;
	public bool IsEmpty { get { return bulletCount < 1; } }

	void Start()
	{
		_collider = GetComponent<Collider>();
		_rigidbody = GetComponent<Rigidbody>();

		if (gunInteractible != null) gunInteractible.magazine = this;
	}

	public void Eject()
	{
		transform.parent = null;

		gunInteractible.magazine = null;
		gunInteractible = null;

		_collider.isTrigger = false;
		_rigidbody.isKinematic = false;
		_rigidbody.velocity = -transform.up * ejectVelocity;
	}

	public void Insert(InteractibleGun interactible, Transform inserter)
	{
		transform.parent = inserter;
		transform.rotation = inserter.rotation;
		transform.position = inserter.position - inserter.up * insertOffset;

		gunInteractible = interactible;
		interactible.magazine = this;

		_collider.isTrigger = true;
		_rigidbody.isKinematic = true;

		StartCoroutine(InsertRoutine());
	}

	IEnumerator InsertRoutine()
	{
		Transform parent = transform.parent;

		while ((transform.position - parent.position).magnitude > 0.01f)
		{
			transform.position = Vector3.MoveTowards(transform.position, parent.position, insertSpeed * Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
	}
}
