using UnityEngine;
using System.Collections;

public class ProjectileSpawner : MonoBehaviour {

	public GameObject projectile;
	public float fireRate = 3;
	public float countDown = 0;
	public float projectileSpeed = .2f;
	public float projectileScale = 2f;

	public void Update()
	{
		countDown += Time.deltaTime;

		if (countDown > fireRate) {
			Projectile p = Instantiate (projectile).GetComponent("Projectile") as Projectile;
			p.SetProperties(projectileSpeed, projectileScale, this.transform.position);
			countDown = 0;
		}
	}

	public void Reset()
	{
		countDown = 0;
	}

}
