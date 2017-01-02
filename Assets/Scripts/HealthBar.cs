using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

	public DamageHandler damageHandler;

	Transform healthImage;

	void Start () {
		healthImage = transform.FindChild("Foreground");
	}
	
	void Update () {
		Vector3 scale = healthImage.localScale;
		scale.x = damageHandler.currentHealth/damageHandler.maxHealth;
		healthImage.localScale = scale;
	}
}
