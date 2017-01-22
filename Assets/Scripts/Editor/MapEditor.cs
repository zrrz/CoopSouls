using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapEditor : EditorWindow {

	[MenuItem ("Window/Map Editor")]
	static void Init () {
		// Get existing open window or if none, make a new one:
		MapEditor window = (MapEditor)EditorWindow.GetWindow (typeof (MapEditor));
		window.Show();
	}

	void OnGUI () {
		if(GUILayout.Button("Duplicate Left")) {
			GameObject newTile = (GameObject)Instantiate(Selection.activeGameObject, Selection.activeGameObject.transform.position, Selection.activeGameObject.transform.rotation);
			newTile.name = newTile.name.Replace("(Clone)", "");
			newTile.transform.position += new Vector3(-9.25f, 0, 0f);
			Selection.activeGameObject = newTile;
		}
		if(GUILayout.Button("Duplicate Right")) {
			GameObject newTile = (GameObject)Instantiate(Selection.activeGameObject, Selection.activeGameObject.transform.position, Selection.activeGameObject.transform.rotation);
			newTile.name = newTile.name.Replace("(Clone)", "");
			newTile.transform.position += new Vector3(9.25f, 0, 0f);
			Selection.activeGameObject = newTile;
		}
	}
}
