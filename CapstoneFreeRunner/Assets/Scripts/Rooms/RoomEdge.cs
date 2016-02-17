using UnityEngine;
using System.Collections;

public class RoomEdge : MonoBehaviour {

	// Use this for initialization
	void Start () {
        float radius = gameObject.GetComponent<ParticleSystem>().shape.radius;
        gameObject.GetComponent<ParticleSystem>().emissionRate = radius * 1.5f;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
