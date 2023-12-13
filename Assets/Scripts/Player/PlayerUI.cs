using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System.Security;
using Unity.VisualScripting;
using System.Linq;
using DG.Tweening;

public class PlayerUI : NetworkBehaviour
{
	// Start is called before the first frame update
	[SerializeField, Header("UI")] GameObject UICanvasObj;
	[SerializeField] float chargeTime;
	[SerializeField] public Texture2D defaultItemTex;
	float requireChargeTime;
	bool isCharge = false;
	Image		playerHaveItemImage;
	Image		SlideJumpImage;
	Material	SlideJumpImageMaterial;
	RectTransform	SlideJumpImageAnchoredPosition;
	Image saturateUI;
	Image saturateCircleUI;
	Image roleUI;
	[SerializeField] Sprite roleTexOrga;
	[SerializeField] Sprite roleTexPenguin;
	SaturatedAccele satirateCircleComp;
	Image[] tutolialImage;
	RectTransform[] tutolialImageTrans;
	//item�p�ϐ�
	private int mCurFrame = 0;
	private float mDelta = 0;
	public float FPS = 5;
	public List<Sprite> SpriteFrames;
	public bool isPlaying = false;
	public bool Loop = false;
	public int tutorialNum {get; private set;}= 0;
	bool isTutorialAnim = false;
	public int FrameCount
	{
		get
		{
			return SpriteFrames.Count;
		}
	}
	void Start()
	{
		for(int i = 0; i < UICanvasObj.transform.childCount; i++){
			var childObj = UICanvasObj.transform.GetChild(i);
			if(childObj.name == "RoleImage"){
				roleUI = childObj.GetComponent<Image>();
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
			if(childObj.name == "SaturatedLineCircle"){
				saturateCircleUI = childObj.GetComponent<Image>();
				satirateCircleComp = childObj.GetComponent<SaturatedAccele>();
				saturateCircleUI.gameObject.SetActive(false);
			}
			if(childObj.name == "Tutorial1"){
				if(tutolialImage == null) tutolialImage = new Image[2];
				if(tutolialImageTrans == null) tutolialImageTrans = new RectTransform[2];
				tutolialImage[0] = childObj.GetComponent<Image>();
				tutolialImageTrans[0] = childObj.GetComponent<RectTransform>();
			}
			if(childObj.name == "Tutorial2"){
				if(tutolialImage == null) tutolialImage = new Image[2];
				if(tutolialImageTrans == null) tutolialImageTrans = new RectTransform[2];
				tutolialImage[1] = childObj.GetComponent<Image>();
				tutolialImageTrans[1] = childObj.GetComponent<RectTransform>();
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

        //itmeUI�ϐ��X�V�p
        if (isPlaying)
        {
			mDelta += Time.deltaTime;
			if (mDelta > 1 / FPS)
			{
				mDelta = 0;
				mCurFrame++;
				if (mCurFrame >= FrameCount)
				{
					if (Loop)
					{
						mCurFrame = 0;
					}
				}
				SetSprite(mCurFrame);
			}
		}
	}

	public void ChangeOrgaPlayer(bool orgaflg){
		if(orgaflg){
			roleUI.sprite = roleTexOrga;
		}else{
			roleUI.sprite = roleTexPenguin;
		}
	}

	public void ChangeJumpType(CPlayer.eJump_Type jumptype){

	}

	public void MainSceneUICanvas(){
		if(UICanvasObj == null) return;
		Debug.Log("PlayerUI�����ݒ�");
		// ���UI�I�u�W�F�N�g���N������
		UICanvasObj.SetActive(true);
		// ���̂����Ŏ��������[�J���v���C���[�łȂ��Ȃ�UI���ēx�؂�
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
		StopItemUI();
		playerHaveItemImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),Vector2.zero);
		//SetSprite(mCurFrame);
	}
	private void SetSprite(int idx)
	{
		playerHaveItemImage.sprite = SpriteFrames[idx];
	}
	public void StartItemUI()
    {
		isPlaying = true;
	}
	public void StopItemUI()
	{
		mCurFrame = 0;
		//SetSprite(mCurFrame);
		isPlaying = false;
	}

	public void SetActiveSaturateCanvas(bool flg){
		saturateUI.gameObject.SetActive(flg);
		if(flg){
			SetPlaneDistance(1);
			saturateCircleUI.gameObject.SetActive(flg);
			satirateCircleComp.StartAccele();
		}else{
			SetPlaneDistance(2);
			satirateCircleComp.StopAccele();
		}
	}

	public void SetPlaneDistance(float value){
		UICanvasObj.GetComponent<Canvas>().planeDistance = value;
	}

	public void NextTutolialPage(){
		if(isTutorialAnim) return;
		if(tutorialNum + 1 == tutolialImage.Count()) return;
		tutorialNum++;
		isTutorialAnim = true;

		for(int i = 0; i < tutolialImage.Count(); i++){
			var pos = tutolialImageTrans[i].anchoredPosition.x - 1920;
			Debug.Log(pos);
			tutolialImageTrans[i].DOAnchorPosX(pos, 1.0f).
			OnComplete(() => DOVirtual.DelayedCall(1.0f, () => isTutorialAnim = false));
		}
	}
	public void BackTutolialPage(){
		if(isTutorialAnim) return;
		if(tutorialNum == 0) return;
		tutorialNum--;
		isTutorialAnim = true;
		for(int i = 0; i < tutolialImage.Count(); i++){
			var pos = tutolialImageTrans[i].anchoredPosition.x + 1920;
			Debug.Log(pos);
			tutolialImageTrans[i].DOAnchorPosX(pos, 1.0f).
			OnComplete(() => DOVirtual.DelayedCall(1.0f, () => isTutorialAnim = false));
		}
	}

	public void CloseTutorialImage(){
		for(int i = 0; i < tutolialImage.Count(); i++){
			var pos = tutolialImageTrans[i].position.y + 1080;
			tutolialImageTrans[i].DOAnchorPosY(pos, 1.0f).
			OnComplete(() => tutolialImage[i].gameObject.SetActive(false));
		}
	}
}
