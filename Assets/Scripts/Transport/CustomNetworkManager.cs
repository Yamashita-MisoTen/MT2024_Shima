using System.Collections;
using System.Collections.Generic;
using System.Data;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
	[Header("�v���C���[���g�p����v���n�u")]
	[SerializeField] List<GameObject> pPlayer;
	[SerializeField] List<Vector3> StartPos;
	[SerializeField] GameRuleManager mgr;
	private int connectPlayerCount = 0;	// ���݂̐ڑ��l��
	override public void  OnServerAddPlayer(NetworkConnectionToClient conn){
		// if(SceneManager.GetActiveScene().name != "Title"){
		// 	Debug.Log("�^�C�g���V�[���ȊO�ő����悤�Ƃ��Ă�");
		// 	return;
		// }
		Debug.Log("�v���C���[�𐶐�");
		// ���݂̐ڑ��l�������Z
		connectPlayerCount++;
		Debug.Log("�v���C���[�̐l�� : " + connectPlayerCount);

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
		DontDestroyOnLoad(player);	// �V�[���J�ڗp�Ƀv���C���[�f�[�^���c�����܂܂ɂ��Ă���
		NetworkServer.AddPlayerForConnection(conn, player);
		mgr = GameObject.Find("Pf_GameRuleManager").GetComponent<GameRuleManager>();
		// �}�l�[�W���[�Ƀf�[�^��ۊǂ���
		mgr.AddPlayerData(player);
	}

	public override void OnClientConnect()
	{
		base.OnClientConnect();

		print("�N���C�A���g�Őڑ�?");
	}

	override public void OnServerDisconnect(NetworkConnectionToClient conn){
		Debug.LogError("�N���C�A���g�̐ڑ��؂�܂���");
		Debug.Log(conn);

		// �v���C���[�̃f�[�^���m�F����
		var allPlayer = mgr.GetAllPlayerData();
		GameObject deleteObj = null;
		foreach(CPlayer p in allPlayer){
			if(p.connectionToClient.connectionId == conn.connectionId){
				deleteObj = p.gameObject;
				connectPlayerCount--;
				break;
			}
		}
		if(deleteObj != null){
			mgr.RemovePlayerData(deleteObj);
		}
		if(mgr != null){
			NetworkServer.Destroy(mgr.gameObject);
		}
		base.OnServerDisconnect(conn);
	}

	public override void OnClientDisconnect()
	{
		Debug.LogError("�ڑ��؂�܂���");

		if(mgr != null){
			mgr.RemoveAllPlayerData();
			NetworkServer.Destroy(mgr.gameObject);
		}

		base.OnClientDisconnect();
	}

	public override void OnStopHost()
	{
		base.OnStopHost();
		if(mgr != null){
			mgr.RemoveAllPlayerData();
		}
		connectPlayerCount = 0;
	}
	public override void OnStopServer()
	{
		base.OnStopServer();
		if(mgr != null){
			mgr.RemoveAllPlayerData();
		}
		connectPlayerCount = 0;
	}

	public override void OnStopClient(){
		base.OnStopClient();
		if(mgr != null){
			mgr.RemoveAllPlayerData();
		}
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
