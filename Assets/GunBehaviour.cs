using UnityEngine;
using System.Collections;

public class GunBehaviour : MonoBehaviour {
	public GameObject BulletPrefab;
	public float magazine_size;

	private ArrayList bullets;
	private ArrayList bullets_waiting;
	// Use this for initialization
	void Start () {
		bullets = new ArrayList ();
		bullets_waiting = new ArrayList ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Input.GetKeyDown(KeyCode.Mouse1)){
			if (bullets_waiting.Count > 0){
				Bullet my_bullet = bullets[0];
				bullets_waiting.Remove (my_bullet);
				my_bullet.reset ();
				bullets.Add (my_bullet);
			}
		}
	}
}
