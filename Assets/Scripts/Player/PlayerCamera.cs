using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerCamera : NetworkBehaviour
{
	// Start is called before the first frame update
	GameObject CameraObj;
	void Start()
	{
		// 子供を検索してカメラを確認する
		for(int i = 0; i < this.transform.childCount; i++){
			GameObject childObj = this.transform.GetChild(i).gameObject;
			// 接続時にプレイヤーごとにカメラを分ける
			if(childObj.name == "PlayerCamera"){
				CameraObj = childObj;
				CameraObj.SetActive(false);
				break;
			}
		}
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void MainSceneCamera(){
		if(CameraObj == null) return;
		Debug.Log("カメラ初期設定");
		// 一回カメラオブジェクトを起動する
		CameraObj.SetActive(true);
		// そのうえで自分がローカルプレイヤーでないならカメラを再度切る
		if(!isLocalPlayer){
			Debug.Log("カメラOff");
			CameraObj.SetActive(false);
		}
	}

	public void SetCamera(bool flg){
		Debug.Log(this.name + "カメラ停止命令");
		CameraObj.SetActive(flg);
	}
}
