using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public float start_force;
	public float propelled_force;
	public bool is_aoe;
	public int damage;
	public float damage_range;
	private Vector3 initial_dir;
	private bool _isactive;
	private MeshRenderer mesh_renderer;
	private Rigidbody bullet_rigid;

	void OnCollisionEnter(Collision collision) {
		
		var hit = collision.gameObject;
		if (is_aoe == true) {
			var players = GameObject.FindGameObjectsWithTag ("Player");
			foreach (GameObject player in players) {
				RaycastHit ray_hit;
				var rayDirection = player.transform.position - transform.position;
				Physics.Raycast (this.transform.position, rayDirection, out ray_hit, damage_range);
				if (ray_hit.collider.gameObject == player) {
					var health = player.GetComponent<Health> ();
					health.takeDamage (damage);
				}
			}
		} else {
			var health = hit.GetComponent<Health> ();
			if (health != null) {
				health.takeDamage (damage);
			}
		}
		this._isactive = false;
		mesh_renderer = this.GetComponent<MeshRenderer>();
		mesh_renderer.enabled = false;
	}

	public void Fire(Transform gun_transform) {
		this.transform.position = gun_transform.position;
		initial_dir = gun_transform.forward.normalized;
		bullet_rigid = this.GetComponent<Rigidbody>();
		Debug.Log (initial_dir);
		bullet_rigid.AddForce (start_force * initial_dir);
	}

	void FixedUpdate() {
		if (this._isactive == false) {
			return;
		}
		if (propelled_force > 0) {
			bullet_rigid.AddRelativeForce (new Vector3 (0, propelled_force, 0));					
	}
}
	public void reset(){
		this._isactive = true;
		mesh_renderer = this.GetComponent<MeshRenderer>();
		mesh_renderer.enabled = true;
	}
}