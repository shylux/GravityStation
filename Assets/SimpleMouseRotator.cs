using System;
using UnityEngine;
using UnityEngine.Networking;

public class SimpleMouseRotator: NetworkBehaviour {
    public float rotationSpeed = 10;
	public GameObject head;

    private Vector3 targetAngles;
    private Quaternion originalRotation;

    private void Start() {
		originalRotation = head.transform.localRotation;
    }

    private void FixedUpdate() {
		if (!isLocalPlayer)
			return;
        // we make initial calculations from the original local rotation
        //transform.localRotation = originalRotation;

        // read input from mouse or mobile controls
		float inputH = Input.GetAxis("Mouse X");
		float inputV = Input.GetAxis("Mouse Y");              

        // with mouse input, we have direct control with no springback required.
        targetAngles.y += inputH*rotationSpeed;
        targetAngles.x += inputV*rotationSpeed;

		targetAngles.x = Mathf.Clamp(targetAngles.x, -90, 90);
      
        // update the actual gameobject's rotation
		head.transform.localRotation = originalRotation*Quaternion.Euler(-targetAngles.x, 0, 0);

		// propagate left-right rotation to parent
		transform.localRotation = transform.localRotation*Quaternion.Euler(0, targetAngles.y, 0);
    }
}