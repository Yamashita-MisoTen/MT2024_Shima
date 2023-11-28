using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using System;
using DG.Tweening;

public class PlayerCamera : NetworkBehaviour
{
    //カメラ回転のパラメーター
    public float CameraMove = 0.0f;
    //現在の
    private float CameraMoveNow = 0.0f;
    //実際のカメラ回転
    private float CameraMoveReal = 0.0f;
    //カメラ回転の最大値
    private float Camera_Maximum = 90.0f;

    //プレイヤーオブジェクト
    private GameObject Player_Object;
    private CPlayer C_Player;

    //カメラの位置初期化用
    private Vector3 Position_initialization;
    private Quaternion Rotation_initialization;

    // ** 渦潮関連変数
    public bool InWhirloop { get; set; }    // 入ってるかどうか
    private bool isInWhirloopCameraSet = false;
    private Vector3 initialCameraPos;   // 初期のカメラの位置
    private float initialCameraFov;     // 初期のカメラの視野角
    private float progressLerpTime = 0.0f;  // 補間用の経過時間
    [SerializeField] private float targetLerpTime = 0.0f;   // 補間にかかる時間
    [SerializeField] private Vector3 inWhirloopCameraPos;   // 渦潮内でのカメラの座標
    [SerializeField] private float inWhirloopCameraFov;     // 渦潮内でのカメラの視野角
                                                            // Start is called before the first frame update
    GameObject CameraObj;
    Camera cameraComp;

    bool Reverse = false;
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
                cameraComp = childObj.GetComponent<Camera>();
                break;
            }
        }

        C_Player = GetComponent<CPlayer>();
        InWhirloop = false;
        Position_initialization = CameraObj.transform.localPosition;
        Rotation_initialization = CameraObj.transform.rotation;

    //    Camera_Reverse(true);
      //  Reverse = true;   
    }

    // Update is called once per frame
    void Update()
    {
        // 渦潮に入ってるときのみに処理をする
        if (isInWhirloopCameraSet)
        {
            InWhirloopCameraUpdate();
            return;
        }

        if (C_Player.Moving_Left_Right())
        {
            Horizontal_Rotation();
        }
        //    if (C_Player.Moving_Left_Right() && CameraMoveNow < 2.0f)  //カメラが移動していないときは初期位置を持ち続けるようにする
        {
            //         CameraObj.gameObject.transform.rotation = Rotation_initialization;
            //         CameraObj.gameObject.transform.localPosition = Position_initialization;
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

    // ** 渦潮関連のカメラ
    public void SetCameraInWhirloop()
    {
        Debug.Log("カメラを変更する");
        // フラグの設定
        InWhirloop = true;
        // 必要な情報を格納していく
        // 初期のカメラ位置
        initialCameraPos = CameraObj.transform.position;
        initialCameraFov = cameraComp.fieldOfView;
        // 経過時間の初期化
        progressLerpTime = 0f;

        isInWhirloopCameraSet = true;
    }
    public void SetCameraOutWhirloop()
    {
        // フラグの設定
        InWhirloop = false;
        isInWhirloopCameraSet = true;
        // 経過時間の初期化
        progressLerpTime = 0f;
    }

    private void InWhirloopCameraUpdate()
    {
        if (InWhirloop)
        {
            // ここでカメラの位置etcを補完していく
            progressLerpTime += Time.deltaTime;
            float ratio = progressLerpTime / targetLerpTime;
            if (ratio > 1.0f)
            {
                ratio = 1.0f;
                isInWhirloopCameraSet = false;
            }
            // 補間処理をかける
            var campos = Vector3.Lerp(initialCameraPos, inWhirloopCameraPos, ratio);
            var camfov = Mathf.Lerp(initialCameraFov, inWhirloopCameraFov, ratio);
            // 計算結果を格納する
            //cameraComp.gameObject.transform.position = campos;
            cameraComp.fieldOfView = camfov;
        }
        else
        {
            // ここでカメラの位置etcを補完していく
            progressLerpTime += Time.deltaTime;
            // 出た時に処理
            float ratio = progressLerpTime / targetLerpTime;
            if (ratio > 1.0f)
            {
                ratio = 1.0f;
                isInWhirloopCameraSet = false;
            }
            // 補間処理をかける
            var campos = Vector3.Lerp(inWhirloopCameraPos, initialCameraPos, ratio);
            var camfov = Mathf.Lerp(inWhirloopCameraFov, initialCameraFov, ratio);
            // 計算結果を格納する
            //cameraComp.gameObject.transform.position = campos;
            cameraComp.fieldOfView = camfov;
        }
    }

    public bool Looking_lLeft_Right()
    {
        if (CameraMove != 0.0f || CameraMoveNow != 0.0f)
        {
            return false;
        }
        return true;
    }
    //カメラが横回転しているかどうか
    public bool Looking_Left_Right()
    {
        if (CameraMoveNow != 0.0f)
            return false;


        return true;
    }

    //カメラを横回転する関数
    public void Horizontal_Rotation()
    {
        if (CameraMove == 0.0f)
        {
            if (CameraMoveNow > 2.0f)
            {
                CameraObj.gameObject.transform.RotateAround(this.gameObject.transform.position, this.gameObject.transform.up, -0.5f);
                CameraMoveNow -= 0.5f;
            }
            else if (CameraMoveNow < -2.0f)
            {
                CameraObj.gameObject.transform.RotateAround(this.gameObject.transform.position, this.gameObject.transform.up, 0.5f);
                CameraMoveNow += 0.5f;
            }
            if (2.0f >= CameraMoveNow && CameraMoveNow > 0.0f)
            {
                CameraObj.gameObject.transform.RotateAround(this.gameObject.transform.position, this.gameObject.transform.up, -CameraMoveNow);
                CameraMoveNow -= CameraMoveNow;
                CameraObj.gameObject.transform.localRotation = Rotation_initialization;
                CameraObj.gameObject.transform.localPosition = Position_initialization;
            }
            else if (-2.0f <= CameraMoveNow && CameraMoveNow < 0.0f)
            {
                CameraObj.gameObject.transform.RotateAround(this.gameObject.transform.position, this.gameObject.transform.up, CameraMoveNow);
                CameraMoveNow -= CameraMoveNow;
                CameraObj.gameObject.transform.localRotation = Rotation_initialization;
                CameraObj.gameObject.transform.localPosition = Position_initialization;
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
    }

    private void OnPreCull()
    {
        if (Reverse)
        {
           // cameraComp.ResetProjectionMatrix();
         //   cameraComp.projectionMatrix = cameraComp.projectionMatrix * Matrix4x4.Scale(new Vector3(1,-1,1));
      //        CameraObj.transform.localRotation = 
        }
        else if (!Reverse)
        {
            cameraComp.ResetProjectionMatrix();
            cameraComp.projectionMatrix = cameraComp.projectionMatrix * Matrix4x4.Scale(new Vector3(1, 1, 1));
        }
    }

    private void OnPreRender()
    {
        if (Reverse)
        {
            GL.invertCulling = true;
        }
    }

    private void OnPostRender()
    {
        GL.invertCulling = false;
    }
    //カメラ上下反転(イベント用)
    public void Camera_Reverse(bool Key)
    {
    /*    if (Key)
        {
            cameraComp.ResetProjectionMatrix();
            cameraComp.projectionMatrix = cameraComp.projectionMatrix * Matrix4x4.Scale(new Vector3(1, -1, 1));
            GL.invertCulling = true;
        }
        else if (!Key)
        {
            cameraComp.ResetProjectionMatrix();
            cameraComp.projectionMatrix = cameraComp.projectionMatrix * Matrix4x4.Scale(new Vector3(1, 1, 1));
            GL.invertCulling = false;
        }*/
    }
}



