using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GravityAffected : NetworkBehaviour {
	GravityController controller;
	Rigidbody rigid;

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

		float nearestSourceDistance = float.MaxValue;
		GravitySource nearestSource = null;
		foreach (GravitySource source in controller.sources) {
			float distance = Vector3.Distance (this.transform.position, source.transform.position);
			if (distance < nearestSourceDistance) {
				nearestSourceDistance = distance;
				nearestSource = source;
			}
		}

		// apply gravity force
		Vector3 direction = (nearestSource.transform.position - this.transform.position).normalized;
		Vector3 force = direction * nearestSource.force;
		rigid.AddForce (force);
		Debug.DrawLine (transform.position, nearestSource.transform.position, Color.red);
		transform.up = -direction;

		//transform.rotation = Quaternion.LookRotation(-force);//Quaternion.Euler (direction);
		//transform.Rotate(Vector3.left, 90, Space.Self);
	}
}
