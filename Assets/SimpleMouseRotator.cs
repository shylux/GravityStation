using System;
using UnityEngine;
using UnityEngine.Networking;

public class SimpleMouseRotator: NetworkBehaviour {
    public float rotationSpeed = 10;
	public GameObject head;

    private Vector3 targetAngles;
    private Quaternion originalHeadRotation;
	private float rotY;

    private void Start() {
		originalHeadRotation = head.transform.localRotation;
//		lastUpVector = transform.up;
    }

//	Vector3 lastUpVector;
    private void FixedUpdate() {
		if (!isLocalPlayer)
			return;
        // we make initial calculations from the original local rotation
        //transform.localRotation = originalRotation;

        // read input from mouse or mobile controls
		float inputH = Input.GetAxis("Mouse X");
		float inputV = Input.GetAxis("Mouse Y");

        // with mouse input, we have direct control with no springback required.
		rotY += inputH*rotationSpeed;
        targetAngles.x += inputV*rotationSpeed;

		targetAngles.x = Mathf.Clamp(targetAngles.x, -90, 90);
      
        // update the actual gameobject's rotation
		head.transform.localRotation = originalHeadRotation*Quaternion.Euler(-targetAngles.x, 0, 0);

		// propagate left-right rotation to parent
		GravityAffected ga = GetComponent<GravityAffected>() as GravityAffected;

//		Quaternion rotation = Quaternion.Euler (0, rotY, 0);
//		Quaternion tilt = Quaternion.FromToRotation (Vector3.up, ga.getUpVector ());
//		Quaternion result = tilt * rotation;
//		transform.rotation = Quaternion.Slerp (transform.rotation, result, .9f); 
		//transform.rotation = Quaternion.LookRotation (, ga.getUpVector ());
		//transform.up = ga.getUpVector();
		//transform.rotation = Quaternion.AngleAxis(rotY, ga.getUpVector());
		//Debug.Log (transform.rotation + " " + transform.up);
//		transform.rotation *= Quaternion.FromToRotation(lastUpVector, ga.getUpVector());
		transform.Rotate(new Vector3(0, inputH*rotationSpeed, 0), Space.Self);
		transform.rotation = Quaternion.LookRotation(ga.getUpVector(), -transform.forward);
		transform.Rotate (Vector3.right, 90f);

		//lastUpVector = ga.getUpVector ();
    } 
}