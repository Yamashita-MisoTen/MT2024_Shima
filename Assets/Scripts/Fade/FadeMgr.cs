using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DG.Tweening;
using UnityEngine.UI;

// ** フェードアウトしたらオブジェクト削除すするようにして
// タイトルシーンにおく→メインシーンでフェードインしたらリザルト用のフェードオブジェクト生成してこのオブジェクト削除
// っていう風にしたらタイトルシーンに戻ったとしても大丈夫な気がする

public class FadeMgr : NetworkBehaviour
{
	[SerializeField, Tooltip("フェードインの時の時間(秒)")] public float fadeInTime { get; } = 1f ;
	[SerializeField, Tooltip("フェードアウトの時の時間(秒)")] public float fadeOutTime { get; } = 1f;
	[SerializeField] Canvas canvasComp;
	private List<RectTransform> fadeObj;
	private List<Material> fadeObjMt;
	public bool isCompleteFadeOut { get; private set;}
	void Start()
	{
		this.name = "Pf_FadeCanvas";
		DontDestroyOnLoad(this);
		fadeObj = new List<RectTransform>();
		fadeObjMt = new List<Material>();
		canvasComp = this.gameObject.GetComponent<Canvas>();
		for(int i = 0; i < this.transform.childCount; i++){
			var rect = this.transform.GetChild(i).gameObject.GetComponent<RectTransform>();
			var mt = this.transform.GetChild(i).gameObject.GetComponent<Image>().material;
			fadeObj.Add(rect);
			fadeObjMt.Add(mt);
		}
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void StartFadeIn(){
		for(int i = 0; i < fadeObj.Count; i++){
			float targetX = 0f;
			if(i % 2 == 0){
				targetX = -1050;
			}else{
				targetX = 1050;
			}
			fadeObj[i].DOAnchorPosX(targetX,fadeInTime)
			.SetEase(Ease.InQuad)
			.OnComplete(() => CompleteFadeIn());
		}
	}

	public void StartFadeOut(){
		for(int i = 0; i < fadeObj.Count; i++){
			float targetX = 0f;
			if(i % 2 == 0){
				targetX = 1050;
			}else{
				targetX = -1050;
			}
			fadeObj[i].DOAnchorPosX(targetX,fadeOutTime)
			.SetEase(Ease.InQuad)
			.OnComplete(() => SetUpFadeIn());
		}
	}

	void SetUpFadeIn(){
		isCompleteFadeOut = true;
		for(int i = 0; i < fadeObjMt.Count; i++){
			fadeObjMt[i].SetInt("_isReverse",1);
			fadeObjMt[i].SetInt("_isInverse",1);
		}
	}

	void CompleteFadeIn(){
		NetworkServer.Destroy(this.gameObject);
	}

	public void SetRenderCamera(Camera cam){
		canvasComp.worldCamera = cam;
	}
}
