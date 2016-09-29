using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MouseLocker : NetworkBehaviour {
	private CursorLockMode originalState;

	private void Start() {
		originalState = Cursor.lockState;
		Cursor.lockState = wantedMode = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void OnDestroy() {
		Cursor.lockState = originalState;
		Cursor.visible = (CursorLockMode.Locked != Cursor.lockState);
	}

	CursorLockMode wantedMode;
	void OnGUI () {
		if (!isLocalPlayer)
			return;
		
		if (Input.GetKeyDown (KeyCode.Escape))
			Cursor.lockState = wantedMode = CursorLockMode.None;
		if (Input.GetMouseButtonDown(0))
			Cursor.lockState = wantedMode = CursorLockMode.Locked;

		Cursor.lockState = wantedMode;
		// Hide cursor when locking
		Cursor.visible = (CursorLockMode.Locked != wantedMode);
	}
}
