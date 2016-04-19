using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {

	public bool up;
	public bool down;
	public bool left;
	public bool right;
	public bool exterior;
	private int exteriorI = 1;

	public GameObject edgeMeshPrefab;
	private GameObject edgeMesh;

	void Start () {
		Vector3 bounds;
		Vector2 min;
		Vector2 max;
		SpriteRenderer sr = this.GetComponent <SpriteRenderer> ();

		bounds = sr.bounds.size;
		min = sr.bounds.min;
		max = sr.bounds.max;

		if (exterior)
			exteriorI *= -1;

		if (up) {
			InstantiateEdgeMesh (bounds.x, new Vector3 (min.x, max.y, -1), 0);
		}
		if (down) {
			InstantiateEdgeMesh (bounds.x, new Vector3 (max.x, min.y, -1), 180);
		}
		if (left) {
			InstantiateEdgeMesh (bounds.y, new Vector3 (min.x, min.y, -1), 90);
		}
		if (right) {
			InstantiateEdgeMesh (bounds.y, new Vector3 (max.x, max.y, -1), 270);
		}
	}

	private void InstantiateEdgeMesh(float length, Vector3 startPos, float angle)
	{
		edgeMesh = Instantiate (edgeMeshPrefab);
		edgeMesh.transform.localPosition = startPos;
		edgeMesh.transform.Rotate (0,0,angle);
		edgeMesh.GetComponent <EdgeMesh> ().SetUpMesh (length, exteriorI);
		edgeMesh.transform.parent = this.transform;

		Vector3 t = edgeMesh.transform.localPosition;
		t.z = -1;
		edgeMesh.transform.localPosition = t;
	}
}
