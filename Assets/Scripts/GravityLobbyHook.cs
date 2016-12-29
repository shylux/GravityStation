using UnityEngine;
using System.Collections;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class GravityLobbyHook : LobbyHook {

	public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) {
		LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer> ();
		GravityPlayerController player = gamePlayer.GetComponent<GravityPlayerController> ();

		player.color = lobby.playerColor;
		player.username = lobby.playerName;
		Debug.Log (player.name);
	}
}
