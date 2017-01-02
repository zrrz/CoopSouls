using UnityEngine;
using System.Collections;

public class Item {

	public Item() {
		this.itemID = 0; this.rarity = Rarity.Common; this.name = "Default"; this.sprite = null; this.breakable = false; this.maxDurability = 1; this.stackable = false;
	}
//
//	public Item(ushort itemID, Rarity rarity, string name, Sprite sprite, int maxDurability) {
//
//	}
//
//	public Item(ushort itemID, Rarity rarity, string name, Sprite sprite, bool stackable) {
////		Item(itemID, rarity, name, sprite, false, 0, stackable);
//		new Item(0, rarity, "", null, false, 0, false);
//	}

	public Item(ushort itemID, Rarity rarity, string name, Sprite sprite, bool breakable, int maxDurability, bool stackable) {
		this.itemID = itemID; this.rarity = rarity; this.name = name; this.sprite = sprite; this.breakable = breakable; this.maxDurability = maxDurability; this.stackable = stackable;
	}

	public ushort itemID;

	public enum Rarity {
		Common, Uncommon, Rare, Epic, Legendary, Artifact //Gray, Green, Blue, Purple, Orange, Red
	}

	public Rarity rarity;

	public string name;
	public Sprite sprite;

	public bool breakable;
	public int maxDurability;
	public int durability;

	public bool stackable;
	public int amount = 0;
}
