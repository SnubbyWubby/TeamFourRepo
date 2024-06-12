using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class enemyBullet : MonoBehaviour 
{
	[SerializeField] Rigidbody enyBullet;

	[SerializeField] int dmgBullet;
	[SerializeField] int spdBullet;
	[SerializeField] int dstBullet; 

	// Start Is Called Before The First Frame Update 
	void Start()
	{
		enyBullet.velocity = transform.forward * spdBullet;

		Destroy(gameObject, dstBullet); 
	}

	private void OnTriggerEnter(Collider other) 
	{
		if (other.isTrigger) { return; }

		IDamage damage = other.GetComponent<IDamage>();

		if (damage != null) 
		{
			damage.takeDamage(dmgBullet); 
		}

		Destroy(gameObject);
	}
}