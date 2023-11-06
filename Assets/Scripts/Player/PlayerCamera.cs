using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class PlayerCamera : NetworkBehaviour
{
    //カメラ回転の速度
    public float CameraMove = 0.0f;
    //現在のカメラ回転度
    private float CameraMoveNow = 0.0f;
    //実際のカメラ回転
    private float CameraMoveReal = 0.0f;
    //カメラ回転の最大値
    private float Camera_Maximum = 90.0f;

    //プレイヤーオブジェクト
    private CPlayer C_Player;

    //水流に入っているかどうか
    public bool InWireLoop{ get; set; }
    // Start is called before the first frame update
    GameObject CameraObj;
    void Start()
    {
        // 子供を検索してカメラを確認する
        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameObject childObj = this.transform.GetChild(i).gameObject;
            // 接続時にプレイヤーごとにカメラを分ける
            if (childObj.name == "PlayerCamera")
            {
                CameraObj = childObj;
                CameraObj.SetActive(false);
                break;
            }
        }
        C_Player = GetComponent<CPlayer>();
        InWireLoop = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (CameraMove == 0.0f)
        {
            if (CameraMoveNow > 0.5f)
            {
                CameraObj.gameObject.transform.RotateAround(this.gameObject.transform.position, this.gameObject.transform.up, -0.5f);
                CameraMoveNow -= 0.5f;
            }
            else if (CameraMoveNow < -0.5f)
            {
                CameraObj.gameObject.transform.RotateAround(this.gameObject.transform.position, this.gameObject.transform.up, 0.5f);
                CameraMoveNow += 0.5f;
            }
            if (0.1f >= CameraMoveNow && CameraMoveNow > 0.0f)
            {
                CameraObj.gameObject.transform.RotateAround(this.gameObject.transform.position, this.gameObject.transform.up, -CameraMoveNow);
                CameraMoveNow = 0.0f;
            }
            else if (-0.1f <= CameraMoveNow && CameraMoveNow < 0.0f)
            {
                CameraObj.gameObject.transform.RotateAround(this.gameObject.transform.position, this.gameObject.transform.up, CameraMoveNow);
                CameraMoveNow = 0.0f;
            }
        }
        else
        {
            //カメラ移動
            CameraMoveNow += CameraMove;
            CameraMoveReal = CameraMove;
            //PlayerCamera.gameObject.transform.rotation = Quaternion.AngleAxis(CameraMoveNow, PlayerCamera.gameObject.transform.up);
            //カメラ移動の制限
            CameraMoveNow = Mathf.Clamp(CameraMoveNow, -Camera_Maximum, Camera_Maximum);
            if (Mathf.Abs(CameraMoveNow) >= Camera_Maximum)
            {
                CameraMoveReal = 0.0f;
            }

            //	Player_Camera.gameObject.transform.rotation = Player_Camera.gameObject.transform.rotation * Quaternion.Euler(0.0f, CameraMove, 0.0f,this.gameObject.transform.position);
            //    CameraObj.gameObject.transform.rotation *= Quaternion.AngleAxis(CameraMove, this.gameObject.transform.up);
            //     CameraObj.gameObject.transform.rotation *= Quaternion.AngleAxis(CameraMove, this.gameObject.transform.up);
            CameraObj.gameObject.transform.RotateAround(this.gameObject.transform.position, this.gameObject.transform.up, CameraMoveReal);
        }

        /*		Debug.Log("カメラ更新");
                if(CameraMove > Camera_Maximum)
                {
                    CameraMove = Camera_Maximum;
                }
                this.gameObject.transform.rotation = Quaternion.Euler(0.0f, CameraMove,0.0f);*/
    }

    public void MainSceneCamera()
    {
        if (CameraObj == null) return;
        Debug.Log("カメラ初期設定");
        // 一回カメラオブジェクトを起動する
        CameraObj.SetActive(true);
        // そのうえで自分がローカルプレイヤーでないならカメラを再度切る
        if (!isLocalPlayer)
        {
            Debug.Log("カメラOff");
            CameraObj.SetActive(false);
        }
    }

    public void SetCamera(bool flg)
    {
        Debug.Log(this.name + "カメラ停止命令");
        CameraObj.SetActive(flg);
    }

    //カメラ移動
    public void OnCameraRotation(InputValue valus)
    {
        if (!C_Player.isCanMove) return;

        //Debug.Log("動く");
        // MoveActionの入力値を取得
        var axis = valus.Get<Vector2>();
        CameraMove = axis.x;
    }
}
