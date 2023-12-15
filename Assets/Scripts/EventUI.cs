using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class EventUI : NetworkBehaviour
{
	GameObject eventobj;
	RectTransform eventObjTrans;
	TextMeshProUGUI eventName;
	TextMeshProUGUI eventExplanatory;
	Material eventTimeMt;
	// Start is called before the first frame update
	void Start()
	{
		for(int i = 0; i < this.gameObject.transform.childCount; i++){
			var child = this.gameObject.transform.GetChild(i).gameObject;
			if(child.name == "Event"){
				eventobj = child;
				eventObjTrans = child.GetComponent<RectTransform>();
				SetUpEvent(eventobj);
			}
		}
		// 必要タイミングで出せばいいので消しておく
		for(int i = 0; i < this.gameObject.transform.childCount; i++){
			this.gameObject.transform.GetChild(i).gameObject.SetActive(false);
		}
	}

	void SetUpEvent(GameObject obj){
		for(int i = 0; i < obj.transform.childCount; i++){
			var child = obj.transform.GetChild(i).gameObject;
			switch(child.name){
				case "EventTime":
					eventTimeMt = child.GetComponent<Image>().material;
					break;
				case "EventName":
					eventName = child.gameObject.GetComponent<TextMeshProUGUI>();
					break;
				case "EventExplanatory":
					eventExplanatory = child.gameObject.GetComponent<TextMeshProUGUI>();
					break;
			}
		}
	}

	public void SetCameraData(Camera camera){
		this.GetComponent<Canvas>().worldCamera = camera;
	}

	// Update is called once per frame
	void Update()
	{
		if(!isServer) return;
	}

	public void StartUpEventUI(string name, string explanatory){
		// 子供を全部起動する
		for(int i = 0; i < this.gameObject.transform.childCount; i++){
			this.gameObject.transform.GetChild(i).gameObject.SetActive(true);
		}

		EventBeltSetUp(name, explanatory);
	}

	private void EventBeltSetUp(string name, string explanatory){
		eventName.text = name;
		eventExplanatory.text = explanatory;
		Debug.Log(eventobj.transform.position);
		eventObjTrans.DOAnchorPosY(-540, 1.0f);
	}

	public void SetRatio(float ratio){
		var r = 1f - ratio;
		if(r < 0) r = 0;
		eventTimeMt.SetFloat("_Ratio", 1f - ratio);
	}

	public void StopEventUI(){
		for(int i = 0; i < this.gameObject.transform.childCount; i++){
			var child = this.gameObject.transform.GetChild(i).gameObject;
			if(child == eventobj) continue;
			child.SetActive(false);
		}
	}
	public void StopEventBeltUI(){
		Debug.Log(eventobj.transform.position);
		eventObjTrans.DOAnchorPosY(-590, 1.0f)
		.OnComplete(() => eventobj.SetActive(false));
	}
}
