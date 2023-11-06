using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class PlayerUI : NetworkBehaviour
{
	// Start is called before the first frame update
	[SerializeField] GameObject UICanvasObj;
	TextMeshProUGUI playerStateText;		// プレイヤーの状態を表示しておく
	TextMeshProUGUI playerJumpStateText;
	Image		playerHaveItemImage;
	void Start()
	{
		for(int i = 0; i < UICanvasObj.transform.childCount; i++){
			var childObj = UICanvasObj.transform.GetChild(i);
			if(childObj.name == "PlayerState"){
				playerStateText = childObj.GetComponent<TextMeshProUGUI>();
			}
			if(childObj.name == "JumpState"){
				playerJumpStateText = childObj.GetComponent<TextMeshProUGUI>();
			}
		}
		UICanvasObj.SetActive(false);

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void ChangeOrgaPlayer(bool orgaflg){
		if(orgaflg){
			playerStateText.text = "Your Orga";
		}else{
			playerStateText.text = "";
		}
	}

	public void ChangeJumpType(CPlayer.eJump_Type jumptype){
		playerJumpStateText.text = jumptype.ToString();
	}

	public void MainSceneUICanvas(){
		if(UICanvasObj == null) return;
		Debug.Log("PlayerUI初期設定");
		// 一回UIオブジェクトを起動する
		UICanvasObj.SetActive(true);
		// そのうえで自分がローカルプレイヤーでないならUIを再度切る
		if(!isLocalPlayer){
			Debug.Log("PlayerUI_Off");
			UICanvasObj.SetActive(false);
		}
	}
}
