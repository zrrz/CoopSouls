using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

	public enum UIState {
		MainMenu, InGame, Paused, Inventory //TODO think about this
	}

	public UIState state = UIState.InGame;

	public InventoryUIManager inventory;

	void Start () {
	
	}
	
	void Update () {
		switch(state) {
			case UIState.InGame:
				if(Input.GetKeyDown(KeyCode.I)) {
					state = UIState.Inventory;
					ShowInventory();
				}
				break;
			case UIState.Inventory:
				if(Input.GetKeyDown(KeyCode.I)) {
					state = UIState.InGame;
					HideInventory();
				}
				break;
			default:
				break;
		}
	}

	void ShowInventory() {
		inventory.gameObject.SetActive(true);
	}

	void HideInventory() {
		inventory.gameObject.SetActive(false);
	}
}
