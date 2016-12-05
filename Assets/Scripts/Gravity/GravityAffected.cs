using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GravityAffected : NetworkBehaviour {
	GravityController controller;
	Rigidbody rigid;

	public bool applyGravity = true;

	// Use this for initialization
	void Start () {
		if (!isLocalPlayer)
			return;
		rigid = GetComponent (typeof(Rigidbody)) as Rigidbody;
		rigid.useGravity = false;

		// register
		controller = FindObjectOfType(typeof(GravityController)) as GravityController;
		controller.rigidbodys.Add (rigid);
	}

	void FixedUpdate () {
		if (!isLocalPlayer || controller.sources.Count == 0)
			return;

		// apply gravity force
		GravitySource nearestSource = getNearestSource();
		Vector3 upVector = getUpVector();
		Vector3 force = -upVector * nearestSource.force;
		if (applyGravity)
			rigid.AddForce (force);
	}

	public GravitySource getNearestSource() {
		float nearestSourceDistance = float.MaxValue;
		GravitySource nearestSource = null;
		foreach (GravitySource source in controller.sources) {
			float distance = Vector3.Distance (this.transform.position, source.transform.position);
			if (distance < nearestSourceDistance) {
				nearestSourceDistance = distance;
				nearestSource = source;
			}
		}
		return nearestSource;
	}

	public Vector3 getUpVector() {
		GravitySource nearestSource = getNearestSource ();
		Vector3 direction = (nearestSource.transform.position - this.transform.position).normalized;

		Debug.DrawLine (transform.position, nearestSource.transform.position, Color.red);
		return -direction;
	}
}
