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

	// ** �ړ��ނ̃p�����[�^
	private float Velocity;  //���͂���Ă��鑬�x
	public float NowVelocity;  //���݂̑��x
	[SerializeField, Header("�ړ��̑��x����")]
	private float Velocity_Limit;


	[SerializeField, Header("�ړ��̉����x")]
	private float Acceleration;

	[SerializeField, Tooltip("�W�����v�̃N�[���^�C��")] private float jumpCollTime = 3f;

	//�C�x���g�Ȃǉ����̒l���ω�����Ƃ�
	private float Velocity_Addition;
	[SerializeField] float orgaPlusSpeed = 5.0f;
	private float velocity_Orga;

	//?????x
	private float Deceleration = 0.5f;
	private bool isCanJump = true;

	private Vector3 Start_Position;
	private eJump_Type Jump_Type;

	// �W�����v�̃N�[���^�C���p�̌o�ߎ���
	private float requireJumpTime = 0f;

	//�_�b�V���������x
	private float Jump_Fall = 1.0f;

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
				continue;
			}

			//�A�j���[�V��������
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

		// �Q���ɏ���Ă�Ƃ��͊�{�����̍X�V�����Ȃ�
		if (isOnWhirloop){
			return;
		}

		// �����Ă�Ƃ��̉�
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

		// �T�[�o�[���ł̊e�v���C���[�̓��͒l�ɉ����������x���v�Z����
		// �T�[�o�[���ł̏��������Ȃ��ƃN���C�A���g�Ő�����X�V�񐔂̍��ŃY���������邽��
		if (isServer) PlayerMoveServerProcess();
		// �N���C�A���g�ŃT�[�o�[�����瓾�����ōX�V���s��
		PlayerMoveClientProcess();

	}

	void PlayerMoveServerProcess()
	{
		// ** �T�[�o�[���Ńv���C���[�̋����𐧌䂷�� ** //
		// ** ��{�ړ�
		var velocity = ForwardMove();

		// ** ���E�ւ̈ړ��̋���
		var sidevelocity = SideMove();

		// �X�V���e�N���C�A���g�ɓ�������
		RpcSendPlayerTransform(velocity, sidevelocity);
	}

	void PlayerMoveClientProcess()
	{
		// �N���C�A���g���ōŏI�̍X�V���s��
		if(isOnWhirloop) return;
		// �ړ�
		this.transform.position += this.gameObject.transform.forward * (NowVelocity + Velocity_Addition + velocity_Orga) * Time.deltaTime;
		// ��]
		var qt = this.transform.rotation;
		if(Side_MoveNow != 0.0f){
			qt *= Quaternion.AngleAxis(Side_MoveNow, this.gameObject.transform.up);
			// ��]�̂Ƃ��̃v���C���[�̃J�����̍X�V����
			var euler = new Vector3(CameraCopy.x, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
			var camqt = Quaternion.AngleAxis(Side_MoveNow * Camera_Deferred_Power, this.transform.up);
			cameraObj.CameraMoveforPlayerMove(euler, camqt);
		}
		this.transform.rotation = qt;
	}

	float ForwardMove(){
		// ** ��{�ړ�
		// �����x
		if (Velocity == 0f && NowVelocity > 0f) {
			// ��������
			NowVelocity -= Deceleration * Time.deltaTime;
			// �ŏI�␳
			if (NowVelocity < 0f) NowVelocity = 0f;
		}
		else {
			NowVelocity += Velocity;
		}
		// �����x�ɐ�����������
		NowVelocity = Mathf.Clamp(NowVelocity, -Velocity_Limit, Velocity_Limit);
		// ���ۂɓ�����
		return NowVelocity;
	}

	float SideMove(){
		// ** ���E�ւ̈ړ��̋���
		Quaternion qt = this.gameObject.transform.rotation;

		// �J�����̑�������Ă�Ƃ��͉�����ł��Ȃ�
		if (!C_Camera.Looking_Left_Right()) return 0f;
		//���ړ�
		if (Side_Move != 0) Side_MoveNow += Side_Move * Time.deltaTime;

		// ��������
		if(Side_Move == 0.0f){
			// �����x�̐�Βl��0.1f�ȉ��ɂȂ����ꍇ�̏���
			if(Mathf.Abs(Side_MoveNow) <= 0.1f){
				Side_MoveNow = Vector2.zero.x;
			}else{
				int pol = 1;	// �␳�l
				if(Side_MoveNow > 0f){
					pol = -1;
				}
				// ����
				Side_MoveNow += pol * 0.01f;
			}
		}
		// ���E�̈ړ��l�ɐ�����������
		Side_MoveNow = Mathf.Clamp(Side_MoveNow, -Side_Move_Limit, Side_Move_Limit);
		return Side_MoveNow;
	}

	private void OnAccelerator(InputValue value) // �A�N�Z��
	{
		if (!isCanMove) return;
		if (isOnWhirloop) return;

		var axis = value.Get<float>();
		// �����x�̍X�V
		CmdUpdateVelocity(axis * Acceleration);
	}
	private void OnMove(InputValue value) // ���E�̓���
	{
		if (!isCanMove) return;
		if (isOnWhirloop) return;

		var axis = value.Get<Vector2>();
		CmdUpdateSideMove(axis.x * Side_Acceleration);
	}

	public float GetNowSpeed(){
		return NowVelocity + Velocity_Addition + velocity_Orga;
	}

	private void OnJump() // �W�����v
	{
		if (!isCanMove) return;		// �ړ��\�ȏꍇ�̂�
		// �W�����v�̐������m�F����
		if(!isCanJump) return;		// �W�����v�̃N�[���^�C��
		if(!_isNowOrga) return;		// �������S�̂Ƃ��̂�
		if(isOnWhirloop) return;	// �Q���ɏ���ĂȂ��Ƃ��̂�
		if(!isLocalPlayer) return;	// ����L�����̂Ƃ��̂�
		// �W�����v�̕ϐ��X�V
		isCanJump = false;
		// UI���X�V����
		ui.SetCharge();
		// �Q���𐶐�����
		CmdCreateWhrloop();
	}

	public void OnUseItem()	// �A�C�e���g�p
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

	// �ʐM�ŗp���铯���֐��Q

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

	// �ً}��~�p
	[Command]
	private void CmdEmergencyStop()
	{
		Debug.Log("�ً}��~");
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