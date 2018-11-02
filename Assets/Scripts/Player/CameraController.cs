using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] KeyCode modKey = KeyCode.LeftShift;
	[SerializeField] float mouseSensitivity = 3;

	float pan, tilt;
	const string mouseX = "Mouse X", mouseY = "Mouse Y";
	Vector3 desiredRotation;

	void Update()
	{
		if (Application.isEditor && Cursor.lockState != CursorLockMode.None) // Emulate Head movement in Editor
		{
			if (!Input.GetKey(modKey)) // Rotate only when Modifier Key is not pressed
			{
				pan += Input.GetAxis(mouseX) * mouseSensitivity;
				tilt -= Input.GetAxis(mouseY) * mouseSensitivity;
				tilt = Mathf.Clamp(tilt, -80, 80);

				desiredRotation.x = tilt;
				desiredRotation.y = pan;

				transform.localEulerAngles = desiredRotation;
			}
		}
	}
}
