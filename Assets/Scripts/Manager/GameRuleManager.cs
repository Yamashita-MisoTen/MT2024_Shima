using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Mirror.Examples.Basic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRuleManager : NetworkBehaviour
{
	//// **** 変数定義 **** ////

	// ** ゲームの状態関連
	enum GameState{
		Ready,
		NowPlay,
		Finish
	}

	GameState gameState = GameState.Ready;

	public struct SendOrgaPlayerData : NetworkMessage{
		public CPlayer nextOrgaPlayerData;
	}

	// ** 時間関係
	// ゲームの制限時間
	[SerializeField] [Header("ゲームの制限時間(秒)")]int _LimitTime = 300;
	float _progressLimitTime;	// 経過時間
	// 鬼のタッチ後のクールタイム
	[SerializeField]
	[Header("鬼変更後のクールタイム")]int _changeCoolTime = 5;
	[Header("UI関連")][SerializeField] private TextMeshProUGUI timerText;
	[SerializeField] private TextMeshProUGUI gameStateText;
	[SerializeField]float _progressCoolTime = 0.0f;	// 経過時間
	bool _isCoolTimeNow = false;

	// ** ゲーム開始前の準備時間関連

	// ** プレイヤー関係
	[SyncVar]List<CPlayer> _playerData;	// プレイヤーのデータを格納しておく
	[SerializeField]CPlayer _orgaPlayer;		// 現在鬼のプレイヤーを格納しておく

	// ** 共通のUI
	GameObject UICanvas = null;

	bool _isFinishGame = false;
	void Awake() {
		NetworkClient.RegisterHandler<SendOrgaPlayerData>(ReciveOrgaPlayerDataInfo);
	}
	void Start()
	{
		for(int i = 0; i < this.transform.childCount; i++){
			var obj = this.transform.GetChild(i);
			if(obj.name == "GameUICanvas"){
				UICanvas = obj.gameObject;
				UICanvas.SetActive(false);
			}
		}

		// 配列のnullチェック
		if(_playerData == null){
			_playerData = new List<CPlayer>();
		}

		DontDestroyOnLoad(this);
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
	public void ReadyGame(){
		//Debug.LogError("準備" + _playerData.Count);
		// プレイヤーの準備
		foreach(CPlayer p in _playerData){
			Debug.Log(p);
			Debug.Log(p.isLocalPlayer);
			p.DataSetUPforMainScene();
		}

		// UIの準備
		UICanvas.SetActive(true);
		if(isServer){
			RandomSetOrgaPlayer();
		}
	}

	[ClientRpc]
	void RpcStartGame(){	// ゲーム開始時に呼び出される関数
		Debug.Log("ゲーム開始するで");
		gameStateText.text = "now play";
		gameState = GameState.NowPlay;
		foreach(CPlayer p in _playerData){
			p.isCanMove = true;
		}
	}
	[ClientRpc]
	void RpcFinishGame(){	// ゲーム終了時に呼び出される関数
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
		gameStateText.text = "Ready Push 'L'Key ";
		// 開始前にカウントダウン入れたりする
		if(Input.GetKeyDown(KeyCode.L)){
			RpcStartGame();
		}
	}
	void UpdateGame(){
		if(_LimitTime < _progressLimitTime) RpcFinishGame();	// 制限時間を過ぎたらゲーム終了する
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
			if(_changeCoolTime > _progressCoolTime){
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
		Debug.Log("始めの鬼の番号 : " + num);
		// サーバー側の設定
		_orgaPlayer = _playerData[num];		// サーバー側だけ先に更新しても問題ないため念の為早めに鬼の設定をしておく
		SendChangeOrga(_playerData[num]);	// クライアントのマネージャーのデータを更新する
		// クライアント側にもデータを送る必要があるのでここで設定
		Debug.Log("初期の鬼が設定されました : " + _playerData[num].name);
	}
	bool CheckIdentityPlayer(int plNum, CPlayer player){	// 同一のプレイヤーか確認する
		if(_playerData[plNum] == player){
			return true;
		}
		return false;
	}

	// ** 鬼変更関連の関数群
	[Server]
	public void ServerGetChangeOrga(CPlayer nextOrgaPlayer){
		if(!CheckOverCoolTime()) return;
		SetCoolTime();
		SendChangeOrga(nextOrgaPlayer);
	}
	void SendChangeOrga(CPlayer nextOrgaPlayer){
		if(!isClient) return;
		// クライアント側に次の鬼を通知する為にメッセージを送る
		var sendData = new SendOrgaPlayerData() {nextOrgaPlayerData = nextOrgaPlayer};
		NetworkServer.SendToAll(sendData);
	}

	private void ChangeOrgaPlayer(CPlayer nextOrgaPlayer){
		// 各クライアント内でのプレイヤーデータの更新を行う
		Debug.Log("データ送信します" + isServer);
		Debug.Log("次の鬼は " + nextOrgaPlayer.name);
		for(int i = 0; i < _playerData.Count(); i++){
			Debug.Log("鬼の確認中 :" + i + "番");
			bool orgaFlg = false;
			if(CheckIdentityPlayer(i, nextOrgaPlayer)){
				orgaFlg = true;
			}
			_playerData[i].ChangeOrgaPlayer(orgaFlg);
		}
	}

	/// <summary>
	/// サーバーから受け取ったデータを各クライアントで使う
	/// </summary>
	/// <param name="conn">コネクション情報　関数内で使ってないけど必要みたい</param>
	/// <param name="receivedData">受け取ったデータ</param>
	private void ReciveOrgaPlayerDataInfo(SendOrgaPlayerData receivedData)
	{
		//ローカルのフラグに反映
		_orgaPlayer = receivedData.nextOrgaPlayerData;
		Debug.Log(_orgaPlayer);
		ChangeOrgaPlayer(receivedData.nextOrgaPlayerData);
	}

	private void SetCoolTime(){		// クールタイムの設定
		// これももしかしたらサーバー通知のほうがいいかも
		// ラグがあるからね
		Debug.Log("クールタイムを開始する");
		_isCoolTimeNow = true;
		_progressCoolTime = 0f;
	}

	public bool CheckOverCoolTime(){	// クールタイム終わってるか確認
		return !_isCoolTimeNow;
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
			if(_playerData[i].connectionToClient.connectionId == player.connectionToClient.connectionId) return;
		}

		_playerData.Add(player);	// プレイヤーのデータを格納する
		Debug.Log("プレイヤーを追加しました");
		Debug.Log("追加したプレイヤー" + player.name);
		Debug.Log("現在のプレイヤー数" + _playerData.Count());

		// ロビーでの座標を調整する
		int detail = _playerData.Count() - 1;
		Vector3 pos = new Vector3( -1.5f + detail, 0, 0);
		playerObj.transform.position = pos;

		RpcSendPlayerData(_playerData);
	}

	public List<CPlayer> GetAllPlayerData(){
		return _playerData;
	}

	public void RemovePlayerData(GameObject playerObj){
		CPlayer player = playerObj.GetComponent<CPlayer>();
		// nullチェック
		if(player == null) return;
		if(_playerData == null) return;

		CPlayer deleteObj = null;
		foreach(CPlayer p in _playerData){
			if(p == player){
				Debug.Log("プレイヤーを削除しました");
				Debug.Log("削除したプレイヤー" + player.name);
				Debug.Log("現在のプレイヤー数" + (_playerData.Count() - 1));
				deleteObj = player;
			}
		}
		if(deleteObj != null){
			_playerData.Remove(player);
		}
	}

	public void RemoveAllPlayerData(){
		_playerData.Clear();
		Debug.Log("プレイヤーを全て削除しました");
		NetworkServer.Destroy(this.gameObject);
	}

	[ClientRpc]
	void RpcSendPlayerData(List<CPlayer> players){
		_playerData = players;
		Debug.Log("クライアントにデータを送信しました 人数 : " + _playerData.Count);

		foreach(CPlayer p in players){
			DontDestroyOnLoad(p);
		}
	}
}
