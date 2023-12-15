using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using System.Collections;
using System;
using Mirror.Examples.Common;
using Unity.VisualScripting;
using DG.Tweening;
using TMPro;


public partial class CPlayer : NetworkBehaviour
{
	public enum eJump_Type
	{
		UP,
		SIDE,
	}

	// ** 移動類のパラメータ
	private float Velocity;  //入力されている速度
	public float NowVelocity;  //現在の速度
	[SerializeField, Header("移動の速度制限")]
	private float Velocity_Limit;


	[SerializeField, Header("移動の加速度")]
	private float Acceleration;

	[SerializeField, Tooltip("ジャンプのクールタイム")] private float jumpCollTime = 3f;

	//イベントなど加速の値が変化するとき
	private float Velocity_Addition;
	[SerializeField] float orgaPlusSpeed = 5.0f;
	private float velocity_Orga;

	//?????x
	private float Deceleration = 0.5f;
	private bool isCanJump = true;

	private Vector3 Start_Position;
	private eJump_Type Jump_Type;

	// ジャンプのクールタイム用の経過時間
	private float requireJumpTime = 0f;

	//ダッシュ落下速度
	private float Jump_Fall = 1.0f;

	//横回転移動のパラメーター
	private float Side_Move = 0.0f;
	//横回転移動のパラメーター
	private float Side_MoveNow = 0.0f;
	//横回転移動の速度制限
	private float Side_Move_Limit = 2.0f;
	//横回転移動の速度調整用
	private float Side_Acceleration = 3.0f;

	//カメラオブジェクト
	private GameObject CameraObject;

	//カメラスクリプト
	private PlayerCamera C_Camera;

	private Vector3 CameraCopy = Vector3.zero;

	[SerializeField, Header("カメラ遅延の大きさ")]
	private float Camera_Deferred_Power;

	[Header("泳ぐアニメーション")]
	private Animator Swimming;

	private float AttenRate = 0.01f;    // Start is called before the first frame update
	void CPlayerMoveStart()
	{
		// 子供を検索してカメラを確認する
		for (int i = 0; i < this.transform.childCount; i++)
		{
			GameObject childObj = this.transform.GetChild(i).gameObject;
			// 接続時にプレイヤーごとにカメラを分ける
			if (childObj.name == "PlayerCamera")
			{
				CameraObject = childObj;
				CameraObject.SetActive(false);
				continue;
			}

			//アニメーションを代入
			if (childObj.name == "PenguinFBX")
			{
				Swimming = childObj.GetComponent<Animator>();
				continue;
			}
		}

		CameraCopy = CameraObject.transform.eulerAngles;
		C_Camera = GetComponent<PlayerCamera>();

		Start_Position = this.transform.position;
		Jump_Type = eJump_Type.SIDE;
		NowVelocity = 0.0f;
	}

	// Update is called once per frame
	void CplayerMoveUpdate()
	{
		if (!isCanMove) return;

		if(!isCanJump){
			requireJumpTime += Time.deltaTime;
			if(requireJumpTime >= jumpCollTime){
				isCanJump = true;
				requireJumpTime = 0f;
			}
		}

		// 渦潮に乗ってるときは基本挙動の更新をしない
		if (isOnWhirloop){
			return;
		}

		// 動いてるときの音
		if (NowVelocity > 0)
		{
			isSwim = true;
			var ratio = NowVelocity / Velocity_Limit;
			SoundManager.instance.ChangeVolume(ratio / 50, moveAudioComp.GetAudioSource());
			cameraObj.cameraComp.fieldOfView = Mathf.Lerp(60, 75, ratio);
		}
		else
		{
			SoundManager.instance.ChangeVolume(0f, moveAudioComp.GetAudioSource());
			isSwim = false;
			cameraObj.cameraComp.fieldOfView = 60;
		}

		//アニメーションに数値代入
		Swimming.SetFloat("MoveSpeed", NowVelocity);
		if (Mathf.Abs(NowVelocity) >= Velocity_Limit)
		{
			Swimming.SetBool("MoveFastest", true);
		}
		else
		{
			Swimming.SetBool("MoveFastest", false);
		}

		// サーバー側での各プレイヤーの入力値に応じた加速度を計算する
		// サーバー側での処理をしないとクライアントで生じる更新回数の差でズレが生じるため
		if (isServer) PlayerMoveServerProcess();
		// クライアントでサーバー側から得た情報で更新を行う
		PlayerMoveClientProcess();

	}

	void PlayerMoveServerProcess()
	{
		// ** サーバー側でプレイヤーの挙動を制御する ** //
		// ** 基本移動
		var velocity = ForwardMove();

		// ** 左右への移動の挙動
		var sidevelocity = SideMove();

		// 更新を各クライアントに同期する
		RpcSendPlayerTransform(velocity, sidevelocity);
	}

	void PlayerMoveClientProcess()
	{
		// クライアント側で最終の更新を行う
		if(isOnWhirloop) return;
		// 移動
		this.transform.position += this.gameObject.transform.forward * (NowVelocity + Velocity_Addition + velocity_Orga) * Time.deltaTime;
		// 回転
		var qt = this.transform.rotation;
		if(Side_MoveNow != 0.0f){
			qt *= Quaternion.AngleAxis(Side_MoveNow, this.gameObject.transform.up);
			// 回転のときのプレイヤーのカメラの更新処理
			var euler = new Vector3(CameraCopy.x, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
			var camqt = Quaternion.AngleAxis(Side_MoveNow * Camera_Deferred_Power, this.transform.up);
			cameraObj.CameraMoveforPlayerMove(euler, camqt);
		}
		this.transform.rotation = qt;
	}

	float ForwardMove(){
		// ** 基本移動
		// 加速度
		if (Velocity == 0f && NowVelocity > 0f) {
			// 減速処理
			NowVelocity -= Deceleration * Time.deltaTime;
			// 最終補正
			if (NowVelocity < 0f) NowVelocity = 0f;
		}
		else {
			NowVelocity += Velocity;
		}
		// 加速度に制限をかける
		NowVelocity = Mathf.Clamp(NowVelocity, -Velocity_Limit, Velocity_Limit);
		// 実際に動かす
		return NowVelocity;
	}

	float SideMove(){
		// ** 左右への移動の挙動
		Quaternion qt = this.gameObject.transform.rotation;

		// カメラの操作をしてるときは横操作できない
		if (!C_Camera.Looking_Left_Right()) return 0f;
		//横移動
		if (Side_Move != 0) Side_MoveNow += Side_Move * Time.deltaTime;

		// 減速処理
		if(Side_Move == 0.0f){
			// 加速度の絶対値が0.1f以下になった場合の処理
			if(Mathf.Abs(Side_MoveNow) <= 0.1f){
				Side_MoveNow = Vector2.zero.x;
			}else{
				int pol = 1;	// 補正値
				if(Side_MoveNow > 0f){
					pol = -1;
				}
				// 減速
				Side_MoveNow += pol * 0.01f;
			}
		}
		// 左右の移動値に制限をかける
		Side_MoveNow = Mathf.Clamp(Side_MoveNow, -Side_Move_Limit, Side_Move_Limit);
		return Side_MoveNow;
	}

	private void OnAccelerator(InputValue value) // アクセル
	{
		if (!isCanMove) return;
		if (isOnWhirloop) return;

		var axis = value.Get<float>();
		// 加速度の更新
		CmdUpdateVelocity(axis * Acceleration);
	}
	private void OnMove(InputValue value) // 左右の入力
	{
		if (!isCanMove) return;
		if (isOnWhirloop) return;

		var axis = value.Get<Vector2>();
		CmdUpdateSideMove(axis.x * Side_Acceleration);
	}

	public float GetNowSpeed(){
		return NowVelocity + Velocity_Addition + velocity_Orga;
	}

	private void OnJump() // ジャンプ
	{
		if (!isCanMove) return;		// 移動可能な場合のみ
		// ジャンプの制限を確認する
		if(!isCanJump) return;		// ジャンプのクールタイム
		if(!_isNowOrga) return;		// 自分が鬼のときのみ
		if(isOnWhirloop) return;	// 渦潮に乗ってないときのみ
		if(!isLocalPlayer) return;	// 操作キャラのときのみ
		// ジャンプの変数更新
		isCanJump = false;
		// UIを更新する
		ui.SetCharge();
		// 渦潮を生成する
		CmdCreateWhrloop();
	}

	public void OnUseItem()	// アイテム使用
	{
		if (!isCanMove) return;
		if (!isLocalPlayer) return;
		if (isOnWhirloop) return;
		if (_HaveItemData == null) return;
		Debug.Log("Num1");
		Debug.Log(_HaveItemData);
		CmdUseItem(_HaveItemData);
		_HaveItemData = null;
		ui.SetItemTexture(ui.defaultItemTex);
	}
	[Command]
	void CmdUseItem(Item item){
		Debug.Log("Num2");
		item.UseEffect(this.transform.position, this.transform.rotation);
	}

	// 通信で用いる同期関数群

	[ClientRpc] private void RpcSendPlayerTransform(float velocity, float sidevelocity)
	{
		NowVelocity = velocity;
		Side_MoveNow = sidevelocity;
	}

	[Command] private void CmdUpdateSideMove(float side)
	{
		Side_Move = side;
	}

	[Command] private void CmdUpdateVelocity(float velo)
	{
		Velocity = velo;
	}

	// 緊急停止用
	[Command]
	private void CmdEmergencyStop()
	{
		Debug.Log("緊急停止");
		NowVelocity = 0.0f;
		Velocity = 0.0f;
		Side_MoveNow = 0.0f;
		Side_Move = 0.0f;
	}

	public bool Moving_Left_Right()
	{
		if (Side_MoveNow != 0.0f)
			return false;

		return true;
	}
}