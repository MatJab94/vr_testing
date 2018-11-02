using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Holdable))]
public class InteractibleGun : Interactible
{
	public Magazine magazine;
	public GunType gunType;

	[SerializeField] KeyCode ejectMagazineButton = KeyCode.R;
	[SerializeField] OVRInput.RawButton ejectMagazineButtonVR = OVRInput.RawButton.DpadDown;

	[SerializeField] GameObject bulletPrefab;
	[SerializeField] GameObject shellPrefab;

	[SerializeField] Transform bulletSpawn;
	[SerializeField] Transform shellSpawn;
	[SerializeField] float shellVelocity = 1;

	[SerializeField] bool autoFire;
	[SerializeField] float autoFireTime;
	[SerializeField] Animator animator;

	Holdable gunHoldable;
	bool active;

	void Start()
	{
		gunHoldable = GetComponent<Holdable>();
	}

	void Update()
	{
		InteractionUpdate();

		if (gunHoldable.IsGrabbed && magazine != null)
		{
			if (Application.isEditor && Input.GetKeyDown(ejectMagazineButton) ||
				OVRInput.GetDown(ejectMagazineButtonVR, gunHoldable.HoldingHand))
			{
				magazine.Eject();
			}
		}
	}

	protected override void InteractionStarted()
	{
		active = true;

		if (autoFire) StartCoroutine(AutoFire());
		else Fire();
	}

	protected override void InteractionActive() { }

	protected override void InteractionEnded()
	{
		active = false;
	}

	IEnumerator AutoFire()
	{
		while (active)
		{
			Fire();
			yield return new WaitForSeconds(autoFireTime);
		}
	}

	void Fire()
	{
		if (magazine != null && !magazine.IsEmpty)
		{
			magazine.bulletCount--;
			Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
			if (animator != null) animator.SetTrigger("ShotFired");

		}
	}

	public void EjectShell()
	{
		GameObject shell = Instantiate(shellPrefab, shellSpawn.position, shellSpawn.rotation);
		float ejectAngle = Random.Range(10f, 80f);
		Vector3 ejectVelocity = Quaternion.AngleAxis(ejectAngle, shellSpawn.up) * shellSpawn.forward * shellVelocity;
		shell.GetComponent<Rigidbody>().velocity = ejectVelocity;
	}
}
