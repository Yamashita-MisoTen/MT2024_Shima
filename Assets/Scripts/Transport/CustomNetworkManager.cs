using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Examples.Basic;
using Telepathy;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
	[Header("プレイヤーが使用するプレハブ")]
	[SerializeField] List<GameObject> pPlayer;
	[SerializeField] List<Vector3> StartPos;
	[SerializeField] GameObject dataMgrPrefab;
	[SerializeField] GameObject dataManagerObj;
	[SerializeField,Header("ああああ")]NetWorkDataManager dataManager;
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

		if(dataManager == null){
			dataManagerObj = Instantiate(dataMgrPrefab);
			dataManager = dataManagerObj.GetComponent<NetWorkDataManager>();
			NetworkServer.Spawn(dataManagerObj.gameObject);
		}

		var pComp = player.GetComponent<CPlayer>();
		var connid = pComp.connectionToClient.connectionId;
		dataManager.SetPlayerData(player,pComp,connid);

		// mgr = GameObject.Find("Pf_GameRuleManager").GetComponent<GameRuleManager>();
		// // マネージャーにデータを保管する
		// mgr.AddPlayerData(player);
	}

	override public void OnServerDisconnect(NetworkConnectionToClient conn){
		Debug.LogError("クライアントの接続切れました");
		Debug.Log(conn);

		// プレイヤーのデータを確認する
		if(dataManager.CheckIdentity(conn.connectionId)){
			dataManager.DeleteObj(conn.connectionId);
			connectPlayerCount--;
		}
		base.OnServerDisconnect(conn);
	}

	public override void OnStopHost()
	{
		base.OnStopHost();
		dataManager.DeleteAllObj();
		connectPlayerCount = 0;
	}
	public override void OnStopServer()
	{
		base.OnStopServer();
		dataManager.DeleteAllObj();
		connectPlayerCount = 0;
	}

	public override void OnClientError(TransportError error, string reason){
		base.OnClientError(error,reason);
		print("OnClientError : "+reason);
	}

	public List<GameObject> GetPlayerDatas(List<GameObject> obj){
		if(dataManager == null){
			dataManager = Instantiate(dataMgrPrefab).GetComponent<NetWorkDataManager>();
			dataManager.CmdSetPlayerData();
			Debug.Log(dataManager.GetPlayerData(obj));
		}
		return dataManager.GetPlayerData(obj);
	}
	public List<CPlayer> GetPlayerDatas(List<CPlayer> obj){
		if(dataManager == null){
			//dataManager = Instantiate(dataMgrPrefab).GetComponent<NetWorkDataManager>();
			//dataManager.CmdSetPlayerData();
			Debug.Log(dataManager.GetPlayerData(obj));
		}
		return dataManager.GetPlayerData(obj);
	}
	public List<int> GetPlayerDatas(List<int> obj){
		if(dataManager == null){
			dataManager = Instantiate(dataMgrPrefab).GetComponent<NetWorkDataManager>();
			dataManager.CmdSetPlayerData();
			Debug.Log(dataManager.GetPlayerData(obj));
		}
		return dataManager.GetPlayerData(obj);
	}

	public void SetDataMgr(NetWorkDataManager mgr){
		dataManager = mgr;
	}

	public void PlayerDataInit(){
		if(dataManagerObj == null) return;
		dataManager.PlayerDataInit();
	}
}
