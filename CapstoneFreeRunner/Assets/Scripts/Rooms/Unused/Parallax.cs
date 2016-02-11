using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour {

	Vector3 initialPosition;
	float offsetX;
	float offsetY;
	float offsetAmountX;
	float offsetAmountY;
	float zPos = 0;

	public void Init(Vector3 initialPosition, float offsetAmountX, float offsetAmountY, float zPos)
	{
		this.offsetAmountX = offsetAmountX;

		if (offsetAmountY < 0) {
			this.offsetAmountY = 0;
		} else {
			this.offsetAmountY = offsetAmountY;
		}
		this.initialPosition = initialPosition;
		this.zPos = zPos;
	}
		
	void Update () {
		offsetX = offsetAmountX * (initialPosition.x - PlayerController.PlayerPosition.x);
		offsetY = offsetAmountY * Mathf.Abs((initialPosition.y - PlayerController.PlayerPosition.y));

		this.transform.localPosition = new Vector3(offsetX, -offsetY, zPos);
	}
}
