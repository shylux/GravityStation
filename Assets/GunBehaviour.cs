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
	public void Fire () {
			Debug.Log ("blubb");
			if (bullets_waiting.Count > 0) {
				Bullet my_bullet = (Bullet) bullets [0];
				bullets_waiting.Remove (my_bullet);
				my_bullet.reset ();
				bullets.Add (my_bullet);
				my_bullet.Fire (this.transform);
			} else {
				GameObject bulletgo = (GameObject)Instantiate (
							BulletPrefab,
							this.transform.position,
							this.transform.rotation
				);
				Bullet bullet = (Bullet)bulletgo.GetComponent ("Bullet");
				bullets.Add (bullet);
				bullet.Fire(this.transform);

		}
	}
}
