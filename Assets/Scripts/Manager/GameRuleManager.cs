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
using DG.Tweening;

public partial class GameRuleManager : NetworkBehaviour
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

	public struct SendCompleyeChangeSceme : NetworkMessage{
		public bool isChangeSceneComplete;
	}
	[SerializeField, Header("デバッグモード")] bool isDebugMode = false;
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
	[SerializeField, Header("イベントマネージャー")] GameObject eventMgrPrefabs;
	[SerializeField, Header("イベントの発生時間(s)")] List<int> eventTimer;
	[SerializeField] List<Vector3> playerStartPosition;
	[SerializeField] List<Vector3> resultPos;

	EventMgr eventMgr;
	int nextEventNum = 0;
	bool _isCoolTimeNow = false;
	int completeChangeSceneClient = 0;

	// ** ゲーム開始前の準備時間関連

	// ** プレイヤー関係
	[SerializeField]List<CPlayer> _playerData;	// プレイヤーのデータを格納しておく
	[SerializeField][SyncVar]CPlayer _orgaPlayer;	// 現在鬼のプレイヤーを格納しておく

	// ** 共通のUI
	FadeMgr fadeMgr;
	ResultFade fadeResult;
	GameObject UICanvas = null;
	List<Canvas> canvas;
	CustomNetworkManager netMgr = null;
	SoundManager soundManager = null;

	bool _isFinishGame = false;
	// **　リザルト関連
	bool isResultAnnounce = false;	// 結果発表が終われば

	void Awake() {
		NetworkClient.RegisterHandler<SendOrgaPlayerData>(ReciveOrgaPlayerDataInfo);
		NetworkClient.RegisterHandler<SendCompleyeChangeSceme>(ReciveChangeSceneClient);
		netMgr = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
		fadeMgr = GameObject.Find("Pf_FadeCanvas").GetComponent<FadeMgr>();
		fadeResult = GameObject.Find("Pf_ResultFade").GetComponent<ResultFade>();
	}
	void Start()
	{
		for(int i = 0; i < this.transform.childCount; i++){
			var obj = this.transform.GetChild(i);
			canvas = new List<Canvas>();
			if(obj.name == "GameUICanvas"){
				UICanvas = obj.gameObject;
				canvas.Add(UICanvas.GetComponent<Canvas>());
				UICanvas.SetActive(true);
			}
			if(obj.name == "ReadyCanvas"){
				readyCanvasObj = obj.gameObject;
				canvas.Add(readyCanvasObj.GetComponent<Canvas>());
				readyCanvasObj.SetActive(true);
			}
		}
		if(netMgr == null)GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
		var mgr = Instantiate(eventMgrPrefabs);
		eventMgr = mgr.GetComponent<EventMgr>();
		ReadyGame();
	}

	[SerializeField] GameObject TestPre;
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

	private void ReadyGame(){
		if(_playerData == null) {
			_playerData = new List<CPlayer>();
			Debug.Log(_playerData);
			Debug.Log(netMgr);
		}
		_playerData = netMgr.GetPlayerDatas(_playerData);
		Debug.Log("準備" + _playerData.Count);
		int num = 1;
		// プレイヤーの準備
		foreach(CPlayer p in _playerData){
			p.name = "Player" + num;
			p.DataSetUPforMainScene(this);
			if(p.isLocalPlayer){
				fadeMgr.SetRenderCamera(p.GetRenderCamera());
				fadeResult.SetCamera(p.GetRenderCamera());
				foreach(Canvas c in canvas){
					c.worldCamera = p.GetRenderCamera();
				}
			}
			num++;
		}
		RandomSetOrgaPlayer();

		for(int i = 0; i < _playerData.Count; i++){
			if(_playerData[i].isLocalPlayer){
				eventMgr.SetUpUI(_playerData[i].gameObject);
			}
			// 座標をスタート位置に格納する
			_playerData[i].transform.position = playerStartPosition[i];
		}

		fadeMgr.StartFadeIn();

		// デバッグの時は演出いれない
		if(!isDebugMode){
			DOVirtual.DelayedCall(fadeMgr.fadeInTime, () => StartReadyPerformance(), false);
		}
	}

	[ClientRpc]
	void RpcStartGame(){	// ゲーム開始時に呼び出される関数

		// ゲームのBGM流す
		BGMSoundManager.instance.PlayAudio(BGMSoundManager.AudioID.GameBGM);

		countdownText.text = "Start!!";
		// 1s後に非表示に
		DOVirtual.DelayedCall (1f, ()=> readyCanvasObj.SetActive(false), false);

		gameState = GameState.NowPlay;
		ChangeOrgaPlayer(_orgaPlayer);
		foreach(CPlayer p in _playerData){
			p.isCanMove = true;
		}
	}
	[ClientRpc]
	void RpcFinishGame(){	// ゲーム終了時に呼び出される関数
		if(_isFinishGame) return;
		Debug.Log("ゲームを終了します");
		_isFinishGame = true;
		gameState = GameState.Finish;
		timerText.text = "";

		countdownText.text = "Finish!!";

		SoundManager.instance.PlayAudio(SoundManager.AudioID.whistle);
		SoundManager.instance.ChangeVolume(0.1f);

		readyCanvasObj.SetActive(true);

		float waitTime = 3f;

		// 敗者一応置いといたほうがよき？
		DOVirtual.DelayedCall(waitTime,() => countdownText.text = "");
		DOVirtual.DelayedCall(waitTime,() => fadeResult.StartFadeOut());
		DOVirtual.DelayedCall(waitTime + fadeResult.fadeOutTime,() => StartResultPerforamce());

		// 接続しているプレイヤーすべてに終了命令を送る
	}

	void UpdateReady(){
		if(isDebugMode){
			gameStateText.text = "Ready Push 'L'Key ";
			// 開始前にカウントダウン入れたりする
			if(Input.GetKeyDown(KeyCode.L)){
				RpcStartGame();
			}
		}else{
			UpdateReadyPerformance();
			if(isCompleteCountdown){
				RpcStartGame();
			}
		}
	}
	void UpdateGame(){
		if(_LimitTime < _progressLimitTime) RpcFinishGame();	// 制限時間を過ぎたらゲーム終了する
		_progressLimitTime += Time.deltaTime;				// 経過時間を更新する
		var timer = (int)(_LimitTime - _progressLimitTime);

		// イベント発生のお知らせ
		if(eventTimer.Count != nextEventNum){
			if(eventTimer[nextEventNum] >= timer){
				eventMgr.RandomEventLottery();
				if(eventTimer.Count > nextEventNum){
					nextEventNum++;
				}
			}
		}

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
		if(!isResultAnnounce){
			UpdateResultPerformance();
		}else{
			if(!isServer) return;
			if(Input.GetKeyDown(KeyCode.L)){
				netMgr.ServerChangeScene("Title");
			}
		}
	}

	// ** 単発系の関数
	[Server]
	void RandomSetOrgaPlayer(){		// 鬼をランダムで変更する
		int num = UnityEngine.Random.Range(0,_playerData.Count());
		Debug.Log("始めの鬼の番号 : " + num);
		_orgaPlayer = _playerData[num];
		//SendChangeOrga(_orgaPlayer);
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
		Debug.Log("aaa");
		SendChangeOrga(nextOrgaPlayer);
	}
	void SendChangeOrga(CPlayer nextOrgaPlayer){
		if(!isClient) return;
		// クライアント側に次の鬼を通知する為にメッセージを送る
		var sendData = new SendOrgaPlayerData() {nextOrgaPlayerData = nextOrgaPlayer};
		ChangeOrgaPlayer(nextOrgaPlayer);
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
		Debug.Log("次の鬼はこれレシーブした" + _orgaPlayer);
		ChangeOrgaPlayer(receivedData.nextOrgaPlayerData);
	}

	private void ReciveChangeSceneClient(SendCompleyeChangeSceme receivedData){
		completeChangeSceneClient++;
		Debug.Log("シーン遷移完了通知");
		if(_playerData.Count == completeChangeSceneClient){
			RandomSetOrgaPlayer();
		}
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
}
