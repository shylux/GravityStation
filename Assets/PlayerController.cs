﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

	public float speed = 5.0f;
	public float hover = 10.0f;

	public GameObject bulletPrefab;
	public Transform bulletSpawn;
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!isLocalPlayer)
			return;

		var x = Input.GetAxis ("Horizontal");
		var z = Input.GetAxis ("Vertical");
		var jump = Input.GetKey (KeyCode.Space);
		//Vector3 movement = new Vector3 (x, 0, z).normalized * Time.deltaTime * speed;
		//movement = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * movement;
		//transform.Translate (movement, Space.Self);
		var _oldPosition = transform.position;
		var movement = (Vector3.forward * z + Vector3.right * x) * Time.deltaTime * speed;
		if (jump)
			movement += Vector3.up * hover * Time.deltaTime;
		transform.Translate(movement, Space.Self);
		Debug.DrawLine (_oldPosition, _oldPosition + (transform.position - _oldPosition) * 15);

		if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
			CmdFire();
		}
	}
		
	Camera lastCamera;
	public override void OnStartLocalPlayer () {
		base.OnStartLocalPlayer ();
		//GetComponent<MeshRenderer> ().material.color = Color.blue;

		lastCamera = Camera.allCameras [0];
		lastCamera.enabled = false;
		GetComponentInChildren<Camera> ().enabled = true;
	}
	void OnDestroy() {
		GetComponentInChildren<Camera> ().enabled = false;
		lastCamera.enabled = true;
	}

	[Command]
	void CmdFire() {
//		var bullet = (GameObject)Instantiate (
//			bulletPrefab,
//			bulletSpawn.position,
//			bulletSpawn.rotation
//		);
//
//		bullet.GetComponent<Rigidbody> ().velocity = bullet.transform.forward * 6;
//
//		NetworkServer.Spawn (bullet);
//
//		Destroy (bullet, 2f);
	}
}
