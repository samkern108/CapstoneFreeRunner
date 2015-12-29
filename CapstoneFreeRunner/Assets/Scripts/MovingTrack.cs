using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovingTrack : MonoBehaviour {

	LineRenderer lineRenderer;
	List<Vector3> track = new List<Vector3>();

	public float pauseTimeAtMidpoints = 1f;
	public float moveSpeed;
	public int startIndex;

	public bool reversePathAtEnd;
	public bool haltAtEnd;
	private int completePathInt = 1;
	public bool completePath;
	public bool inMotion;

	public bool pathVisible;

	public Transform movingObject;
	

	void Start () {
		if (!movingObject) {
			movingObject = transform.parent;
		}
		lineRenderer = GetComponent <LineRenderer>();

		BuildTrack ();
		MoveAlongTrack ();
	}

	private void BuildTrack()
	{
		Transform[] trackPieces = GetComponentsInChildren <Transform>();
		lineRenderer.SetVertexCount (trackPieces.Length);

		for(int i = 1; i < trackPieces.Length; i++) {
			track.Add(trackPieces[i].position);
			if(pathVisible) {
				lineRenderer.SetPosition(i-1, track[i-1]);
			}
		}
		if (completePath && pathVisible) {
			lineRenderer.SetPosition (trackPieces.Length - 1, track [0]);
			completePathInt = 0;
		}
	
		index = startIndex;
		endIndex = track.Count - completePathInt;
		posVector = track[index];
		vecToTarget = track [(index + 1) % track.Count] - track [index];
		unitVector = (vecToTarget) / (vecToTarget).magnitude;
	}


	int index;
	private int endIndex;

	Vector3 vecToTarget;
	Vector3 posVector;
	Vector3 unitVector;
	float progression = 0;
	int count = 0;

	private void MoveAlongTrack()
	{
		progression += moveSpeed;
		posVector += unitVector * moveSpeed;

		if((unitVector * progression).magnitude > (vecToTarget).magnitude) {

			count++;
			index = (index + 1) % track.Count;

			if(count == endIndex){
				HandleTrackEnd();
			}

			posVector = track[index];
			vecToTarget = track [(index + 1) % track.Count] - track [index];

			progression = 0;
			unitVector = (vecToTarget) / (vecToTarget).magnitude;

			if(pauseTimeAtMidpoints > 0) {
				inMotion = false;
				Invoke("StartMoving",pauseTimeAtMidpoints);
			}
		}

		movingObject.position = posVector;
	}

	private void StartMoving()
	{
		inMotion = true;
	}

	private void HandleTrackEnd()
	{
		count = 0;
		if (!completePath) {
			index = 0;
		} else if(completePath && reversePathAtEnd) {
			if(index == 0)
				index = track.Count - 1;
			else
				index = 0;
		}

		if (reversePathAtEnd) {
			track.Reverse();
		}
		if (haltAtEnd) {
			inMotion = false;
		}
	}

	void Update () {
		if (inMotion) {
			MoveAlongTrack();
		}
	}

	public void Activate()
	{
		inMotion = true;
	}
}
