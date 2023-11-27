using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class EventMgr : NetworkBehaviour
{
	[SerializeField] List<GameEvent> eventKind;
	[SerializeField] GameObject eventUIObj;
	[SerializeField] EventUI eventUI;
	void Start(){
		eventUI = Instantiate(eventUIObj).GetComponent<EventUI>();
	}
	public void SetUpUI(GameObject playerObj){
		for(int i = 0 ; i < playerObj.transform.childCount; i++){
			var obj = playerObj.transform.GetChild(i);
			if(obj.name == "PlayerCamera"){
				if(eventUI == null) eventUI = eventUIObj.GetComponent<EventUI>();
				eventUI.SetCameraData(obj.GetComponent<Camera>());
			}
		}
	}

	public void RandomEventLottery(){
		if(eventKind == null) return;

		StartPerformance();

		var eventnum = Random.Range(0, eventKind.Count);
		Debug.Log(eventKind[eventnum].GetEventName() + "発生");
		eventKind[eventnum].StartEvent();
	}

	void StartPerformance(){
		eventUI.StartUpEventUI();
	}
}
