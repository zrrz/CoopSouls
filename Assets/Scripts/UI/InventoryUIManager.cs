using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour {

	public bool isDirty = true;

	public Inventory inventory;

	public Transform itemHolder;

	public GameObject itemSectionPrefab;

	void Start () {
	
	}
	
	void Update () {
		if(isDirty) {
			GenerateInventoryList();
			isDirty = false;
		}
	}

	void GenerateInventoryList() {
		for(int i = 0; i < itemHolder.childCount; i++) { //TODO less brute force of deleting them all
			Destroy(itemHolder.GetChild(i).gameObject);
		}
		itemHolder.DetachChildren();

		List<Item> items = new List<Item>(inventory.items.Values);
		Vector2 sizeDelta = itemHolder.GetComponent<RectTransform>().sizeDelta;
		sizeDelta.y = 70f * items.Count;
		itemHolder.GetComponent<RectTransform>().sizeDelta = sizeDelta;
		foreach(Item item in items) {
			GameObject itemSection = (GameObject)Instantiate(itemSectionPrefab);
			itemSection.transform.SetParent(itemHolder);
			itemSection.transform.localScale = Vector3.one;
			itemSection.transform.localPosition = new Vector3(197f, -35f - (70f * (itemHolder.childCount - 1)), 0f);
			itemSection.transform.FindChild("Name").GetComponent<Text>().text = item.name;
			if(item.stackable)
				itemSection.transform.FindChild("Amount").GetComponent<Text>().text = item.amount.ToString();
			else
				itemSection.transform.FindChild("Amount").gameObject.SetActive(false);
			itemSection.transform.FindChild("Image").GetComponent<Image>().sprite = item.sprite;
		}
	}
}
