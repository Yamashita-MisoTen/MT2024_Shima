using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class ResultFade : NetworkBehaviour
{
	[SerializeField, Tooltip("フェードインの時の時間(秒)")] public float fadeInTime { get; } = 1f ;
	[SerializeField, Tooltip("フェードアウトの時の時間(秒)")] public float fadeOutTime { get; } = 1f;
	bool isFadeNow = false;
	bool isFadeOut = false;
	float requireTime = 0f;
	float targetTime = 0f;
	Material material;
	Canvas canvas;

	void Start(){
		gameObject.name = "Pf_ResultFade";
		canvas = this.gameObject.transform.GetComponent<Canvas>();
		material = this.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().material;
		material.SetFloat("_Ratio",1f);
	}

	// Update is called once per frame
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.U)){
			StartFadeOut();
		}
		if(!isFadeNow) return;

		requireTime += Time.deltaTime;
		var ratio = requireTime / targetTime;
		Debug.Log(ratio);
		if(requireTime > targetTime){
			requireTime = 0f;
			targetTime = 0f;
			isFadeNow = false;
		}

		if(isFadeOut){
			ratio = 1 - ratio;
			Debug.Log(ratio);
		}

		material.SetFloat("_Ratio",ratio);
	}

	public void StartFadeIn(){
		isFadeOut = false;
		isFadeNow = true;
		targetTime = fadeInTime;
	}
	public void StartFadeOut(){
		isFadeOut = true;
		isFadeNow = true;
		targetTime = fadeOutTime;
	}

	public void SetCamera(Camera camera){
		canvas.worldCamera = camera;
	}
}
