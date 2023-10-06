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

public partial class CPlayer : NetworkBehaviour
{
    [SerializeField] bool _isNowOrga = false;
    public bool testMove = true;
    // アイテム所持するように
    // CItem _HaveItemData;

    //	enum eJump_Type
    //	{
    //		UP,
    //		SIDE,
    //	}

    // ** 移動類のパラメータ
    /*	private Vector3 Velocity;
        private float Jump_Speed = 10;
        private float NowJump_speed;
        private bool Jump_Switch;
        private Vector3 Start_Position;
        private eJump_Type Jump_Type;
        private float Dash_Speed = 5.0f;
        private float Dash_Time = 0.5f;
        private float Now_Time;*/

    /// </summary>
    /// // Start is called before the first frame update
    void Start()
    {
        GameRuleManager.instance.AddPlayerData(this);

        // Start_Position = this.transform.position;
        //  Jump_Type = eJump_Type.UP;
        CPlayerMoveStart();
    }

    // Update is called once per frame
    void Update()
    {
        /*     // オブジェクト移動
             this.gameObject.transform.position += Velocity * Time.deltaTime;

             if (Jump_Switch)
             {
                 if (Jump_Type == eJump_Type.UP)
                 {
                     this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + NowJump_speed * Time.deltaTime, this.gameObject.transform.position.z);

                     NowJump_speed -= 0.1f;
                     if (this.transform.position.y < Start_Position.y)
                     {
                         this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, Start_Position.y, this.gameObject.transform.position.z);
                         Jump_Switch = false;
                     }
                 }
                 else if (Jump_Type == eJump_Type.SIDE)
                 {
                     this.gameObject.transform.position += this.gameObject.transform.forward * SJump_Acceleration * Time.deltaTime;

                     SJump_NowTime += Time.deltaTime;
                     if (SJump_NowTime > SJump_AllTime)
                     {
                         Jump_Switch = false;
                     }
                 }
             }*/
        CplayerMoveUpdate();
    }

    //	[Command]
  /*  private void OnMove(InputValue value)
    {
        Debug.Log("動く");
        // MoveActionの入力値を取得
        var axis = value.Get<Vector2>();

        // 移動速度を保持
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
            }
            else if (Jump_Type == eJump_Type.SIDE)
            {
                Jump_Switch = true;
                SJump_NowTime = 0.0f;
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
  */
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("あたり");
        if (!other.gameObject.CompareTag("Player")) return;

        // 自分が鬼のときのみ通知をする
        if (_isNowOrga && GameRuleManager.instance.CheckOverCoolTime())
        {
            Debug.Log("当たり判定発生");
            GameRuleManager.instance.ChangeOrgaPlayer(other.gameObject.GetComponent<CPlayer>());
            _isNowOrga = false;
        }
    }

    public void ChangeToOrga()
    {
        _isNowOrga = true;
    }

}
