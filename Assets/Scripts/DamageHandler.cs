using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Events;

public class DamageHandler : MonoBehaviour /*NetworkBehaviour*/ {

	[HideInInspector]
	public UnityEvent OnDeath;

	public float maxHealth = 100f;

//	[SyncVar(hook = "OnChangeHealth")]
	public float currentHealth;

	void Start() {
		currentHealth = maxHealth;
	}

	public void TakeDamage(float amount)
	{
//		if (!isServer)
//		{
//			return;
//		}

		currentHealth -= amount;
		if (currentHealth <= 0)
		{
			currentHealth = 0;
			Debug.Log("Dead!");
			OnDeath.Invoke();
		}
	}

//	public void CmdTakeDamage(float amount) {
//
//	}

	void OnChangeHealth (float health)
	{
		//I get called on the client
	}
}
