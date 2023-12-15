using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class Title : NetworkBehaviour{
	// Start is called before the first frame update
	[SerializeField]CustomNetworkManager netMgr;
	[SerializeField] GameObject Soundprefab;
	[SerializeField] GameObject BGMSoungprefab;

	[Serializable]
	public struct TitleSendData : NetworkMessage{
		public bool _isHostReady;
	}

	[SerializeField] GameObject fadeObj;
	FadeMgr fadeMgr;
	void Awake(){

	}

	void Start()
	{
		if(netMgr == null) netMgr = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
		var obj = Instantiate(fadeObj);
		fadeMgr = obj.GetComponent<FadeMgr>();
		fadeMgr.SetRenderCamera(GameObject.Find("Main Camera").GetComponent<Camera>());
		netMgr.PlayerDataInit();

		obj = Instantiate(Soundprefab);
		obj = Instantiate(BGMSoungprefab);
	}

	// Update is called once per frame
	void Update()
	{

	}

	[ClientRpc]
	void RpcChangeSceneMainGame(string sceneName){
		if(!fadeMgr.isFadeOutNow){
			fadeMgr.StartFadeOut();
			// フェードの命令いれる
			DOVirtual.DelayedCall(fadeMgr.fadeOutTime,() =>netMgr.ServerChangeScene(sceneName));
		}
	}

	[ServerCallback]
	public void StartGame(){
		RpcChangeSceneMainGame("MainGame");
	}
}
