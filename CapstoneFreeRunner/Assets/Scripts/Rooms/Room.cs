using UnityEngine;
using System.Collections;

public class Room : MonoBehaviour {

	public bool initiallyActive = false;
	private Vector3 originalScale;

	void Start () {
		gameObject.SetActive (initiallyActive);
		originalScale = this.transform.localScale;
	}

	public void RoomDisappearEffect()
	{
		StartCoroutine ("RoomDisappear");
	}

	public void RoomAppearEffect()
	{
		StartCoroutine ("RoomAppear");
	}

	IEnumerator RoomDisappear()
	{
		float opacity = 100;
		float size = 100;
		while(opacity > 0)
		{
			yield return null;
		}
	}

	IEnumerator RoomAppear()
	{
		float opacity = 0;
		float size = 0;
		while( opacity < 100)
		{
			
			yield return null;
		}
	}
		
}
