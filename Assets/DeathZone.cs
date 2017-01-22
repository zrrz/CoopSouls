using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other) {
		if(other.GetComponent<DamageHandler>() != null) {
			other.GetComponent<DamageHandler>().TakeDamage(other.GetComponent<DamageHandler>().currentHealth);
		}
	}
}
