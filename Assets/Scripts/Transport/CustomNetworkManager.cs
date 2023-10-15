using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
	[Header("プレイヤーが使用するプレハブ")]
	[SerializeField] List<GameObject> pPlayer;
	[SerializeField] List<Vector3> StartPos;
	GameRuleManager mgr;
	private int connectPlayerCount = 0;	// 現在の接続人数
	override public void  OnServerAddPlayer(NetworkConnectionToClient conn){
		// if(SceneManager.GetActiveScene().name != "Title"){
		// 	Debug.Log("タイトルシーン以外で増えようとしてる");
		// 	return;
		// }
		Debug.Log("プレイヤーを生成");
		// 現在の接続人数を加算
		connectPlayerCount++;
		Debug.Log("プレイヤーの人数 : " + connectPlayerCount);

		GameObject prefab;
		if(pPlayer.Count < connectPlayerCount){
			prefab = pPlayer[0];
		}else{
			prefab = pPlayer[connectPlayerCount - 1];
		}
		Transform startPos = GetStartPosition();
		GameObject player = startPos != null
			? Instantiate(prefab, startPos.position, startPos.rotation)
			: Instantiate(prefab);

		player.name = $"{prefab.name} [connId={conn.connectionId}]";
		DontDestroyOnLoad(player);	// シーン遷移用にプレイヤーデータを残したままにしておく
		NetworkServer.AddPlayerForConnection(conn, player);
		mgr = GameObject.Find("Pf_GameRuleManager").GetComponent<GameRuleManager>();
		// マネージャーにデータを保管する
		mgr.AddPlayerData(player);
	}

	override public void OnServerDisconnect(NetworkConnectionToClient conn){
		Debug.LogError("クライアントの接続切れました");
		Debug.Log(conn);

		// プレイヤーのデータを確認する
		var allPlayer = mgr.GetAllPlayerData();
		GameObject deleteObj = null;
		foreach(CPlayer p in allPlayer){
			if(p.connectionToClient.connectionId == conn.connectionId){
				deleteObj = p.gameObject;
				connectPlayerCount--;
				break;
			}
		}
		if(deleteObj != null){
			mgr.RemovePlayerData(deleteObj);
		}
		base.OnServerDisconnect(conn);
	}

	public override void OnStopHost()
	{
		base.OnStopHost();
		mgr.RemoveAllPlayerData();
		connectPlayerCount = 0;
	}
	public override void OnStopServer()
	{
		base.OnStopServer();
		mgr.RemoveAllPlayerData();
		connectPlayerCount = 0;
	}

	public override void OnClientError(TransportError error, string reason){
		base.OnClientError(error,reason);
		print("OnClientError : "+reason);
	}

	public Transform GetStartPosition(int num){
		Transform result = GetStartPosition();
		if(StartPos.Count < num){
			result.position = StartPos[0];
		}else{
			result.position = StartPos[num];
		}
		return result;
	}

	private void AddPlayerCharacter(){
		
	}

}
