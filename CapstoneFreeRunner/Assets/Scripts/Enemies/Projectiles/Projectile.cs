using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public Vector3 movementVector;
	public float speed = .2f;
	
	public void SetProperties(float speed, float scale, Vector3 position)
	{
		this.transform.position = position;
		this.speed = speed;
		this.transform.localScale = new Vector3 (scale, scale, 1);

		movementVector = PlayerController.PlayerPosition - this.transform.position;
		float distanceToPlayer = Mathf.Sqrt (Mathf.Pow (PlayerController.PlayerPosition.x- this.transform.position.x,2) + Mathf.Pow(PlayerController.PlayerPosition.y - this.transform.position.y,2));
		movementVector = movementVector / distanceToPlayer;
	}

	public void Reset()
	{
		Destroy (this.gameObject);
	}
}
