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

    // ** 移動類のパラメータ
    private float Velocity;
    private float NowVelocity;
    [SerializeField, Header("速度制限")]
    private float Velocity_Limit;

    [SerializeField, Header("加速度")]
    private float Acceleration;

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

    //横回転のパラメーター
    private float Side_Move = 0.0f;
    private float Side_MoveNow = 0.0f;

    // Start is called before the first frame update
    void CPlayerMoveStart()
    {
        Start_Position = this.transform.position;
        Jump_Type = eJump_Type.UP;
        NowVelocity = 0.0f;
    }

    // Update is called once per frame
    void CplayerMoveUpdate()
    {
        if (Jump_Switch)
        {
            if (Jump_Type == eJump_Type.UP)
            {
                //縦ジャンプの予備動作
                if (HJump_NowTime <= HJump_AllTime * 0.2f)
                {
                    this.gameObject.transform.position += -this.gameObject.transform.transform.up * Jump_Fall * Time.deltaTime;
                    HJump_NowTime += Time.deltaTime;
                }
                else//縦ジャンプ
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
                //横ジャンプの予備動作
                if (SJump_NowTime <= SJump_AllTime * 0.2f)
                {
                    this.gameObject.transform.position += -this.gameObject.transform.transform.up * Jump_Fall * Time.deltaTime;
                    this.gameObject.transform.position += this.gameObject.transform.forward * SJump_Speed * Time.deltaTime;
                    SJump_NowTime += Time.deltaTime;
                }
                else  //横ジャンプ
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
        else
        {
            /*    NowVelocity += Velocity;
             //速度制限
                NowVelocity.x = Mathf.Clamp(NowVelocity.x, -Velocity_Limit, Velocity_Limit);
                NowVelocity.y = Mathf.Clamp(NowVelocity.y, -Velocity_Limit, Velocity_Limit);
                NowVelocity.z = Mathf.Clamp(NowVelocity.z, -Velocity_Limit, Velocity_Limit);

                // オブジェクト移動
                this.gameObject.transform.position += NowVelocity * Time.deltaTime;*/

            NowVelocity += Velocity;
            //速度制限
            NowVelocity = Mathf.Clamp(NowVelocity, -Velocity_Limit, Velocity_Limit);

            // オブジェクト移動
            this.gameObject.transform.Translate(this.gameObject.transform.forward * NowVelocity * Time.deltaTime);
           // this.gameObject.transform.forward *= NowVelocity;

            //オブジェクト横回転
            this.gameObject.transform.rotation *= Quaternion.AngleAxis(Side_Move, this.gameObject.transform.up);
    }
        //落下速度計算
        if (this.gameObject.transform.position.y > 0)
        {
            this.gameObject.transform.position += -this.gameObject.transform.up * Fall_Speed * Time.deltaTime;
            Fall_Speed += Fall_Acceleration * Time.deltaTime;

            if (this.gameObject.transform.position.y <= 0)
            {
                this.gameObject.transform.position = new Vector3(this.transform.position.x, 0.0f, this.gameObject.transform.position.z);
                Fall_Speed = 0.0f;
            }
        }
    }

    //横回転
    private void OnMove(InputValue value)
    {
        if(!isCanMove) return;

        //Debug.Log("動く");
        // MoveActionの入力値を取得
        var axis = value.Get<Vector2>();

        Side_Move = axis.x;

        //入力情報を保持
        // 移動速度を保持
  //      Velocity = new Vector3(axis.x, 0, axis.y);
 //       Velocity *= 0.01f * Acceleration;
        //var axis = value.Get<Vector2>();
        //	Vector3 pos = this.transform.position;
        //	pos.x += value.;
        //	this.gameObject.transform.position = pos;
    }

    private void OnJump()
    {
        if(!isCanMove) return;

        if (!Jump_Switch)
        {
            //速度停止
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

                //ここで現在のプレイヤーの速度を代入
                SJump_Speed = 0.0f;
            }
            CreateWhirloop(1);  //水流を出す
        }
    }

    private void OnJumpChange()
    {
        if(!isCanMove) return;

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

    //アクセル操作
    private void OnAccelerator(InputValue value)
    {
        if (!isCanMove) return;

        var axis = value.Get<float>();


        // 移動速度を保持
        Velocity = axis;

        Velocity *= Acceleration;
    }

    //緊急停止
    private void Emergency_Stop()
    {
        NowVelocity = 0.0f;
        Velocity = 0.0f;
    }
}