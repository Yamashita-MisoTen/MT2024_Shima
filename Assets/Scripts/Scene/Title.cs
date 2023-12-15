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
		public int connectNum;
	}

	[SerializeField] GameObject fadeObj;
	FadeMgr fadeMgr;
	Ui_Move uimove;
	public int connectNum;
	void Awake(){
		NetworkClient.RegisterHandler<TitleSendData>(ReciveTitleDataInfo);
	}

	void Start()
	{
		if(netMgr == null) netMgr = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
		var obj = Instantiate(fadeObj);
		fadeMgr = obj.GetComponent<FadeMgr>();
		fadeMgr.SetRenderCamera(GameObject.Find("Main Camera").GetComponent<Camera>());
		netMgr.PlayerDataInit();
		uimove = GameObject.Find("TitleCanvas").GetComponent<Ui_Move>();

		obj = Instantiate(Soundprefab);
		obj = Instantiate(BGMSoungprefab);
	}

	// Update is called once per frame
	void Update()
	{
		if (NetworkServer.active && NetworkClient.isConnected)
		{
			Debug.Log("�T�[�o�[���");
		}
		else if (NetworkClient.isConnected)
		{
			Debug.Log("�N���C�A���g���");
		}
	}

	[ClientRpc]
	void RpcChangeSceneMainGame(string sceneName){
		if(!fadeMgr.isFadeOutNow){
			fadeMgr.StartFadeOut();
			// �t�F�[�h�̖��߂����
			DOVirtual.DelayedCall(fadeMgr.fadeOutTime,() =>netMgr.ServerChangeScene(sceneName));
		}
	}
	[ServerCallback]
	public void ConnectUpdate(int num){
		var send = new TitleSendData{connectNum = num};
		NetworkServer.SendToAll(send);
	}
	[ClientRpc]
	public void RpcConnectUpdate(int num){
		connectNum = num;
	}

	[ServerCallback]
	public void StartGame(){
		RpcChangeSceneMainGame("MainGame");
	}

	private void ReciveTitleDataInfo(TitleSendData receivedData)
	{
		//���[�J���̃t���O�ɔ��f
		connectNum = receivedData.connectNum;
		Debug.Log("��������" + connectNum);
	}
}
