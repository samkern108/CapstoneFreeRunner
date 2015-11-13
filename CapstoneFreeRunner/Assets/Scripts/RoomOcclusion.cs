using UnityEngine;
using System.Collections;

public class RoomOcclusion : MonoBehaviour {

	void OnTriggerEnter2D()
	{
		SpriteRenderer s = this.GetComponent ("SpriteRenderer") as SpriteRenderer;
		s.enabled = true;
		foreach (Transform child in transform.parent)
		{
			if (child != transform.parent && child != this.transform){
				SpriteRenderer r = child.gameObject.GetComponent("SpriteRenderer") as SpriteRenderer;
				r.enabled = false;
			}
		}
	}
}
