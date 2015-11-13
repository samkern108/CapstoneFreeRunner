using UnityEngine;
using System.Collections;

public class HomingBullet : Projectile {

	Projectile self;
	public float accuracy = .5f;

	void Update () {
		if (!self) {
			self = this.GetComponent("Projectile") as Projectile;
		}

		movementVector = PlayerController.self.transform.position - this.transform.position;
		float distanceToPlayer = Mathf.Sqrt (Mathf.Pow (PlayerController.self.transform.position.x - this.transform.position.x,2) + Mathf.Pow(PlayerController.self.transform.position.y - this.transform.position.y,2));
		movementVector = movementVector / distanceToPlayer;

		this.transform.position += movementVector * speed;
	}
	
	void OnTriggerEnter2D()
	{
		Destroy (this.gameObject);
	}
}
