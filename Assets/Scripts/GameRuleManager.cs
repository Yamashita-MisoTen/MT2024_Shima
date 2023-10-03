using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Mirror.Examples.Basic;
using TMPro;
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
	[SerializeField] private TextMeshProUGUI text;
	float _progressCoolTime;	// 経過時間
	bool _isCoolTimeNow = false;

	// ** ゲーム開始前の準備時間関連
	bool _isGameReady = false;

	// ** プレイヤー関係
	List<CPlayer> _playerData;	// プレイヤーのデータを格納しておく
	CPlayer _orgaPlayer;		// 現在鬼のプレイヤーを格納しておく

	bool _isFinishGame = false;
	void Start()
	{
		_playerData = new List<CPlayer>(0);
	}

	// Update is called once per frame
	void Update()
	{
		if(!_isGameReady){
			if(Input.GetKeyDown(KeyCode.Space)) GameReady();
			return;
		}
		if(_LimitTime < _progressLimitTime) FinishGame();	// 制限時間を過ぎたらゲーム終了する
		_progressLimitTime += Time.deltaTime;				// 経過時間を更新する
		var timer = (int)(_LimitTime - _progressLimitTime);
		string min = ((int)(timer / 60)).ToString();
		string sec = (timer % 60).ToString();
		text.text = min +":"+ sec;

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

	// ** ゲームの開始終了関連の関数

	void StartGame(){	// ゲーム開始時に呼び出される関数
		RandomSetOrgaPlayer();
	}


	void FinishGame(){	// ゲーム終了時に呼び出される関数
		if(_isFinishGame) return;
		Debug.Log("ゲームを終了します");
		_isFinishGame = true;

		// 接続しているプレイヤーすべてに終了命令を送る
	}

	void GameReady(){
		// 開始前にカウントダウン入れたりする
		_isGameReady = true;
	}


	void RandomSetOrgaPlayer(){		// 鬼をランダムで変更する
		int num = UnityEngine.Random.Range(0,_playerData.Count());
		ChangeOrgaPlayer(_playerData[num]);
	}

	bool CheckIdentityPlayer(int plNum, CPlayer player){	// 同一のプレイヤーか確認する
		if(_playerData[plNum] == player){
			Debug.Log(player.name);
			return true;
		}
		return false;
	}

	public void ChangeOrgaPlayer(CPlayer nextOrgaPlayer){	// 鬼が変わるタイミングで呼ぶ関数
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

	public void AddPlayerData(GameObject playerObj){
		CPlayer player = playerObj.GetComponent<CPlayer>();
		if(player == null){
			Debug.Log("追加されたプレハブがおかしい");
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
