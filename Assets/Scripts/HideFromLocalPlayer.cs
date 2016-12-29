using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class HideFromLocalPlayer : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		if (isLocalPlayer) {
			foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>()) {
				renderer.enabled = false;
			}
			foreach (Canvas canvas in gameObject.GetComponentsInChildren<Canvas>()) {
				canvas.enabled = false;
			}
		}
	}
}
