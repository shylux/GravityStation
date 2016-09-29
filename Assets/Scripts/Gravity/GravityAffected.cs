using UnityEngine;
using System.Collections;

public class GravityAffected : MonoBehaviour {
	GravityController controller;
	Rigidbody rigidbody;

	// Use this for initialization
	void Start () {
		rigidbody = GetComponent (typeof(Rigidbody)) as Rigidbody;
		rigidbody.useGravity = false;

		// register
		controller = FindObjectOfType(typeof(GravityController)) as GravityController;
		controller.rigidbodys.Add (rigidbody);
	}

	void FixedUpdate () {
		if (controller.sources.Count == 0)
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
		rigidbody.AddForce (force);
		this.transform.rotation = Quaternion.LookRotation (Vector3.down);//Quaternion.Euler (direction);
	}
}
