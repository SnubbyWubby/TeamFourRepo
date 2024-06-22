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
		Vector3 ptnPlayer = GameManager.Instance.Player.transform.position;   

		enyBullet.velocity = (new Vector3(ptnPlayer.x, ptnPlayer.y + 0.5f, ptnPlayer.z) - transform.position).normalized * spdBullet; 

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