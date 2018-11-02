using UnityEngine;

public class BrushDrawing : MonoBehaviour
{
	[SerializeField] OVRInput.Controller hand;
	[SerializeField] KeyCode drawButton;
	[SerializeField] OVRInput.RawButton drawButtonVR;
	[SerializeField] LayerMask ignoreLayers;

	RaycastHit hitInfo;
	DrawingBoard board;

	void Update()
	{
		if (Physics.Raycast(transform.position, transform.forward, out hitInfo, 10, ~ignoreLayers))
		{
			DrawingBoard hitObject = hitInfo.collider.GetComponent<DrawingBoard>();
			if (hitObject != null)
			{
				board = hitObject;
				board.MoveMarker(hitInfo.point);

				if ((Application.isEditor && Input.GetKey(drawButton) ||
					OVRInput.Get(drawButtonVR, hand)) && !board.isDrawing)
					board.StartDrawing();
			}
			else StopDrawing();
		}
		else StopDrawing();

		if (board != null)
		{
			if (Application.isEditor && Input.GetKeyDown(drawButton) || OVRInput.GetDown(drawButtonVR, hand))
			{
				board.StartDrawing();
			}
			if (Application.isEditor && Input.GetKeyUp(drawButton) || OVRInput.GetUp(drawButtonVR, hand))
			{
				StopDrawing();
			}
		}
	}

	void StopDrawing()
	{
		if (board != null)
		{
			board.StopDrawing();
			board = null;
		}
	}
}
