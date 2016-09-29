using UnityEngine;
using System.Collections;

public class GravitySource : MonoBehaviour {

	public float force = 9.81f;

	void Start() {
		// register
		GravityController controller = FindObjectOfType(typeof(GravityController)) as GravityController;
		controller.sources.Add (this);
	}

}
