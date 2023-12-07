using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System.Security;
using Unity.VisualScripting;

public class PlayerUI : NetworkBehaviour
{
	// Start is called before the first frame update
	[SerializeField, Header("UI")] GameObject UICanvasObj;
	[SerializeField] float chargeTime;
	[SerializeField] public Texture2D defaultItemTex;
	float requireChargeTime;
	bool isCharge = false;
	TextMeshProUGUI playerStateText;		// プレイヤーの状態を表示しておく
	Image		playerHaveItemImage;
	Image		SlideJumpImage;
	Material	SlideJumpImageMaterial;
	RectTransform	SlideJumpImageAnchoredPosition;
	RectTransform	HighJumpImageAnchoredPosition;
	Image saturateUI;
	void Start()
	{
		for(int i = 0; i < UICanvasObj.transform.childCount; i++){
			var childObj = UICanvasObj.transform.GetChild(i);
			if(childObj.name == "PlayerState"){
				playerStateText = childObj.GetComponent<TextMeshProUGUI>();
			}
			if(childObj.name == "Item"){
				playerHaveItemImage = childObj.GetComponent<Image>();
				SetItemTexture(defaultItemTex);
			}
			if(childObj.name == "SlideJumpImage"){
				SlideJumpImage = childObj.GetComponent<Image>();
				SlideJumpImageMaterial = SlideJumpImage.material;
				SlideJumpImageAnchoredPosition = childObj.GetComponent<RectTransform>();
			}
			if(childObj.name == "SaturateImage"){
				saturateUI = childObj.GetComponent<Image>();
				saturateUI.gameObject.SetActive(false);
			}
		}
		UICanvasObj.SetActive(false);

	}

	// Update is called once per frame
	void Update()
	{
		if(isCharge){
			float ratio = requireChargeTime / chargeTime;
			SlideJumpImageMaterial.SetFloat("_Ratio",Mathf.Lerp(1f,0f,ratio));

			requireChargeTime += Time.deltaTime;
			if(chargeTime < requireChargeTime){
				requireChargeTime = 0f;
				SlideJumpImageMaterial.SetFloat("_Ratio",1.1f);
				SlideJumpImageMaterial.SetInt("_isCharge",0);
				isCharge = false;
			}
		}
	}

	public void ChangeOrgaPlayer(bool orgaflg){
		if(orgaflg){
			playerStateText.text = "Your Orga";
		}else{
			playerStateText.text = "";
		}
	}

	public void ChangeJumpType(CPlayer.eJump_Type jumptype){

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

	public void SetCharge(){
		isCharge = true;
		SlideJumpImageMaterial.SetFloat("_Ratio",0f);
		SlideJumpImageMaterial.SetInt("_isCharge",1);
	}

	public void SetActiveUICanvas(bool flg){
		UICanvasObj.SetActive(flg);
	}

	public void SetItemTexture(Texture2D tex){
		playerHaveItemImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),Vector2.zero);
	}

	public void SetActiveSaturateCanvas(bool flg){
		saturateUI.gameObject.SetActive(flg);
	}
}
