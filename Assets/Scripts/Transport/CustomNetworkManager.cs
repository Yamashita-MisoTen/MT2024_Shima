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
	[SerializeField] GameObject pPlayer;
	[SerializeField] List<Color> playerColor;
	[SerializeField] GameObject dataMgrPrefab;
	GameObject dataManagerObj;
	public NetWorkDataManager dataManager{get; private set;}
	private int connectPlayerCount = 0;	// 現在の接続人数

	override public void  OnServerAddPlayer(NetworkConnectionToClient conn){

		Debug.Log("プレイヤーを生成");
		// 現在の接続人数を加算
		connectPlayerCount++;
		Debug.Log("プレイヤーの人数 : " + connectPlayerCount);

		GameObject prefab;
		prefab = pPlayer;

		Transform startPos = GetStartPosition();
		GameObject player = startPos != null
			? Instantiate(prefab, startPos.position, startPos.rotation)
			: Instantiate(prefab);

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
		StopClient();
		StopServer();
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

	public Color GetPlayerColor(int num){
		return playerColor[num];
	}
}
