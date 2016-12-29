using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour {

	public GameObject enemyPrefab;
	public int numberOfEnemies;

	public override void OnStartServer() {
		for (int i = 0; i < numberOfEnemies; i++) {
			var spawnPosition = new Vector3 (Random.Range (-8f, 8f), 0, Random.Range (-8f, 8f));
			var spawnRotation = Quaternion.Euler (0, Random.Range (0, 360), 0);
			var enemy = (GameObject)Instantiate (enemyPrefab, spawnPosition, spawnRotation);
			NetworkServer.Spawn (enemy);
		}
	}
}
