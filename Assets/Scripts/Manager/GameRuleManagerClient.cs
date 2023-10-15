using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameRuleManagerClient : NetworkBehaviour
{
	GameRuleManager mgr;

	// ** プレイヤー関係
	List<CPlayer> _playerData;	// プレイヤーのデータを格納しておく
	CPlayer _orgaPlayer;		// 現在鬼のプレイヤーを格納しておく

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
		Debug.Log("クライアントにデータを送信しました 人数 : " + _playerData.Count);
	}
}
