using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using System.Collections;
using System;
using Mirror.Examples.Common;


public partial class CPlayer : NetworkBehaviour
{
	public enum eJump_Type
	{
		UP,
		SIDE,
	}

	// ** 移動類のパラメータ
	private float Velocity;  //入力されている速度
	private float NowVelocity;  //現在の速度
	[SerializeField, Header("移動の速度制限")]
	private float Velocity_Limit;


	[SerializeField, Header("移動の加速度")]
	private float Acceleration;

	//イベントなど加速の値が変化するとき
	private float Velocity_Addition;

	//?????x
	private float Deceleration = 0.5f;

	private float NowJump_speed;
	private float Jump_Speed = 10;
	private bool Jump_Switch;
	private Vector3 Start_Position;
	private eJump_Type Jump_Type;

	// ** 横ダッシュのパラメーター
	//横ダッシュ加速度
	private float SJump_Acceleration = 20.0f;
	//全体の時間
	private float SJump_AllTime = 1.0f;
	//ジャンプ経過時間
	private float SJump_NowTime;
	//現在の速度
	private float SJump_Speed;

	//ダッシュ落下速度
	private float Jump_Fall = 1.0f;

	// ** 縦ダッシュのパラメーター
	//全体の時間
	private float HJump_AllTime = 1.0f;
	//ジャンプ経過時間
	private float HJump_NowTime;

	// ** 落下のパラメーター
	//落下速度
	private float Fall_Speed;
	//落下加速度
	private float Fall_Acceleration = 1.0f;

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
			}

			//アニメーションを代入
			if (childObj.name == "PenguinFBX")
			{
				Swimming = childObj.GetComponent<Animator>();
			}
		}

		CameraCopy = CameraObject.transform.eulerAngles;
		C_Camera = GetComponent<PlayerCamera>();

		Start_Position = this.transform.position;
		Jump_Type = eJump_Type.SIDE;
		NowVelocity = 0.0f;

		// 子供を検索してカメラを確認する
		for (int i = 0; i < this.transform.childCount; i++)
		{
			GameObject childObj = this.transform.GetChild(i).gameObject;
			// 接続時にプレイヤーごとにカメラを分ける
			if (childObj.name == "PlayerCamera")
			{
				CameraObject = childObj;
				CameraObject.SetActive(false);
				break;
			}
		}
		CameraCopy = CameraObject.gameObject.transform.eulerAngles;
		C_Camera = GetComponent<PlayerCamera>();
	}

	// Update is called once per frame
	void CplayerMoveUpdate()
	{

		if(!isCanMove) return;
		if(!isOnWhirloop){
			// 動いてるときの音
			if(NowVelocity > 0){
				isSwim = true;
				var ratio = NowVelocity / Velocity_Limit;
				SoundManager.instance.ChangeVolume(ratio / 50, audioComp.GetAudioSource());
				cameraObj.cameraComp.fieldOfView = Mathf.Lerp(60,75,ratio);
			}else{
				SoundManager.instance.ChangeVolume(0f, audioComp.GetAudioSource());
				isSwim = false;
				cameraObj.cameraComp.fieldOfView = 60;
			}
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

		if (Jump_Switch)
		{
			if (Jump_Type == eJump_Type.UP)
			{
				//縦ジャンプの予備動作
				if (HJump_NowTime <= HJump_AllTime * 0.2f)
				{
					//     this.transform.position += -this.gameObject.transform.transform.up * Jump_Fall * Time.deltaTime;
					this.gameObject.transform.Translate(-Vector3.up * Jump_Fall * Time.deltaTime);
					HJump_NowTime += Time.deltaTime;
				}
				else//縦ジャンプ
				{
					// this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + NowJump_speed * Time.deltaTime, this.transform.position.z);
					this.gameObject.transform.Translate(Vector3.up * NowJump_speed * Time.deltaTime);
					NowJump_speed -= 0.1f;
					if (this.transform.position.y < Start_Position.y && NowJump_speed <= 0.0f)
					{
						this.transform.position = new Vector3(this.transform.position.x, Start_Position.y, this.transform.position.z);
						Jump_Switch = false;
					}
				}
			}
			else if (Jump_Type == eJump_Type.SIDE)
			{
				//横ジャンプの予備動作
				if (SJump_NowTime <= SJump_AllTime * 0.2f)
				{
					this.transform.position += -this.gameObject.transform.up * Jump_Fall * Time.deltaTime;
					this.transform.position += this.gameObject.transform.forward * SJump_Speed * Time.deltaTime;
					SJump_NowTime += Time.deltaTime;
				}
				else  //???W?????v
				{
					this.transform.position += this.gameObject.transform.forward * SJump_Speed * Time.deltaTime;
					this.transform.position += this.gameObject.transform.up * Jump_Fall * Time.deltaTime;

					SJump_NowTime += Time.deltaTime;
					SJump_Speed += SJump_Acceleration * Time.deltaTime;
					// Debug.Log(SJump_Acceleration);
					if (SJump_NowTime > SJump_AllTime)
					{
						Jump_Switch = false;
					}
				}
			}
		}
		else
		{
			/*    NowVelocity += Velocity;
			 //???x????
				NowVelocity.x = Mathf.Clamp(NowVelocity.x, -Velocity_Limit, Velocity_Limit);
				NowVelocity.y = Mathf.Clamp(NowVelocity.y, -Velocity_Limit, Velocity_Limit);
				NowVelocity.z = Mathf.Clamp(NowVelocity.z, -Velocity_Limit, Velocity_Limit);

				// オブジェクト移動
				this.transform.position += NowVelocity * Time.deltaTime;*/

			if (Velocity == 0 && NowVelocity > 0)
			{
				NowVelocity -= Deceleration * Time.deltaTime;
				if (NowVelocity < 0)
					NowVelocity = 0;
			}
			else
			{
				NowVelocity += Velocity;
			}
			//???x????
			NowVelocity = Mathf.Clamp(NowVelocity, -Velocity_Limit, Velocity_Limit);

			// オブジェクト移動
			this.transform.Translate(Vector3.forward * (NowVelocity + Velocity_Addition) * Time.deltaTime);
			// this.gameObject.transform.forward *= NowVelocity;

			//カメラを横回転していないときだけ横移動ができる
			if (C_Camera.Looking_Left_Right())
			{
				//横移動制限
				if (Side_Move != 0)
				{
					Side_MoveNow += Side_Move * Time.deltaTime;
				}

				//横移動減速するときの挙動
				if (Side_Move == 0.0f)
				{
					if (Side_MoveNow > 0.1f)
					{
						Side_MoveNow -= 0.01f;
					}
					else if (Side_MoveNow <= 0.1f && Side_MoveNow > 0.0f)
					{
						Side_MoveNow -= Side_MoveNow;

						if (Side_MoveNow < 0.00f)
						{
							Side_MoveNow = Vector2.zero.x;
						}
					}
					else if (Side_MoveNow < -0.1f)
					{
						Side_MoveNow += 0.01f;
					}
					else if (Side_MoveNow >= -0.1f && Side_MoveNow < 0.0f)
					{
						Side_MoveNow -= Side_MoveNow;

						if (Side_MoveNow > 0.00f)
						{
							Side_MoveNow = Vector2.zero.x;
						}
					}
				}

				if (Side_MoveNow != 0.0f)
				{
					Side_MoveNow = Mathf.Clamp(Side_MoveNow, -Side_Move_Limit, Side_Move_Limit);

					Vector3 eulerAngles = this.gameObject.transform.eulerAngles;
					//オブジェクト横回転
					this.gameObject.transform.rotation *= Quaternion.AngleAxis(Side_MoveNow, this.gameObject.transform.up);


					// CameraObject.gameObject.transform.eulerAngles = Vector3.Lerp(eulerAngles, this.gameObject.transform.eulerAngles, Time.deltaTime * AttenRate);
					CameraObject.transform.eulerAngles = new Vector3(CameraCopy.x, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
					CameraObject.gameObject.transform.rotation *= Quaternion.AngleAxis(Side_MoveNow * Camera_Deferred_Power, this.transform.up);
					//    if (!C_Camera.Looking_Left_Right())

					{
						//     C_Camera.Horizontal_Rotation();
					}
				}
			}
		}

		Debug.Log(this.gameObject.transform.position);

		if(isLocalPlayer)CmdUpdateTransform(this.transform.position, this.transform.rotation);
		///Debug.Log();
	}

	[Command]
	private void CmdUpdateTransform(Vector3 motion, Quaternion quaternion)
	{
		this.transform.position = motion;
		this.transform.rotation = quaternion;
	}

	//????]
	private void OnMove(InputValue value)
	{
		if (!isCanMove) return;

		//Debug.Log("????");
		// MoveAction?????l???擾
		var axis = value.Get<Vector2>();

		Side_Move = axis.x * Side_Acceleration;
		//?????????
		// ??????x????
		//      Velocity = new Vector3(axis.x, 0, axis.y);
		//       Velocity *= 0.01f * Acceleration;
		//var axis = value.Get<Vector2>();
		//	Vector3 pos = this.transform.position;
		//	pos.x += value.;
		//	this.gameObject.transform.position = pos
	}

	private void OnJump()
	{
		if (!isCanMove) return;

		if (!Jump_Switch)
		{
			//???x??~
			Emergency_Stop();

			if (Jump_Type == eJump_Type.UP)
			{
				Jump_Switch = true;
				NowJump_speed = Jump_Speed;
				HJump_NowTime = 0.0f;
			}
			else if (Jump_Type == eJump_Type.SIDE)
			{
				Jump_Switch = true;
				SJump_NowTime = 0.0f;

				//??????????v???C???[????x????
				SJump_Speed = 0.0f;
			}

			ui.SetCharge();

			if (_isNowOrga)
			{
				CmdCreateWhrloop();  //???????o??
			}
		}
	}

	private void OnJumpChange()
	{
		if (!isCanMove) return;

		if (!Jump_Switch)
		{
			if (Jump_Type == eJump_Type.UP)
			{
				Jump_Type = eJump_Type.SIDE;
			}
			else if (Jump_Type == eJump_Type.SIDE)
			{
				Jump_Type = eJump_Type.UP;
			}
		}
	}

	//?A?N?Z??????
	private void OnAccelerator(InputValue value)
	{
		if (!isCanMove) return;

		var axis = value.Get<float>();


		// ??????x????
		Velocity = axis;

		Velocity *= Acceleration;
	}

	//??}??~
	private void Emergency_Stop()
	{
		NowVelocity = 0.0f;
		Velocity = 0.0f;
		Side_MoveNow = 0.0f;
	}
	public bool Moving_Left_Right()
	{
		if (Side_MoveNow != 0.0f)
			return false;

		return true;
	}

	public void OnUseItem(){
		if(_HaveItemData == null) return;
		_HaveItemData.UseEffect(this.transform.position ,this.transform.rotation);
		_HaveItemData = null;
		ui.SetItemTexture(ui.defaultItemTex);
	}

	public void SetVelocity(float velo){
		NowVelocity = velo;
	}
}