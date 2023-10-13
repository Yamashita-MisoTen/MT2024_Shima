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
		// �N�����ɃN���C�A���g���Ƀf�[�^�𑗐M����R�[���o�b�N�֐����`���Ă���
		NetworkClient.RegisterHandler<TitleSendData>(ReceivedReadyInfo);
	}

	/// <summary>
	/// �T�[�o�[����󂯎�����f�[�^���e�N���C�A���g�Ŏg��
	/// </summary>
	/// <param name="conn">�R�l�N�V�������@�֐����Ŏg���ĂȂ����ǕK�v�݂���</param>
	/// <param name="receivedData">�󂯎�����f�[�^</param>
	private void ReceivedReadyInfo(TitleSendData receivedData)
	{
		if(receivedData._isHostReady){
			Debug.Log("�V�[���J�ڂ̖���");
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
		// �t�F�[�h�̖��߂����
		manager.ServerChangeScene("MainGame");
	}
}
