using UnityEngine;
using System.Collections;

public static class DebugHelper {
	
	public static void DrawRect(Vector2 pos, Vector2 size, Color col, float duration) {
		Debug.DrawLine(new Vector3(pos.x - size.x/2f, pos.y - size.y/2f, 0f), new Vector3(pos.x + size.x/2f, pos.y - size.y/2f, 0f), col, duration);
		Debug.DrawLine(new Vector3(pos.x - size.x/2f, pos.y - size.y/2f, 0f), new Vector3(pos.x - size.x/2f, pos.y + size.y/2f, 0f), col, duration);
		Debug.DrawLine(new Vector3(pos.x + size.x/2f, pos.y - size.y/2f, 0f), new Vector3(pos.x + size.x/2f, pos.y + size.y/2f, 0f), col, duration);
		Debug.DrawLine(new Vector3(pos.x - size.x/2f, pos.y + size.y/2f, 0f), new Vector3(pos.x + size.x/2f, pos.y + size.y/2f, 0f), col, duration);
	}
}
