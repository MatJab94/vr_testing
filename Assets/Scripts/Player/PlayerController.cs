using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] KeyCode modKey = KeyCode.LeftShift;
	[SerializeField] float rotateAmount = 30;
	[SerializeField] Transform head;
	[SerializeField] CapsuleCollider _collider;

	int direction;

	void Update()
	{
		if (Application.isEditor) // Rotate in Editor
		{
			if (!Input.GetKey(modKey)) // Rotate only when Modifier Key is not Pressed
			{
				direction = Input.GetKeyDown(KeyCode.Q) ? -1 : Input.GetKeyDown(KeyCode.E) ? 1 : 0;
			}
		}
		else // Rotate in VR
		{
			direction = OVRInput.GetDown(OVRInput.Button.Left) ? -1 : OVRInput.GetDown(OVRInput.Button.Right) ? 1 : 0;
		}
		transform.eulerAngles += new Vector3(0, rotateAmount * direction, 0);

		_collider.center = new Vector3(head.localPosition.x, _collider.center.y, head.localPosition.z);
	}
}
