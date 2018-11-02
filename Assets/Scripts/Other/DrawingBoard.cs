using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawingBoard : MonoBehaviour
{
	[HideInInspector] public bool isDrawing;

	[SerializeField] Transform marker;

	LineRenderer line;

	const float hiddenOffset = 0.02f, visibleOffset = 0.0025f, markerOffset = 0.005f;

	void Start()
	{
		line = GetComponent<LineRenderer>();
	}

	void FixedUpdate()
	{
		if (isDrawing) AddPoint(visibleOffset);
	}

	void AddPoint(float offset)
	{
		line.positionCount++;
		line.SetPosition(line.positionCount - 1, marker.position - transform.forward * offset);
	}

	public void MoveMarker(Vector3 point)
	{
		marker.position = point + transform.forward * markerOffset;
	}

	public void StartDrawing()
	{
		AddPoint(hiddenOffset);
		isDrawing = true;
	}

	public void StopDrawing()
	{
		AddPoint(hiddenOffset);
		isDrawing = false;
		MoveMarker(transform.TransformPoint(transform.localPosition));
	}
}
