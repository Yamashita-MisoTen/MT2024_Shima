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

	// ** ゲームの状態関連
	enum GameState{
		Ready,
		NowPlay,
		Finish
	}

	GameState gameState = GameState.Ready;

	// ** 時間関係
	// ゲームの制限時間
	[SerializeField] [Header("ゲームの制限時間(秒)")]int _LimitTime = 300;
	float _progressLimitTime;	// 経過時間
	// 鬼のタッチ後のクールタイム
	[SerializeField]
	[Header("鬼変更後のクールタイム")]int _changeCoolTime = 5;
	[SerializeField] private TextMeshProUGUI timerText;
	[SerializeField] private TextMeshProUGUI gameStateText;
	float _progressCoolTime;	// 経過時間
	bool _isCoolTimeNow = false;

	// ** ゲーム開始前の準備時間関連
	bool _isGameReady = true;

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
		switch(gameState){
			case GameState.Ready:
				UpdateReady();	// ゲーム始める前の演出などの更新
				break;
			case GameState.NowPlay:
				UpdateGame();	// ゲーム本編の更新
				break;
			case GameState.Finish:
				UpdateFinsh();	// ゲーム終了の更新
				break;
		}
	}

	// ** ゲームの開始終了関連の関数
	void StartGame(){	// ゲーム開始時に呼び出される関数
		gameStateText.text = "now play";
		gameState = GameState.NowPlay;
		RandomSetOrgaPlayer();
	}
	void FinishGame(){	// ゲーム終了時に呼び出される関数
		if(_isFinishGame) return;
		Debug.Log("ゲームを終了します");
		_isFinishGame = true;
		gameStateText.text = "Finish";
		gameState = GameState.Finish;
		timerText.text = "";

		// 敗者一応置いといたほうがよき？

		// 接続しているプレイヤーすべてに終了命令を送る
	}

	void UpdateReady(){
		gameStateText.text = "Ready";
		// 開始前にカウントダウン入れたりする
		if(Input.GetKeyDown(KeyCode.Space)){
			StartGame();
			_isGameReady = false;
		}
	}
	void UpdateGame(){
		if(_LimitTime < _progressLimitTime) FinishGame();	// 制限時間を過ぎたらゲーム終了する
		_progressLimitTime += Time.deltaTime;				// 経過時間を更新する
		var timer = (int)(_LimitTime - _progressLimitTime);
		// UI用に時間を計算しなおす
		string min = ((int)(timer / 60)).ToString();	// 分計算
		int secNum = timer % 60;		// 秒計算
		string sec = secNum.ToString();
		if(secNum < 10) sec = "0" + sec;	// 10秒以下の場合09などのように2桁表記に変更
		timerText.text = min +":"+ sec;	// UIに更新入れる

		// タッチのクールタイムの設定
		if(_isCoolTimeNow){	// キャラクターの点滅とか入れたい
			if(_changeCoolTime < _progressCoolTime){
				_progressCoolTime += Time.deltaTime;
			}else{
				_isCoolTimeNow = false;
				Debug.Log("クールタイム終了");
			}
		}
	}

	void UpdateFinsh(){

	}

	// ** 単発系の関数
	void RandomSetOrgaPlayer(){		// 鬼をランダムで変更する
		int num = UnityEngine.Random.Range(0,_playerData.Count());
		//ChangeOrgaPlayer(_playerData[num]);
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
	private void SetCoolTime(){		// クールタイムの設定
		Debug.Log("クールタイムを開始する");
		_isCoolTimeNow = true;
		_progressCoolTime = 0f;
	}

	public bool CheckOverCoolTime(){	// クールタイム終わってるか確認
		if(_isCoolTimeNow) return false;
		return true;
	}

	// ** 通信関連の関数

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


}
