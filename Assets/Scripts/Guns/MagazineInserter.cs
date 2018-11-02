using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MagazineInserter : MonoBehaviour
{
	InteractibleGun gunInteractible;

	void Start()
	{
		gunInteractible = GetComponentInParent<InteractibleGun>();
	}

	void OnTriggerEnter(Collider other)
	{
		if (gunInteractible.magazine == null)
		{
			Magazine mag = other.GetComponent<Magazine>();
			if (mag != null && mag.gunType == gunInteractible.gunType)
				mag.Insert(gunInteractible, transform);
		}
	}
}
