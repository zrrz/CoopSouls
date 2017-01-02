using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {

	public Dictionary<ushort, Item> items;

	void Awake() {
		items = new Dictionary<ushort, Item>();
		items.Add(0, new Item());
		items.Add(1, new Item());
		items.Add(2, new Item());
		items.Add(3, new Item());
		items.Add(4, new Item());
		items.Add(5, new Item());
		items.Add(6, new Item());
		items.Add(7, new Item());
		items.Add(8, new Item());
		items.Add(9, new Item());

	}

	void AddItem(Item item) {
		if(item.stackable) {
			if(items[item.itemID] != null) {
				items[item.itemID].amount++;
			} else {
				items.Add(item.itemID, item);
			}
		} else {
			items.Add(item.itemID, item);
		}
	}

	void AddItem(Item item, int amount) {
		if(!item.stackable) {
			Debug.LogError("Used wrongly for unstackable item");
			return;
		}

		if(items[item.itemID] != null) {
			items[item.itemID].amount += amount;
		} else {
			items.Add(item.itemID, item);
		}

	}

	void RemoveItem(Item item) {
		if(items[item.itemID] != null) {
			if(item.stackable) {
				items[item.itemID].amount--;
			} else {
				items.Remove(item.itemID);
			}
		} else {
			Debug.LogError("Can't remove item - not found");
			return;
		}
	}

	void RemoveItem(Item item, int amount) {
		if(!item.stackable) {
			Debug.LogError("Used wrongly for unstackable item");
			return;
		}

		if(items[item.itemID] != null) {
			items[item.itemID].amount -= amount;
		} else {
			Debug.LogError("Can't remove item - not found");
			return;
		}
	}
}
