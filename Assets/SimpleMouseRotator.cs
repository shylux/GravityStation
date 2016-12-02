using System;
using UnityEngine;
using UnityEngine.Networking;

public class SimpleMouseRotator: NetworkBehaviour {
    public float rotationSpeed = 10;
	public float upTransitionSpeed = .1f;
	public GameObject head;

	private float rotX;
	private GravitySource lastGravSource;
	// true when player switched gravity source and is in transition between them.
	bool inTransition = false;

    private void Start() {
    }

    private void FixedUpdate() {
		if (!isLocalPlayer)
			return;

		GravityAffected ga = GetComponent<GravityAffected>() as GravityAffected;

		//Debug.DrawLine (head.transform.position, head.transform.position + head.transform.forward * 20);

		if (lastGravSource != ga.getNearestSource ()) // started transition
			inTransition = true;

		lastGravSource = ga.getNearestSource ();

		// interpolate up vector to make smooth transition between up change
		Vector3 up = Vector3.MoveTowards (transform.up, ga.getUpVector(), upTransitionSpeed).normalized;

		if (inTransition && up == ga.getUpVector ()) // finished transition
			inTransition = false;
		
		// Handle up vector change
		if (inTransition) {
			Vector3 lastRotation = head.transform.forward;
			// left/right
			transform.rotation = Quaternion.LookRotation (up, -lastRotation * 100000);
			transform.Rotate (Vector3.right, 90f);

			// up/down
			head.transform.LookAt (lastRotation * 100000, up);
			// project new look vector to X plane and read/set angle
			Vector3 projectedX = Vector3.ProjectOnPlane (head.transform.forward, Vector3.Cross (head.transform.forward, transform.up));
			float angleX = Vector3.Angle (projectedX, transform.up);
			rotX = -angleX + 90;
			head.transform.localRotation = Quaternion.Euler (-rotX, 0, 0);
		}

		// read input from mouse or mobile controls
		float inputH = Input.GetAxis("Mouse X");
		float inputV = Input.GetAxis("Mouse Y");

		// up/down
		rotX += inputV * rotationSpeed;
		rotX = Mathf.Clamp (rotX, -80, 80);
		head.transform.localRotation = Quaternion.Euler (-rotX, 0, 0);

		// left/right
		transform.Rotate (new Vector3 (0, inputH * rotationSpeed, 0), Space.Self);
		transform.rotation = Quaternion.LookRotation (up, -transform.forward);
		transform.Rotate (Vector3.right, 90f);
    } 
}