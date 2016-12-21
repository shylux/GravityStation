using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public float start_force;
	public float propelled_force;
	public float is_aoe;
	public int damage;
	public float damage_range;
	public GunBehaviour gun_origin;
	private Vector3 initial_dir;
	private bool _isactive;
	private MeshRenderer mesh_renderer;
	private Rigidbody bullet_rigid;


	void Start(){
		this.transform = gun_origin.transform;
		mesh_renderer = this.GetComponent("MeshRenderer");
		bullet_rigid = this.GetComponent ("Rigidbody");


	}

	void OnCollisionEnter(Collision collision) {
		var hit = collision.gameObject;
		var health = hit.GetComponent<Health> ();
		if (health != null) {
			health.takeDamage (10);
		}
		this._isactive = false;
		mesh_renderer.enabled (false);
	}

	void Fire() {
		initial_dir = gun_origin.transform.eulerAngles.normalized();
		bullet_rigid.AddForce (start_force * initial_dir);
	}

	void FixedUpdate() {
		if (propelled_force > 0) {
			bullet_rigid.AddRelativeForce (Vector3 (0, propelled_force, 0));					
	}
}
	void reset(){
		this.transform.position = gun_origin.transform.position();
			mesh_renderer.enabled(true);
	}
}