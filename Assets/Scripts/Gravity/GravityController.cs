using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GravityController : MonoBehaviour {

	public List<Rigidbody> rigidbodys = new List<Rigidbody> ();
	public List<GravitySource> sources = new List<GravitySource> ();

	public void register(GravitySource source) {
		sources.Add (source);
	}
}
