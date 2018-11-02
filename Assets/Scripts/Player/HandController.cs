using UnityEngine;
using UnityEngine.XR;

public class HandController : MonoBehaviour
{
	[SerializeField] XRNode handNode;
	[SerializeField] Vector3 editorDefaultPosition;
	[SerializeField] KeyCode modKey;
	[SerializeField] KeyCode cursorLockKey;
	[SerializeField] float mouseSensitivity;
	[SerializeField] GameObject controllerModel;

	float pan, tilt;
	const string vertical = "Vertical", horizontal = "Horizontal",
				 mouseX = "Mouse X", mouseY = "Mouse Y";
	float inputX, inputY, inputZ;
	Vector3 desiredPosition;
	Quaternion desiredRotation;
	OVRInput.Controller controller;

	void Start()
	{
		controller = handNode == XRNode.RightHand ? OVRInput.Controller.RTrackedRemote : OVRInput.Controller.LTrackedRemote;

		// Initialise Hand Position and Cursor in Editor
		if (Application.isEditor)
		{
			desiredPosition = editorDefaultPosition;
			LockCursor(true);
		}
	}

	void Update()
	{
		UpdateHandMovement();

		if (Input.GetKeyDown(cursorLockKey)) LockCursor(Cursor.visible);
	}

	void LockCursor(bool lockCursor)
	{
		if (handNode == XRNode.LeftHand) return;
		Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
		Cursor.visible = !lockCursor;
	}

	void UpdateHandMovement()
	{
		if (Application.isEditor) // Emulate Hand movement in Editor
		{
			if (Input.GetKey(modKey)) // Move only when Modifier Key is Pressed
			{
				inputX = Input.GetAxisRaw(horizontal);
				inputY = Input.GetKey(KeyCode.E) ? 1 : Input.GetKey(KeyCode.Q) ? -1 : 0;
				inputZ = Input.GetAxisRaw(vertical);

				if (inputX != 0) desiredPosition += Vector3.right * inputX * Time.deltaTime;
				if (inputY != 0) desiredPosition += Vector3.up * inputY * Time.deltaTime;
				if (inputZ != 0) desiredPosition += Vector3.forward * inputZ * Time.deltaTime;

				pan += Input.GetAxis(mouseX) * mouseSensitivity;
				pan = Mathf.Clamp(pan, -80, 80);
				tilt -= Input.GetAxis(mouseY) * mouseSensitivity;
				tilt = Mathf.Clamp(tilt, -80, 80);

				desiredRotation = Quaternion.Euler(tilt, pan, 0);
			}
		}
		else // Move Hand in VR
		{
			controllerModel.SetActive(OVRInput.IsControllerConnected(controller));

			desiredPosition = InputTracking.GetLocalPosition(handNode);
			desiredRotation = InputTracking.GetLocalRotation(handNode);
		}

		transform.localPosition = desiredPosition;
		transform.localRotation = desiredRotation;
	}
}
