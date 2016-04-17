using UnityEngine;
using System.Collections;

public class EdgeMesh : MonoBehaviour {

	private Vector3[] vertices;
	private int[] triangles;
	private Mesh mesh;

	private Vector3[] AddVertices(float length)
	{
		vertices = new Vector3[Mathf.CeilToInt(length) + 3];

		vertices [0] = new Vector3 (0, 0, 0);
		vertices [vertices.Length - 1] = new Vector3 (length, 0, 0);

		for (int i = 1; i < vertices.Length - 1; i++) {
			vertices [i] = new Vector3 (i - 1, 0, 0);
		}

		vertices [vertices.Length - 2] = new Vector3 (length, 0, 0);

		return vertices;
	}

	private int[] Triangulate(Vector3[] vertices)
	{
		triangles = new int[(vertices.Length-2)*3];

		for(int i = 0, j = 0; i < (vertices.Length-2)*3; i+=3, j++) {
			triangles[i] = j;
			triangles[i+1] = j+1;
			triangles[i+2] = j+2;
		}
		return triangles;
	}

	private float exteriorI;

	public void SetUpMesh (float length, float exteriorI) {
		this.exteriorI = exteriorI;
		//Vector3 t = transform.position;
		//t.z = -1;
		//transform.position = t;

		vertices = AddVertices (length);

		MeshRenderer renderer=gameObject.AddComponent<MeshRenderer>();
		renderer.material=new Material(Shader.Find("Hidden/Internal-Colored"));
		renderer.material.color=Color.black;

		MeshFilter filter=gameObject.AddComponent<MeshFilter>();
		mesh=new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = Triangulate(vertices);

		mesh.RecalculateNormals();
		mesh.Optimize();
		filter.mesh=mesh;
	}

	void Update () {
		for (int i = 1; i < vertices.Length - 1; i+=2) {
			this.vertices [i].y = vertices [i].y.Map (0, 8, exteriorI*(-.5f), exteriorI*.5f) * Random.value;
		}

		mesh.vertices = this.vertices;
	}

	void OnBecameVisible() {
		Debug.Log ("Vis");
		this.enabled = true;
	}

	void OnBecameInvisible() {
		Debug.Log ("Invis");
		this.enabled = false;
	}
}
