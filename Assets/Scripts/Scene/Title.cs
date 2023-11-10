using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class Title : NetworkBehaviour{
	// Start is called before the first frame update
	[SerializeField]NetworkManager manager;
	[SerializeField]GameObject gameManager;

	[Serializable]
	public struct TitleSendData : NetworkMessage{
		public bool _isHostReady;
	}
	void Awake(){

	}

	void Start()
	{
		CustomNetworkManager netMgr = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
		netMgr.PlayerDataInit();
	}

	// Update is called once per frame
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space)){
			StartGame();
		}
	}

	[ClientRpc]
	void RpcChangeSceneMainGame(string sceneName){
		// フェードの命令いれる
		manager.ServerChangeScene(sceneName);
	}

	[ServerCallback]
	void StartGame(){
		RpcChangeSceneMainGame("MainGame");
	}
}
