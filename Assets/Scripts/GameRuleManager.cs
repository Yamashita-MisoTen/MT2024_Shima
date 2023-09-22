using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class GameRuleManager : NetworkBehaviour
{
	// ** シングルトン定義
	public static GameRuleManager instance;
	private void Awake(){
		if(instance == null){
			instance = this;
			DontDestroyOnLoad(gameObject);
		}else{
			Destroy(gameObject);
		}
	}

	// ** 変数定義

	[SerializeField]
	[Header("ゲームの制限時間(秒)")]int _LimitTime = 300;	// 制限時間
	float _progressTime;	// 経過時間
	bool _isFinishGame = false;
	enum PlayerID{
		player01,
		player02,
		player03,
		player04,
	}
	PlayerID _orgaPlayer = PlayerID.player01;
	List<CPlayer> _playerData;	// プレイヤーのデータを格納しておく
	void Start()
	{
		_playerData = new List<CPlayer>(0);
		Debug.Log("生成");
		StartGame();
	}

	// Update is called once per frame
	void Update()
	{
		if(_LimitTime < _progressTime) FinishGame();	// 制限時間を過ぎたらゲーム終了する
		_progressTime += Time.deltaTime;				// 経過時間を更新する
	}

	void StartGame(){
		int num = UnityEngine.Random.Range(0,4);
		Debug.Log(num);
		Debug.Log((PlayerID)num);
	}

	void FinishGame(){	// ゲーム終了時に呼び出される関数
		if(_isFinishGame) return;
		Debug.Log("ゲームを終了します");
		_isFinishGame = true;

		// 接続しているプレイヤーすべてに終了命令を送る
	}

	bool CheckIdentityPlayer(int plNum, CPlayer player){
		if(_playerData[plNum] == player) return true;
		return false;
	}

	// 鬼が変わるタイミングで呼ぶ関数
	void ChangeOrgaPlayer(CPlayer player){
		for(int i = 0; i < _playerData.Count(); i++){
			// オブジェクトが同一かどうか確認する
			if(CheckIdentityPlayer(i, player)){
				_orgaPlayer = (PlayerID)i;
				continue;
			}
		}
	}

	public void AddPlayerData(CPlayer player){
		if(_playerData.Count() > 4){
			Debug.LogError("人数制限を超えようとしています");
			return;
		}

		// 同一のデータがある場合は追加しない
		for(int i = 0; i < _playerData.Count(); i++){
			if(_playerData[i] == player) return;
		}
		_playerData.Add(player);	// プレイヤーのデータを格納する
		Debug.Log("プレイヤーを追加しました現在のプレイヤー数" + _playerData.Count());
	}

}
