using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameRuleManagerClient : NetworkBehaviour
{
	GameRuleManager mgr;

	// ** �v���C���[�֌W
	List<CPlayer> _playerData;	// �v���C���[�̃f�[�^���i�[���Ă���
	CPlayer _orgaPlayer;		// ���݋S�̃v���C���[���i�[���Ă���

	// Start is called before the first frame update
	void Start()
	{
		mgr = GameObject.Find("Pf_GameRuleManager").GetComponent<GameRuleManager>();
		DontDestroyOnLoad(this);
	}

	// Update is called once per frame
	void Update()
	{

	}

	[ClientRpc]
	void RpcOrgaData(){

	}

	[ClientRpc]
	public void RpcSetPlayerData(List<CPlayer> playersData){
		_playerData = playersData;
		Debug.Log("�N���C�A���g�Ƀf�[�^�𑗐M���܂��� �l�� : " + _playerData.Count);
	}
}
