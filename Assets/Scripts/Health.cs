using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

	public NetworkStartPosition[] respawnPoints;

	public RectTransform healthBar;

	public const int maxHealth = 100;
	public bool destroyOnDeath;

	[SyncVar(hook="OnChangeHealth")]
	public int currentHealth = maxHealth;

	public void Start() {
		if (!isLocalPlayer)
			return;
		respawnPoints = FindObjectsOfType<NetworkStartPosition> ();
	}

	public void takeDamage(int damage) {
		if (!isServer)
			return;

		currentHealth -= damage;
		if (currentHealth <= 0) {
			currentHealth = maxHealth;
			if (destroyOnDeath)
				Destroy (gameObject);
			else
				RpcRespawn ();
		}
	}
		
	void OnChangeHealth(int currentHealth) {
		healthBar.sizeDelta = new Vector2 (currentHealth, healthBar.sizeDelta.y);
	}

	[ClientRpc]
	void RpcRespawn() {
		if (isLocalPlayer) {
			Vector3 respawnPoint = Vector3.zero;
			if (respawnPoints != null && respawnPoints.Length > 0)
				respawnPoint = respawnPoints [Random.Range (0, respawnPoints.Length)].transform.position;	
			transform.position = respawnPoint;
		}
	}
}
