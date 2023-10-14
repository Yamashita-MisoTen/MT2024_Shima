using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
	[Header("プレイヤーのプレハブを格納する")]
	[SerializeField] List<GameObject> pPlayer;
	[SerializeField] List<Vector3> StartPos;
	private int connectPlayerCount = 0;	// 接続人数
	override public void  OnServerAddPlayer(NetworkConnectionToClient conn){
		Debug.Log("プレイヤー生成するで");
		// 現在の接続人数を加算していく
		connectPlayerCount++;
		Debug.Log("プレイヤーの人数" + connectPlayerCount);

		GameObject prefab;
		if(pPlayer.Count < connectPlayerCount){
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
		DontDestroyOnLoad(player);	// 破壊不可オブジェクトとして生成する
		NetworkServer.AddPlayerForConnection(conn, player);
		// ゲームのマネージャーにデータを渡す
		GameRuleManager.instance.AddPlayerData(player);
	}

	override public void OnServerDisconnect(NetworkConnectionToClient conn){
		Debug.LogError("クライアントの接続切れたで");
		Debug.Log(conn);
		// 確認用にプレイヤーのデータを持ってきておく
		var allPlayer = GameRuleManager.instance.GetAllPlayerData();
		GameObject deleteObj = null;
		foreach(CPlayer p in allPlayer){
			if(p.connectionToClient.connectionId == conn.connectionId){
				deleteObj = p.gameObject;
				connectPlayerCount--;
				break;
			}
		}
		if(deleteObj != null){
			GameRuleManager.instance.RemovePlayerData(deleteObj);
		}
		base.OnServerDisconnect(conn);
	}

	public override void OnStopHost()
	{
		base.OnStopHost();
		GameRuleManager.instance.RemoveAllPlayerData();
		connectPlayerCount = 0;
	}
	public override void OnStopServer()
	{
		base.OnStopServer();
		GameRuleManager.instance.RemoveAllPlayerData();
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
