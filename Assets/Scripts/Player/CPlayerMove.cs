using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public partial class CPlayer : NetworkBehaviour
{
    enum eJump_Type
    {
        UP,
        SIDE,
    }

    // ** �ړ��ނ̃p�����[�^
    private Vector3 Velocity;
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

�@�@//�_�b�V���������x
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
    private float Fall_Acceleration  = 1.0f;

    // Start is called before the first frame update
    void CPlayerMoveStart()
    {
        Start_Position = this.transform.position;
        Jump_Type = eJump_Type.UP;
    }

    // Update is called once per frame
    void CplayerMoveUpdate()
    {
        // �I�u�W�F�N�g�ړ�
        this.gameObject.transform.position += Velocity * Time.deltaTime;

        if (Jump_Switch)
        {
            if (Jump_Type == eJump_Type.UP)
            { Debug.Log("�c�����");
                //�c�W�����v�̗\������
                if (HJump_NowTime <= HJump_AllTime * 0.2f)
                {
                    this.gameObject.transform.position += -this.gameObject.transform.transform.up * Jump_Fall * Time.deltaTime;
                    HJump_NowTime += Time.deltaTime;
                }
                else//�c�W�����v
                {
                    this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + NowJump_speed * Time.deltaTime, this.gameObject.transform.position.z);

                    NowJump_speed -= 0.1f;
                    Debug.Log(this.gameObject.transform.position.y);
                    if (this.transform.position.y < Start_Position.y && NowJump_speed <= 0.0f)
                    {
                        this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, Start_Position.y, this.gameObject.transform.position.z);
                        Jump_Switch = false;
                    }
                }
            }
            else if (Jump_Type == eJump_Type.SIDE)
            {
                Debug.Log("���W�����v");
                Debug.Log(SJump_NowTime);
                Debug.Log(SJump_AllTime);
                //���W�����v�̗\������
                if (SJump_NowTime <= SJump_AllTime * 0.2f)
                {
                    this.gameObject.transform.position += -this.gameObject.transform.transform.up * Jump_Fall * Time.deltaTime;
                    this.gameObject.transform.position += this.gameObject.transform.forward * SJump_Speed * Time.deltaTime;
                    SJump_NowTime += Time.deltaTime;
                }
                else  //���W�����v
                {
                    this.gameObject.transform.position += this.gameObject.transform.forward * SJump_Speed * Time.deltaTime;
                    this.gameObject.transform.position += this.gameObject.transform.transform.up * Jump_Fall * Time.deltaTime;

                    SJump_NowTime += Time.deltaTime;
                    SJump_Speed += SJump_Acceleration * Time.deltaTime;
                    Debug.Log(SJump_Acceleration);
                    if (SJump_NowTime > SJump_AllTime)
                    {
                        Jump_Switch = false;
                    }
                }
            }
        }

        //�������x�v�Z
        if (this.gameObject.transform.position.y > 0)
        {
            this.gameObject.transform.position += -this.gameObject.transform.up * Fall_Speed * Time.deltaTime;
            Fall_Speed += Fall_Acceleration  * Time.deltaTime;

            if(this.gameObject.transform.position.y <= 0)
            {
                this.gameObject.transform.position = new Vector3(this.transform.position.x, 0.0f, this.gameObject.transform.position.z);
                Fall_Speed = 0.0f;
            }
        }
    }

    private void OnMove(InputValue value)
    {
        Debug.Log("����");
        // MoveAction�̓��͒l���擾
        var axis = value.Get<Vector2>();

        // �ړ����x��ێ�
        Velocity = new Vector3(axis.x, 0, axis.y);
        //var axis = value.Get<Vector2>();
        //	Vector3 pos = this.transform.position;
        //	pos.x += value.;
        //	this.gameObject.transform.position = pos;
    }

    private void OnJump()
    {
        if (!Jump_Switch)
        {
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

                //�����Ō��݂̃v���C���[�̑��x����
                SJump_Speed = 1.0f;
            }
        }
    }

    private void OnJumpChange()
    {
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

    //�J�����̈ړ�
    private void OnCameraRotationL()
    {

    }

    private void OnCameraRotationR()
    {

    }
}
