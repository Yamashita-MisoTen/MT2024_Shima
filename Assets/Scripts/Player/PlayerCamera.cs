using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerCamera : NetworkBehaviour
{
	// Start is called before the first frame update
	GameObject CameraObj = null;
	void Start()
	{
		if(!isLocalPlayer){
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
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
