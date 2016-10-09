using System;
using UnityEngine;
using UnityEngine.Networking;

public class SimpleMouseRotator: NetworkBehaviour {
    public float rotationSpeed = 10;
	public GameObject head;

	private float rotX, rotY;
	private GravitySource lastGravSource;

    private void Start() {
    }

	Quaternion rot = Quaternion.identity;
	private Vector3 globalLookAt = Vector3.zero;
    private void FixedUpdate() {
		if (!isLocalPlayer)
			return;
		
//		if (rot != Quaternion.identity)
//			return;

		GravityAffected ga = GetComponent<GravityAffected>() as GravityAffected;

		if (!lastGravSource)
			lastGravSource = ga.getNearestSource ();

        // read input from mouse or mobile controls
		float inputH = Input.GetAxis("Mouse X");
		float inputV = Input.GetAxis("Mouse Y");

        // with mouse input, we have direct control with no springback required.
		rotY += inputH*rotationSpeed;
        rotX += inputV*rotationSpeed;

		rotX = Mathf.Clamp(rotX, -90, 90);

		Vector3 lastRotation = head.transform.forward;

        // up/down
		head.transform.localRotation = Quaternion.Euler(-rotX, 0, 0);

		// propagate left-right rotation to parent

//		Quaternion rotation = Quaternion.Euler (0, rotY, 0);
//		Quaternion tilt = Quaternion.FromToRotation (Vector3.up, ga.getUpVector ());
//		Quaternion result = tilt * rotation;
//		transform.rotation = Quaternion.Slerp (transform.rotation, result, .9f); 
		//transform.rotation = Quaternion.LookRotation (, ga.getUpVector ());
		//transform.up = ga.getUpVector();
		//transform.rotation = Quaternion.AngleAxis(rotY, ga.getUpVector());
		//Debug.Log (transform.rotation + " " + transform.up);
//		transform.rotation *= Quaternion.FromToRotation(lastUpVector, ga.getUpVector());

		// left/right
		transform.Rotate(new Vector3(0, inputH*rotationSpeed, 0), Space.Self);
		transform.rotation = Quaternion.LookRotation(ga.getUpVector(), -transform.forward);
		transform.Rotate (Vector3.right, 90f);

		if (lastGravSource != ga.getNearestSource ()) {
			head.transform.LookAt(lastRotation*100000, transform.up);
			Vector3 projectedX = Vector3.ProjectOnPlane(head.transform.forward, Vector3.Cross(head.transform.forward, transform.up));
			float angleX = Vector3.Angle (projectedX, transform.up);

			rotX = -angleX + 90;

			Debug.Log ("rotX: " + rotX + " angle: " + angleX);
			lastGravSource = ga.getNearestSource ();
		}
		//lastUpVector = ga.getUpVector ();
    } 
}