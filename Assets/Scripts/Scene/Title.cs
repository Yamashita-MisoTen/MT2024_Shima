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
	[SerializeField]GameRuleManager gameManager;

	[Serializable]
	public struct TitleSendData : NetworkMessage{
		public bool _isHostReady;
	}

	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space)){
			StartGame();
		}
	}

	[ClientRpc]
	void RpcChangeSceneMainGame(){
		// フェードの命令いれる
		manager.ServerChangeScene("MainGame");
	}

	[ServerCallback]
	void StartGame(){
		RpcChangeSceneMainGame();
	}
}
