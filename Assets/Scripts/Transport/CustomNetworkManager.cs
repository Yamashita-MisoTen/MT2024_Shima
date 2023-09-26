using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
	[Header("プレイヤーのプレハブを格納する")]
	[SerializeField] List<GameObject>pPlayer;
	private int connectPlayerCount = 0;	// 接続人数
	override public void  OnServerAddPlayer(NetworkConnectionToClient conn){
		Debug.Log("プレイヤー生成するで");
		// 現在の接続人数を加算していく
		connectPlayerCount++;

		GameObject prefab;
		if(pPlayer[connectPlayerCount - 1] == null){
			prefab = pPlayer[0];
		}else{
			prefab = pPlayer[connectPlayerCount - 1];
		}
		Debug.Log("プレイヤーを生成する");
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(prefab, startPos.position, startPos.rotation)
            : Instantiate(prefab);

		player.name = $"{prefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);
		// ゲームのマネージャーにデータを渡す
		GameRuleManager.instance.AddPlayerData(player);
	}

}
