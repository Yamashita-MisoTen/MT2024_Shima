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

	void Awake(){
		// 起動時にクライアント側にデータを送信するコールバック関数を定義しておく
		NetworkClient.RegisterHandler<TitleSendData>(ReceivedReadyInfo);
	}

	/// <summary>
	/// サーバーから受け取ったデータを各クライアントで使う
	/// </summary>
	/// <param name="conn">コネクション情報　関数内で使ってないけど必要みたい</param>
	/// <param name="receivedData">受け取ったデータ</param>
	private void ReceivedReadyInfo(TitleSendData receivedData)
	{
		if(receivedData._isHostReady){
			Debug.Log("シーン遷移の命令");
			ChangeSceneMainGame();
		}
	}
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space)){
			TitleSendData sendData = new TitleSendData() {_isHostReady = true};
			NetworkServer.SendToAll(sendData);
		}
	}

	void ChangeSceneMainGame(){
		// フェードの命令いれる
		manager.ServerChangeScene("MainGame");
	}
}
