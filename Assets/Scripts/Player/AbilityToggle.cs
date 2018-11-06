using UnityEngine;

public class AbilityToggle : MonoBehaviour
{
	[SerializeField] KeyCode toggleButton;
	[SerializeField] OVRInput.RawButton toggleButtonVR;
	[SerializeField] OVRInput.Controller hand;
	[SerializeField] int defaultAbility;

	IToggleable[] toggleables;

	int currentAbility;

	void Start()
	{
		currentAbility = defaultAbility;

		toggleables = GetComponents<IToggleable>();
		if (toggleables.Length > 0)
		{
			foreach (var toggleable in toggleables) toggleable.Deactivate();
			if (defaultAbility >= 0 && defaultAbility < toggleables.Length)
				toggleables[defaultAbility].Activate();
		}
	}

	void Update()
	{
		if (Application.isEditor && Input.GetKeyDown(toggleButton) ||
			OVRInput.GetDown(toggleButtonVR, hand))
			ToggleActive();
	}

	void ToggleActive()
	{
		if (currentAbility >= 0) toggleables[currentAbility].Deactivate();

		currentAbility++;

		if (currentAbility >= toggleables.Length) currentAbility = -1;
		else toggleables[currentAbility].Activate();
	}
}
