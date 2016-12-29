using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class HideFromLocalPlayer : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		if (isLocalPlayer) {
			foreach (MeshRenderer renderer in gameObject.GetComponentsInChildren<MeshRenderer>()) {
				renderer.enabled = false;
			}
			foreach (Canvas canvas in gameObject.GetComponentsInChildren<Canvas>()) {
				canvas.enabled = false;
			}
		}
	}
}
