using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

	DamageHandler damageHandler;

	void Start () {
		damageHandler = GetComponent<DamageHandler>();
		damageHandler.OnDeath.AddListener(OnDeath);
	}
	
	void Update () {
	
	}

	void OnCollisionStay2D(Collision2D col) {
//		if(col.collider.tag == "Player") {
//			damageHandler.TakeDamage(100f*Time.deltaTime);
//		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		if(col.gameObject.tag == "PlayerAttack") {
			damageHandler.TakeDamage(35f);
			Destroy(col.gameObject);
		}
	}

	void OnDeath() {
		DropLoot();
		Destroy(gameObject);
	}

	void DropLoot() {
		LootManager.Instance.DropDust(transform.position, 100, 10, 1f);
	}
}
