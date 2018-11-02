using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
	[SerializeField] KeyCode interactButton;
	[SerializeField] OVRInput.RawButton interactButtonVR;
	[SerializeField] OVRInput.Controller hand;

	List<Interactible> interactibles = new List<Interactible>();
	Interactible interactible;

	void Update()
	{
		if (Application.isEditor && Input.GetKeyDown(interactButton) || OVRInput.GetDown(interactButtonVR, hand))
		{
			interactible = GetBestInteractible();
			if (interactible != null) interactible.InteractStart();
		}

		if (Application.isEditor && Input.GetKeyUp(interactButton) || OVRInput.GetUp(interactButtonVR, hand))
		{
			if (interactible != null)
			{
				interactible.InteractEnd();
				interactible = null;
			}
		}
	}

	Interactible GetBestInteractible()
	{
		if (interactibles.Count > 0)
		{
			Interactible bestInteractible = interactibles[0];
			Vector3 minDistance = interactibles[0].transform.position - transform.position;
			for (int i = 1; i < interactibles.Count; i++)
			{
				Vector3 distance = interactibles[i].transform.position - transform.position;
				if (distance.sqrMagnitude < minDistance.sqrMagnitude)
				{
					minDistance = distance;
					bestInteractible = interactibles[i];
				}
			}
			return bestInteractible;
		}
		return null;
	}

	void OnTriggerEnter(Collider other)
	{
		Interactible interactible = other.GetComponent<Interactible>();
		if (interactible != null) interactibles.Add(interactible);
	}

	void OnTriggerExit(Collider other)
	{
		int index = interactibles.IndexOf(other.GetComponent<Interactible>());
		if (index >= 0) interactibles.RemoveAt(index);
	}
}
