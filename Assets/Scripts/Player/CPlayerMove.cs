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

	// ** �ړ��ނ̃p�����[�^
	private float Velocity;  //���͂���Ă��鑬�x
	private float NowVelocity;  //���݂̑��x
	[SerializeField, Header("�ړ��̑��x����")]
	private float Velocity_Limit;


	[SerializeField, Header("�ړ��̉����x")]
	private float Acceleration;

	//�C�x���g�Ȃǉ����̒l���ω�����Ƃ�
	private float Velocity_Addition;

	//?????x
	private float Deceleration = 0.5f;

	private float NowJump_speed;
	private float Jump_Speed = 10;
	private bool Jump_Switch;
	private Vector3 Start_Position;
	private eJump_Type Jump_Type;

	// ** ���_�b�V���̃p�����[�^�[
	//���_�b�V�������x
	private float SJump_Acceleration = 20.0f;
	//�S�̂̎���
	private float SJump_AllTime = 1.0f;
	//�W�����v�o�ߎ���
	private float SJump_NowTime;
	//���݂̑��x
	private float SJump_Speed;

	//�_�b�V���������x
	private float Jump_Fall = 1.0f;

	// ** �c�_�b�V���̃p�����[�^�[
	//�S�̂̎���
	private float HJump_AllTime = 1.0f;
	//�W�����v�o�ߎ���
	private float HJump_NowTime;

	// ** �����̃p�����[�^�[
	//�������x
	private float Fall_Speed;
	//���������x
	private float Fall_Acceleration = 1.0f;

	//����]�ړ��̃p�����[�^�[
	private float Side_Move = 0.0f;
	//����]�ړ��̃p�����[�^�[
	private float Side_MoveNow = 0.0f;
	//����]�ړ��̑��x����
	private float Side_Move_Limit = 2.0f;
	//����]�ړ��̑��x�����p
	private float Side_Acceleration = 3.0f;

	//�J�����I�u�W�F�N�g
	private GameObject CameraObject;

	//�J�����X�N���v�g
	private PlayerCamera C_Camera;

	private Vector3 CameraCopy = Vector3.zero;

	[SerializeField, Header("�J�����x���̑傫��")]
	private float Camera_Deferred_Power;

	[Header("�j���A�j���[�V����")]
	private Animator Swimming;

	private float AttenRate = 0.01f;    // Start is called before the first frame update
	void CPlayerMoveStart()
	{
		// �q�����������ăJ�������m�F����
		for (int i = 0; i < this.transform.childCount; i++)
		{

			GameObject childObj = this.transform.GetChild(i).gameObject;
			// �ڑ����Ƀv���C���[���ƂɃJ�����𕪂���
			if (childObj.name == "PlayerCamera")
			{
				CameraObject = childObj;
				CameraObject.SetActive(false);
			}

			//�A�j���[�V��������
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

		// �q�����������ăJ�������m�F����
		for (int i = 0; i < this.transform.childCount; i++)
		{
			GameObject childObj = this.transform.GetChild(i).gameObject;
			// �ڑ����Ƀv���C���[���ƂɃJ�����𕪂���
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
			// �����Ă�Ƃ��̉�
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

		//�A�j���[�V�����ɐ��l���
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
				//�c�W�����v�̗\������
				if (HJump_NowTime <= HJump_AllTime * 0.2f)
				{
					//     this.transform.position += -this.gameObject.transform.transform.up * Jump_Fall * Time.deltaTime;
					this.gameObject.transform.Translate(-Vector3.up * Jump_Fall * Time.deltaTime);
					HJump_NowTime += Time.deltaTime;
				}
				else//�c�W�����v
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
				//���W�����v�̗\������
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

				// �I�u�W�F�N�g�ړ�
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

			// �I�u�W�F�N�g�ړ�
			this.transform.Translate(Vector3.forward * (NowVelocity + Velocity_Addition) * Time.deltaTime);
			// this.gameObject.transform.forward *= NowVelocity;

			//�J����������]���Ă��Ȃ��Ƃ��������ړ����ł���
			if (C_Camera.Looking_Left_Right())
			{
				//���ړ�����
				if (Side_Move != 0)
				{
					Side_MoveNow += Side_Move * Time.deltaTime;
				}

				//���ړ���������Ƃ��̋���
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
					//�I�u�W�F�N�g����]
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
		// MoveAction?????l???��
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