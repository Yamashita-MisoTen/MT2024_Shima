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

	//// **** 変数定義 **** ////

	// ** 時間関係
	// ゲームの制限時間
	[SerializeField] [Header("ゲームの制限時間(秒)")]int _LimitTime = 300;
	float _progressLimitTime;	// 経過時間
	// 鬼のタッチ後のクールタイム
	[SerializeField]
	[Header("鬼変更後のクールタイム")]int _changeCoolTime = 5;
	float _progressCoolTime;	// 経過時間
	bool _isCoolTimeNow = false;

	// ** プレイヤー関係
	List<CPlayer> _playerData;	// プレイヤーのデータを格納しておく
	CPlayer _orgaPlayer;		// 現在鬼のプレイヤーを格納しておく

	bool _isFinishGame = false;
	void Start()
	{
		_playerData = new List<CPlayer>(0);
		Debug.Log("生成");
		StartGame();
	}

	// Update is called once per frame
	void Update()
	{
		if(_LimitTime < _progressLimitTime) FinishGame();	// 制限時間を過ぎたらゲーム終了する
		_progressLimitTime += Time.deltaTime;				// 経過時間を更新する

		// タッチのクールタイムの設定
		if(_isCoolTimeNow){
			if(_changeCoolTime < _progressCoolTime){
				_progressCoolTime += Time.deltaTime;
			}else{
				_isCoolTimeNow = false;
				Debug.Log("クールタイム終了");
			}
		}
	}

	void StartGame(){
		// プレイヤーをランダムに鬼に設定する(未完成)
		int num = UnityEngine.Random.Range(0,4);
		Debug.Log(num);
	}

	void RandomSetOrgaPlayer(){
		int num = UnityEngine.Random.Range(0,_playerData.Count());
		ChangeOrgaPlayer(_playerData[num]);
	}

	void FinishGame(){	// ゲーム終了時に呼び出される関数
		if(_isFinishGame) return;
		Debug.Log("ゲームを終了します");
		_isFinishGame = true;

		// 接続しているプレイヤーすべてに終了命令を送る
	}

	bool CheckIdentityPlayer(int plNum, CPlayer player){
		if(_playerData[plNum] == player){
			Debug.Log(player.name);
			return true;
		}
		return false;
	}

	// 鬼が変わるタイミングで呼ぶ関数
	public void ChangeOrgaPlayer(CPlayer nextOrgaPlayer){
		Debug.Log(_playerData.Count());
		for(int i = 0; i < _playerData.Count(); i++){
			// オブジェクトが同一かどうか確認する
			if(CheckIdentityPlayer(i, nextOrgaPlayer)){
				nextOrgaPlayer.ChangeToOrga();	// 鬼になった通知を送る
				_orgaPlayer = nextOrgaPlayer;
				SetCoolTime();					// クールタイムを設定する
				Debug.Log("鬼が変更されました 次の鬼は" + _orgaPlayer.name);
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
		Debug.Log("プレイヤーを追加しました");
		Debug.Log("追加したプレイヤー" + player.name);
		Debug.Log("現在のプレイヤー数" + _playerData.Count());
	}

	private void SetCoolTime(){
		Debug.Log("クールタイムを開始する");
		_isCoolTimeNow = true;
		_progressCoolTime = 0f;
	}

	public bool CheckOverCoolTime(){
		if(_isCoolTimeNow) return false;
		return true;
	}

}
