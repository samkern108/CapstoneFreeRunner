using UnityEngine;
using System.Collections;

public class Bullet : Projectile {
	
	Projectile self;
	
	void Update () {
		if (!self) {
			self = this.GetComponent("Projectile") as Projectile;
		}
		this.transform.position += movementVector * speed;
	}
	
	void OnTriggerEnter2D()
	{
		Destroy (this.gameObject);
	}
}
