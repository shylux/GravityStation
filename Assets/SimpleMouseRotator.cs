using System;
using UnityEngine;
using UnityEngine.Networking;

public class SimpleMouseRotator: NetworkBehaviour {
    public float rotationSpeed = 10;
	public GameObject head;

	private float rotX;
	private GravitySource lastGravSource;

    private void Start() {
    }

    private void FixedUpdate() {
		if (!isLocalPlayer)
			return;

		GravityAffected ga = GetComponent<GravityAffected>() as GravityAffected;

		if (!lastGravSource)
			lastGravSource = ga.getNearestSource ();

        // read input from mouse or mobile controls
		float inputH = Input.GetAxis("Mouse X");
		float inputV = Input.GetAxis("Mouse Y");

        // with mouse input, we have direct control with no springback required.
        rotX += inputV*rotationSpeed;
		rotX = Mathf.Clamp(rotX, -90, 90);

		Vector3 lastRotation = head.transform.forward;

        // up/down
		head.transform.localRotation = Quaternion.Euler(-rotX, 0, 0);

		// left/right
		transform.Rotate(new Vector3(0, inputH*rotationSpeed, 0), Space.Self);
		transform.rotation = Quaternion.LookRotation(ga.getUpVector(), -transform.forward);
		transform.Rotate (Vector3.right, 90f);

		Debug.DrawLine (head.transform.position, head.transform.position + head.transform.forward * 20);

		// Handle up vector change
		if (lastGravSource != ga.getNearestSource ()) {
			// set look at with new up vector to calculate new X rotation (up/down)
			head.transform.LookAt(lastRotation*100000, ga.getUpVector());
			// project new look vector to X plane and read/set angle
			Vector3 projectedX = Vector3.ProjectOnPlane(head.transform.forward, Vector3.Cross(head.transform.forward, transform.up));
			float angleX = Vector3.Angle (projectedX, transform.up);
			rotX = -angleX + 90;
			head.transform.localRotation = Quaternion.Euler(-rotX, 0, 0);

			transform.rotation = Quaternion.LookRotation(ga.getUpVector(), -lastRotation*100000);
			transform.Rotate (Vector3.right, 90f);

			lastGravSource = ga.getNearestSource ();
		}
    } 
}