using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
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
		var obj = GameObject.Find("Pf_GameRuleManager");
		if(obj == null){
			Debug.Log("マネージャーないから追加する");
			Instantiate(gameManager);
		}
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
