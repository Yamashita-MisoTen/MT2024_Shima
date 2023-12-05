using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Mirror;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

public partial class CPlayer : NetworkBehaviour
{
	[SerializeField] bool _isNowOrga = false;
	[Header("渦潮のプレハブ")]
	[SerializeField] GameObject _WhirloopPrefab;
	[SerializeField] float _whirloopLength = 5.0f;
	public bool isCanMove = false;
	public bool isOnWhirloop = false;
	float _rotAngle;
	[SerializeField] PlayerUI ui;

	GameRuleManager mgr;
	GameRuleManager saturateLineUI;
	PlayerCamera cameraObj;
	PlayerAudio audioComp;
	Camera renderCamera;
	
	Volume volume;
	DepthOfField dof;
	// アイテム所持するように
	Item _HaveItemData;

	/// </summary>
	/// // Start is called before the first frame update
	void Start()
	{
		CPlayerMoveStart();
		ParticleStart();
		cameraObj = this.GetComponent<PlayerCamera>();
	}

	// Update is called once per frame
	void Update()
	{
		// 今のシーンを確認してから入力機構切りたい

		CplayerMoveUpdate();	// 移動系の更新
		ParticleUpdate();
		_rotAngle = this.gameObject.transform.eulerAngles.y;

		if(Input.GetKeyDown(KeyCode.H)){
			UseItem();
		}
	}

	public void InitData(){
		transform.position = new Vector3(0,0,0);
		isCanMove = false;
		isOnWhirloop = false;
		_isNowOrga = false;
		_rotAngle = 0f;
		ParticleStartUpSwitch(orgaFX, false);
	}

	public void ResultData(){
		isCanMove = false;
		isOnWhirloop = false;
		ParticleStartUpSwitch(orgaFX, false);
		ui.SetActiveUICanvas(false);
		cameraObj.SetCamera(false);
	}

	public void DataSetUPforMainScene(GameRuleManager manager){
		mgr = manager;
		// メインシーンでのセットアップで使用する
		this.GetComponent<PlayerUI>().MainSceneUICanvas();

		this.GetComponent<PlayerUI>().ChangeJumpType(Jump_Type);
		if(cameraObj == null) cameraObj = this.GetComponent<PlayerCamera>();
		cameraObj.MainSceneCamera();

		volume = GameObject.Find("Volume").GetComponent<Volume>();
		Debug.Log(volume);
		volume.profile.TryGet< DepthOfField >(out dof);

		// オーディオ系をつける
		audioComp = this.gameObject.GetComponent<PlayerAudio>();
		audioComp.SetUpAudio();
		SoundManager.instance.PlayAudio(SoundManager.AudioID.playerMove, audioComp.GetAudioSource(), 0f);


		// 入力系をつける
		if(isLocalPlayer){
			var inputComp = this.gameObject.GetComponent<PlayerInput>();
			inputComp.enabled = true;
		}
	}

	private void OnCollisionEnter(Collision other) {
		// Debug.Log("あたり");
		// if(!other.gameObject.CompareTag("Player")) return;

		// // 自分が鬼のときのみ通知をする
		// Debug.Log("いまは" + mgr.CheckOverCoolTime());
		// if(_isNowOrga && mgr.CheckOverCoolTime()){
		// 	Debug.Log("当たり判定発生");
		// 	CmdChangeOrga(other.gameObject);
		// }
	}

	private void OnTriggerEnter(Collider other){
		if(!other.gameObject.CompareTag("Player")) return;

		// ローカルプレイヤーのときのみ
		if(!isLocalPlayer) return;
		// 自分が鬼のときのみ通知をする
		if(_isNowOrga && mgr.CheckOverCoolTime()){
			Debug.Log("あたり 私が鬼です" + this.name);
			CmdChangeOrga(other.gameObject);
		}
	}

	[Command]
	void CmdChangeOrga(GameObject otherObj){
		mgr.ServerGetChangeOrga(otherObj.GetComponent<CPlayer>());
	}

	public void ChangeOrgaPlayer(bool orgaflg){
		_isNowOrga = orgaflg;
		ParticleStartUpSwitch(orgaFX, orgaflg);
		ui.ChangeOrgaPlayer(_isNowOrga);
	}

	[Command]
	private void CmdCreateWhrloop(){
		RpcCreateWhirloop();
	}

	[ClientRpc]
	private void RpcCreateWhirloop(){
		Debug.Log("うずしおせいせい");
		// 渦潮を生成する座標を自分の現在の座標を格納する
		Vector3 whirloopPosition = this.transform.position;
		// 回転角をクオータニオンに変換
		Quaternion qtAngle;
		if(Jump_Type == eJump_Type.SIDE){
			qtAngle = Quaternion.AngleAxis(_rotAngle, this.transform.up);
		}else{
			qtAngle = Quaternion.AngleAxis(270, new Vector3(1,0,0));
		}
		// オブジェクトを生成する
		var obj = Instantiate(_WhirloopPrefab, whirloopPosition, Quaternion.identity);
		// 渦潮のセットアップ
		obj.GetComponent<WhirloopBase>().SetUpWhrloop(_whirloopLength, 1.0f, qtAngle);
		// 向きの更新
		// obj.transform.rotation = qtAngle * obj.transform.rotation;
		NetworkServer.Spawn(obj);
	}

	public void InWhirloopSetUp(){
		Emergency_Stop();
		isOnWhirloop = true;
		// カメラの設定
		cameraObj.SetCameraInWhirloop();

		// プレイヤーの体の角度を渦潮の方向に向ける

		// 画面演出
		// 被写界深度
		//dof.focalLength.Override(120f);
		// 集中線
		ui.SetActiveSaturateCanvas(true);
	}

	public void OutWhirloop(){
		isOnWhirloop = false;
		cameraObj.SetCameraOutWhirloop();
		// 被写界深度
		//dof.focalLength.Override(0f);
		// 集中線
		ui.SetActiveSaturateCanvas(false);
	}

	public void SetItem(Item item){
		Debug.Log(item);
		_HaveItemData = item;
		ui.SetItemTexture(item.itemTex);
	}

	public bool isHaveItem(){
		return _HaveItemData == null;
	}

	void UseItem(){
		_HaveItemData.UseEffect(this.transform.position ,this.transform.rotation);
		_HaveItemData = null;
		ui.SetItemTexture(ui.defaultItemTex);
	}

	public Camera GetRenderCamera(){
		if(renderCamera == null) renderCamera = cameraObj.cameraComp;
		return renderCamera;
	}
}
