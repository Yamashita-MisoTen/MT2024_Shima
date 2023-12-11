using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DG.Tweening;
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
	[SerializeField,Header("当たった時のエフェクト")] private GameObject collisonVFXPrefab;
	[Header("渦潮のプレハブ")]
	[SerializeField] GameObject _WhirloopPrefab;
	[SerializeField] float _whirloopLength = 5.0f;
	public bool isCanMove = false;
	public bool isOnWhirloop = false;
	float _rotAngle;
	[SerializeField] PlayerUI ui;
	bool _isNowOrga = false;
	bool _isCompleteChutolial = false;
	int _progressDeg = 0;

	GameRuleManager mgr;
	PlayerCamera cameraObj;
	PlayerAudio moveAudioComp;
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
		HitStopStart();
		cameraObj = this.GetComponent<PlayerCamera>();
	}

	// Update is called once per frame
	void Update()
	{
		// 今のシーンを確認してから入力機構切りたい

		CplayerMoveUpdate();	// 移動系の更新
		ParticleUpdate();
		HitStopUpdate();
		_rotAngle = this.gameObject.transform.eulerAngles.y;

		if(Input.GetKeyDown(KeyCode.H)){
			UseItem();
		}
	}
	[ClientRpc]
	public void RpcCreateSettings(string name, Color color){
		this.name = name;
		for(int i = 0; i < this.gameObject.transform.childCount; i++){
			var child = this.gameObject.transform.GetChild(i).gameObject;
			if(child.name != "PenguinFBX") continue;
			for(int j = 0; j < child.transform.childCount; j++){
				var grandChild = child.transform.GetChild(j);
				if(grandChild.name != "penguin") continue;
				Material mat = grandChild.GetComponent<SkinnedMeshRenderer>().material;
				mat.SetColor("_BaseColor", color);
				break;
			}
			break;
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
		moveAudioComp = this.gameObject.GetComponent<PlayerAudio>();
		moveAudioComp.SetUpAudio();
		SoundManager.instance.PlayAudio(SoundManager.AudioID.playerMove, moveAudioComp.GetAudioSource(), 0f);

		// 入力系をつける
		if(isLocalPlayer){
			var inputComp = this.gameObject.GetComponent<PlayerInput>();
			inputComp.enabled = true;
		}
	}

	private void OnCollisionEnter(Collision other) {
		if(!other.gameObject.CompareTag("Player")) return;

		// ローカルプレイヤーのときのみ
		if(!isLocalPlayer) return;
		// 自分が鬼のときのみ通知をする
		if(_isNowOrga && mgr.CheckOverCoolTime()){
			Debug.Log("あたり 私が鬼です" + this.name);
			CmdChangeOrga(other.gameObject);
		}

		// ここでレイを飛ばして正面に相手がいるかどうかを確認
		Vector3 origin = this.transform.position + new Vector3(0,0.5f,0); // 原点
		Vector3 direction = this.transform.forward; // X軸方向を表すベクトル
		var ray = new Ray(origin, direction); // Rayを生成;
		RaycastHit hit;

		if(Physics.Raycast(ray.origin,ray.direction, out hit, 1.0f)){
			if(hit.collider.gameObject == other.gameObject){
				var pos = this.transform.position + (this.transform.forward * 5f);
				Debug.Log("あああ" + pos);
				CmdSetKnockAway(pos, other.gameObject.GetComponent<CPlayer>());
			}
		}

		CmdEmergencyStop();
		// Collisonのエフェクト作成
		var obj = Instantiate(collisonVFXPrefab, new Vector3(0,0,0) , Quaternion.identity);
		obj.gameObject.transform.parent = this.gameObject.transform;
		obj.gameObject.transform.localPosition = new Vector3(0,0.5f,1);
		ui.SetActiveSaturateCanvas(true);
		obj.GetComponent<VisualEffect>().SendEvent("OnPlay");

		DOVirtual.DelayedCall(0.05f, () =>
		{
			HitStopPerformance();
		});

		// 最終のエフェクトを削除する
		DOVirtual.DelayedCall(0.5f, () =>
			{
				Destroy(obj);
				ui.SetActiveSaturateCanvas(false);
			}
		);
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
		CmdEmergencyStop();




		// Collisonのエフェクト作成
		var obj = Instantiate(collisonVFXPrefab, new Vector3(0,0,0) , Quaternion.identity);
		obj.gameObject.transform.parent = this.gameObject.transform;
		obj.gameObject.transform.localPosition = new Vector3(0,0.5f,1);
		ui.SetActiveSaturateCanvas(true);
		obj.GetComponent<VisualEffect>().SendEvent("OnPlay");

		DOVirtual.DelayedCall(0.05f, () =>
		{
			HitStopPerformance();
		});

		// 最終のエフェクトを削除する
		DOVirtual.DelayedCall(0.5f, () =>
			{
				Destroy(obj);
				ui.SetActiveSaturateCanvas(false);
			}
		);
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
		CmdEmergencyStop();
		isOnWhirloop = true;
		// カメラの設定
		cameraObj.SetCameraInWhirloop();

		// プレイヤーの体の角度を渦潮の方向に向ける
		// 回転のときのプレイヤーのカメラの更新処理
		var euler = new Vector3(CameraCopy.x, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
		var camqt = Quaternion.AngleAxis(Side_MoveNow * Camera_Deferred_Power, this.transform.up);
		cameraObj.CameraMoveforPlayerMove(euler, camqt);

		// 画面演出
		// 被写界深度
		//dof.focalLength.Override(120f);
		// 集中線
		ui.SetPlaneDistance(2);
		ui.SetActiveSaturateCanvas(true);
	}

	public void OutWhirloop(){
		Velocity = 5f;
		Debug.Log(Side_Move);
		Debug.Log(Side_MoveNow);
		isOnWhirloop = false;
		cameraObj.SetCameraOutWhirloop();

		// 角度補正
		var newangle = new Vector3(0f,this.transform.eulerAngles.y,0f);
		this.transform.eulerAngles = newangle;

		// 被写界深度
		//dof.focalLength.Override(0f);

		// 集中線
		ui.SetPlaneDistance(2);
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

	void OnDecision(InputValue value){
		if(isCanMove) return;

		// チュートリアルを進める
		_progressDeg++;

		// チュートリアル画像を差し替える

		if(_progressDeg == 1) CmdSetUpComplete();
	}

	[Command]
	private void CmdSetUpComplete(){
		if(_isCompleteChutolial) return;

		_isCompleteChutolial = true;
		mgr.CompleteChutolial();
	}

	[Command]
	private void CmdSetKnockAway(Vector3 position, CPlayer player){
		player.RpcSetKnockAway(position);
	}

	[ClientRpc]
	private void RpcSetKnockAway(Vector3 position){
		Debug.Log("あああああああああああああああああああああああああああああああああああ");
		this.transform.position = position;
	}
}
