using UnityEngine;
using System.Collections;

public class LootManager : MonoBehaviour {

	static LootManager s_instance;

	public static LootManager Instance {
		get { return s_instance; }
	}

	public GameObject dustPrefab;

	void Awake() {
		if(s_instance != null) {
			Destroy(this);
			return;
		} else {
			s_instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	public void DropDust(Vector2 position, int amount, int variance, float multiplier) {
		GameObject dust = (GameObject)Instantiate(dustPrefab, position, Quaternion.identity);
		dust.GetComponent<Dust>().SetAmount((int)((float)(amount + Random.Range(-variance, variance)) * multiplier));
	}

	void Start () {
	
	}
	
	void Update () {
	
	}
}
