using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GravityAffected : NetworkBehaviour {
	GravityController controller;
	Rigidbody rigid;

	public bool applyGravity = true;

	// Use this for initialization
	void Start () {
		if (!isLocalPlayer && this.tag == "Player")
			return;
		rigid = GetComponent (typeof(Rigidbody)) as Rigidbody;
		rigid.useGravity = false;

		// register
		controller = FindObjectOfType(typeof(GravityController)) as GravityController;
		controller.rigidbodys.Add (rigid);
	}

	void FixedUpdate () {
		if ((!isLocalPlayer && this.tag == "Player") || controller.sources.Count == 0)
			return;

		if (!applyGravity)
			return;

		// apply gravity force
		Vector3 cummulativeForce = Vector3.zero;
		foreach (GravitySource source in controller.sources) {
			float distance = Vector3.Distance (this.transform.position, source.transform.position);
			if (distance > source.radius * controller.gravityPropagation)
				continue;
			Vector3 forceDirection = (this.transform.position - source.transform.position).normalized;
			float appliedFraction = 1; // push out if object is inside of planet.
			if (distance > source.radius) {
				// This calculates a fraction (0..1) to be applied based on the distance between the object and the planet surface.
				// The fraction is 1 at the surface and 0 at the outer most point of influence defined by radius and gravityPropagation.
				appliedFraction = ((distance / source.radius) - 1) / (controller.gravityPropagation - 1) - 1;
			}
			//Debug.Log ("Added " + appliedFraction * source.force);

			cummulativeForce += forceDirection * appliedFraction * source.force;
		}
			
		rigid.AddForce (cummulativeForce);
	}


	// Calculates the nearest gravity source. Includes planet size into the calculation (but not configured gravity).
	public GravitySource getNearestSource() {
		float nearestRelativeDistance = float.MaxValue;
		GravitySource nearestSource = null;

		foreach (GravitySource source in controller.sources) {
			float distance = Vector3.Distance (this.transform.position, source.transform.position);
			float relativeDistance = distance / source.radius;
			if (relativeDistance < nearestRelativeDistance) {
				nearestRelativeDistance = relativeDistance;
				nearestSource = source;
			}
		}
		return nearestSource;
	}

	public Vector3 getUpVector() {
		GravitySource nearestSource = getNearestSource ();
		Vector3 direction = (nearestSource.transform.position - this.transform.position).normalized;

		if (Vector3.Distance (nearestSource.transform.position, this.transform.position) / nearestSource.radius < 1)
			direction = -direction;

		Debug.DrawLine (transform.position, nearestSource.transform.position, Color.red);
		return -direction;
	}
}
