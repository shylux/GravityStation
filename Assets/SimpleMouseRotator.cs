using System;
using UnityEngine;

public class SimpleMouseRotator: MonoBehaviour {
    public float rotationSpeed = 10;
    
    private Vector3 targetAngles;
    private Quaternion originalRotation;

    private void Start() {
        originalRotation = transform.localRotation;
    }

    private void Update()
    {
        // we make initial calculations from the original local rotation
        transform.localRotation = originalRotation;

        // read input from mouse or mobile controls
		float inputH = Input.GetAxis("Mouse X");
		float inputV = Input.GetAxis("Mouse Y");              

        // with mouse input, we have direct control with no springback required.
        targetAngles.y += inputH*rotationSpeed;
        targetAngles.x += inputV*rotationSpeed;

		targetAngles.x = Mathf.Clamp(targetAngles.x, -90, 90);
      

        // update the actual gameobject's rotation
		transform.localRotation = originalRotation*Quaternion.Euler(-targetAngles.x, targetAngles.y, 0);
    }
}